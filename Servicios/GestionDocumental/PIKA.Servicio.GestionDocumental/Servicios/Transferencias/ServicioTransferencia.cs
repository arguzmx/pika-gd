﻿
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioTransferencia : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioTransferencia
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Transferencia> repo;
        private IRepositorioAsync<EstadoTransferencia> repoET;
        private IRepositorioAsync<Archivo> repoA;
        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;
        private readonly ConfiguracionServidor ConfiguracionServidor;
        private IOTransferencia ioT;
        private ILogger<ServicioCuadroClasificacion> LoggerCC;
        public ServicioTransferencia(
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger,
            IOptions<ConfiguracionServidor> Config) : base(proveedorOpciones, Logger)
        {
            this.ConfiguracionServidor = Config.Value;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Transferencia>(new QueryComposer<Transferencia>());
            this.repoET = UDT.ObtenerRepositoryAsync<EstadoTransferencia>(new QueryComposer<EstadoTransferencia>());
            this.repoA = UDT.ObtenerRepositoryAsync<Archivo>(new QueryComposer<Archivo>());
            this.ioT = new IOTransferencia(Logger, proveedorOpciones);

        }
        public async Task<bool> Existe(Expression<Func<Transferencia, bool>> predicado)
        {
            List<Transferencia> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteET(Expression<Func<EstadoTransferencia, bool>> predicado)
        {
            List<EstadoTransferencia> l = await this.repoET.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }
        public async Task<bool> ExisteA(Expression<Func<Archivo, bool>> predicado)
        {
            List<Archivo> l = await this.repoA.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Transferencia> CrearDesdeTemaAsync(Transferencia entity, string TemaId, bool EliminarTema = false, CancellationToken cancellationToken = default)
        {

            try
            {

                await VerificaDatosCreacion(entity);
                List<string> activos = (await this.UDT.Context.ActivosSeleccionados.Where(x => x.TemaId == TemaId).ToListAsync())
                    .Select(a => a.Id).ToList();
                List<string> validos = await this.UDT.Context.ActivosValidosTransferencia(activos, entity.ArchivoOrigenId, entity.CuadroClasificacionId, entity.EntradaClasificacionId);


                entity.Nombre = entity.Nombre.Trim();
                entity.Id = System.Guid.NewGuid().ToString();
                entity.FechaCreacion = DateTime.UtcNow;
                entity.CantidadActivos = validos.Count;
                await this.repo.CrearAsync(entity);
                UDT.SaveChanges();



                foreach(string id in validos )
                {
                    this.UDT.Context.ActivosTransferencia.Add(
                        new ActivoTransferencia() { Id = Guid.NewGuid().ToString(), ActivoId = id, Declinado = false, TransferenciaId = entity.Id }
                        );
                }
                this.UDT.SaveChanges();

                await this.UDT.Context.ActualizaActivosEnTrasnferencia(validos, true);


                if(EliminarTema && validos.Count >0)
                {
                    string sqls = $"delete from {DBContextGestionDocumental.TablaActivoSelecionado} where TemaId = '{TemaId}' " +
                        $"and Id in ({validos.MergeSQLStringList()})";
                    await UDT.Context.Database.ExecuteSqlRawAsync(sqls);

                    if(activos.Count == validos.Count)
                    {
                        sqls = $"delete from {DBContextGestionDocumental.TablaTemasActivos} where Id='{TemaId}'";
                        await UDT.Context.Database.ExecuteSqlRawAsync(sqls);
                    }
                }

                return entity.Copia();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
        }



        private async Task VerificaDatosCreacion(Transferencia entity)
        {
            if (!await ExisteET(x => x.Id.Equals(entity.EstadoTransferenciaId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.EstadoTransferenciaId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoOrigenId); }
            if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExErrorRelacional(entity.ArchivoDestinoId); }
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            { throw new ExElementoExistente(entity.Nombre); }

            if (entity.ArchivoOrigenId == entity.ArchivoDestinoId)
            {
                throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ODIDENTICO");
            }

            if (!string.IsNullOrEmpty(entity.CuadroClasificacionId) && !string.IsNullOrEmpty(entity.EntradaClasificacionId))
            {
                if (!this.UDT.Context.EntradaClasificacion.Any(x => x.CuadroClasifiacionId == entity.CuadroClasificacionId && x.Id == entity.EntradaClasificacionId))
                {
                    throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ERRORCCEC");
                }
            }

        }


        public async Task<Transferencia> CrearAsync(Transferencia entity, CancellationToken cancellationToken = default)
        {

            await VerificaDatosCreacion(entity);
            entity.Nombre = entity.Nombre.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity.Copia();
        }
        public async Task ActualizarAsync(Transferencia entity)
        {
            try
            {
                Transferencia o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }
                
                if (!await ExisteA(x => x.Id.Equals(entity.ArchivoOrigenId, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExErrorRelacional(entity.ArchivoOrigenId); }
                
                if (!await ExisteA(x => x.Id.Equals(entity.ArchivoDestinoId, StringComparison.InvariantCultureIgnoreCase)))
                { throw new ExErrorRelacional(entity.ArchivoDestinoId); }
                
                if (await Existe(x => x.Id != entity.Id
                && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
                {
                    throw new ExElementoExistente(entity.Nombre);
                }

                if (entity.ArchivoOrigenId == entity.ArchivoDestinoId)
                {
                    throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ODIDENTICO");
                }

                if (!string.IsNullOrEmpty(entity.CuadroClasificacionId) && !string.IsNullOrEmpty(entity.EntradaClasificacionId))
                {
                    if (!this.UDT.Context.EntradaClasificacion.Any(x => x.CuadroClasifiacionId == entity.CuadroClasificacionId && x.Id == entity.EntradaClasificacionId))
                    {
                        throw new ExDatosNoValidos("APICODE-TRANSFERENCIAS-ERRORCCEC");
                    }
                }

                o.Nombre = entity.Nombre.Trim();
                o.Folio = entity.Folio;
                o.ArchivoDestinoId = entity.ArchivoDestinoId;
                o.EntradaClasificacionId = entity.EntradaClasificacionId;
                o.CuadroClasificacionId = entity.CuadroClasificacionId;

                UDT.Context.Entry(o).State = EntityState.Modified;
                UDT.SaveChanges();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }

        }
        private Consulta GetDefaultQuery(Consulta query)
        {
            if (query != null)
            {
                query.indice = query.indice < 0 ? 0 : query.indice;
                query.tamano = query.tamano <= 0 ? 20 : query.tamano;
                query.ord_columna = string.IsNullOrEmpty(query.ord_columna) ? DEFAULT_SORT_COL : query.ord_columna;
                query.ord_direccion = string.IsNullOrEmpty(query.ord_direccion) ? DEFAULT_SORT_DIRECTION : query.ord_direccion;
            }
            else
            {
                query = new Consulta() { indice = 0, tamano = 20, ord_columna = DEFAULT_SORT_COL, ord_direccion = DEFAULT_SORT_DIRECTION };
            }
            return query;
        }
        public async Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }
        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Transferencia a;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                a = await this.repo.UnicoAsync(x => x.Id == Id);
                if (a != null)
                {
                    UDT.Context.Entry(a).State = EntityState.Deleted;
                    listaEliminados.Add(a.Id);
                }
            }
            UDT.SaveChanges();
            return listaEliminados;
        }
        public Task<List<Transferencia>> ObtenerAsync(Expression<Func<Transferencia, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Transferencia>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public async Task<Transferencia> UnicoAsync(Expression<Func<Transferencia, bool>> predicado = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> ordenarPor = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Transferencia t = await this.repo.UnicoAsync(predicado);
            return t.Copia();
        }
        public async Task<byte[]> ReporteTransferencia(string TransferenciaId, string[] Columnas)
        {
            if (Columnas.Count() < 0)
                Columnas = "EntradaClasificacion.Clave,EntradaClasificacion.Nombre,Nombre,Asunto,FechaApertura,FechaCierre,CodigoOptico,CodigoElectronico,Reservado,Confidencial,Ampliado".Split(',').ToList().Where(x => !string.IsNullOrEmpty(x)).ToArray();

            string f = await ioT.Obtenetdatos(TransferenciaId, Columnas, ConfiguracionServidor.ruta_cache_fisico, ConfiguracionServidor.separador_ruta);
            byte[] b = File.ReadAllBytes(f);
            return b;
        }


        public async Task<string[]> EliminarRelaciones(List<Archivo> listaArchivos)
        {
            //ServicioEventoTransferencia set = new ServicioEventoTransferencia(this.proveedorOpciones, this.logger);
            //ServicioComentarioTransferencia cct = new ServicioComentarioTransferencia(this.proveedorOpciones,this.logger);
            //ServicioActivoTransferencia sat = new ServicioActivoTransferencia(this.proveedorOpciones, this.logger);

            //List<Transferencia> ListaTranferencia = await repo.ObtenerAsync(x => x.ArchivoOrigenId.Contains(listaArchivos.Select(x => x.Id).FirstOrDefault())).ConfigureAwait(false);
            //List<Transferencia> listaT =await repo.ObtenerAsync(x => x.ArchivoDestinoId.Contains(listaArchivos.Select(x => x.Id).FirstOrDefault())).ConfigureAwait(false);
            //ListaTranferencia.AddRange(listaT);
            //List<EventoTransferencia> ListaEvento = await set.ObtenerAsync(x=>x.TransferenciaId.Contains(ListaTranferencia.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
            //List<ComentarioTransferencia> ListaComentarioTransferencia = await cct.ObtenerAsync(x=>x.TransferenciaId.Contains(ListaTranferencia.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);

            //await set.Eliminar(IdsEliminados(ListaEvento.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //await cct.Eliminar(IdsEliminados(ListaComentarioTransferencia.Select(x=>x.Id).ToArray())).ConfigureAwait(false);
            //await sat.EliminarActivosTransferencia().ConfigureAwait(false);


            return null;
        }

        private string[] IdsEliminados(string[] ids)
        {
            return ids;
        }

        #region Sin Implementar


        public Task<IPaginado<Transferencia>> ObtenerPaginadoAsync(Expression<Func<Transferencia, bool>> predicate = null, Func<IQueryable<Transferencia>, IOrderedQueryable<Transferencia>> orderBy = null, Func<IQueryable<Transferencia>, IIncludableQueryable<Transferencia, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }
        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Transferencia>> CrearAsync(params Transferencia[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Transferencia>> CrearAsync(IEnumerable<Transferencia> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }




        #endregion



    }
}
