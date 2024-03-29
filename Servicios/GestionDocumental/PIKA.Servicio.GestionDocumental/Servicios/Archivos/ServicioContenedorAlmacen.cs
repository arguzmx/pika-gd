﻿using DocumentFormat.OpenXml.Vml.Office;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Reportes.JSON;
using PIKA.Servicio.GestionDocumental.Data;
using PIKA.Servicio.GestionDocumental.Interfaces;
using PIKA.Servicio.Reportes.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioContenedorAlmacen : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioContenedorAlmacen
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<ContenedorAlmacen> repo;
        private IServicioReporteEntidad ServicioReporteEntidad;
        private IOptions<ConfiguracionServidor> Config;

        public ServicioContenedorAlmacen(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria, IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            IOptions<ConfiguracionServidor> Config,
            IServicioReporteEntidad ServicioReporteEntidad,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS)
        {
            this.Config = Config;
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<ContenedorAlmacen>());
        }

        public async Task<bool> Existe(Expression<Func<ContenedorAlmacen, bool>> predicado)
        {
            List<ContenedorAlmacen> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        private void RecalculaOcupacionPosicion(string UbicacionId )
        {
            var u = UDT.Context.PosicionesAlmacen.FirstOrDefault(x => x.Id == UbicacionId);
            if (u != null)
            {
                var l = UDT.Context.ContenedoresAlmacen.Where(x => x.PosicionAlmacenId == u.Id).ToList();
                decimal ocupacion = l.Count() * u.IncrementoContenedor;
                u.Ocupacion = ocupacion;
                UDT.Context.SaveChanges();
            }
        }

        public async Task<ContenedorAlmacen> CrearAsync(ContenedorAlmacen entity, CancellationToken cancellationToken = default)
        {

            seguridad.EstableceDatosProceso<ContenedorAlmacen>();

            ZonaAlmacen z = await this.UDT.Context.ZonasAlmacen.SingleOrDefaultAsync(x => x.Id == entity.ZonaAlmacenId);

            if(z==null)
            {
                throw new ExErrorRelacional("La zona de almacén no existe");
            }
            await seguridad.AccesoValidoZonaAlmacen(z);

            entity.AlmacenArchivoId = z.AlmacenArchivoId;
            entity.ArchivoId = z.ArchivoId;
            entity.ZonaAlmacenId = z.Id;
            await seguridad.AccesoValidoContenedorAlmacen(entity);

            if (await Existe(x => x.ZonaAlmacenId == z.Id &&
                x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = Guid.NewGuid().ToString();
            await repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            if (entity.PosicionAlmacenId != null)
            {
                RecalculaOcupacionPosicion(entity.PosicionAlmacenId);
            }

            return entity.Copia();
        }

        public async Task ActualizarAsync(ContenedorAlmacen entity)
        {
            seguridad.EstableceDatosProceso<ContenedorAlmacen>();
            await seguridad.AccesoValidoContenedorAlmacen(entity);

            ContenedorAlmacen o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }


            if(o.ZonaAlmacenId!= entity.ZonaAlmacenId)
            {
                ZonaAlmacen z = await this.UDT.Context.ZonasAlmacen.SingleOrDefaultAsync(x => x.Id == entity.ZonaAlmacenId);

                if (z == null)
                {
                    throw new ExErrorRelacional("La zona de almacén no existe");
                }
                await seguridad.AccesoValidoZonaAlmacen(z);
            }

            string original = o.Flat();


            string pAnterior = o.PosicionAlmacenId;
            string pActual = entity.PosicionAlmacenId;


            if (await Existe(x =>
            x.Id != entity.Id && x.ZonaAlmacenId == o.ZonaAlmacenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.CodigoBarras = entity.CodigoBarras;
            o.CodigoElectronico = entity.CodigoElectronico;
            o.Ocupacion  = entity.Ocupacion;
            o.ZonaAlmacenId = entity.ZonaAlmacenId;
            o.PosicionAlmacenId = entity.PosicionAlmacenId;
            

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));

            if (pActual != pAnterior)
            {
                if (pActual != null)
                {
                    RecalculaOcupacionPosicion(pActual);
                }

                if (pAnterior != null)
                {
                    RecalculaOcupacionPosicion(pAnterior);
                }
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



        public async Task<IPaginado<ContenedorAlmacen>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<ContenedorAlmacen>, IIncludableQueryable<ContenedorAlmacen, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<ContenedorAlmacen>();
            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "AlmacenArchivoId");
            if (f==null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var permitir =  await seguridad.AccesoCacheAlmacenArchivo(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }

        public Task<IEnumerable<ContenedorAlmacen>> CrearAsync(params ContenedorAlmacen[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ContenedorAlmacen>> CrearAsync(IEnumerable<ContenedorAlmacen> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<ContenedorAlmacen>();

            List<ContenedorAlmacen> listaEliminados = new List<ContenedorAlmacen>();
            foreach (var Id in ids)
            {
                ContenedorAlmacen o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    await seguridad.AccesoValidoContenedorAlmacen(o);

                    if ( UDT.Context.ActivoContenedorAlmacen.Any(x => x.ContenedorAlmacenId == o.Id))
                    {
                        throw new ExElementoExistente();
                    }

                    listaEliminados.Add(o);
                }               
            }


            if(listaEliminados.Count>0)
            {
                foreach(var c in listaEliminados)
                {
                    string pActual = c.PosicionAlmacenId;
                    UDT.Context.Entry(c).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(c.Id, c.Nombre);

                    if (pActual != null)
                    {
                        RecalculaOcupacionPosicion(pActual);
                    }

                }
                UDT.SaveChanges();
            }
            

            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public Task<List<ContenedorAlmacen>> ObtenerAsync(Expression<Func<ContenedorAlmacen, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<ContenedorAlmacen>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<ContenedorAlmacen>> ObtenerPaginadoAsync(Expression<Func<ContenedorAlmacen, bool>> predicate = null, Func<IQueryable<ContenedorAlmacen>, IOrderedQueryable<ContenedorAlmacen>> orderBy = null, Func<IQueryable<ContenedorAlmacen>, IIncludableQueryable<ContenedorAlmacen, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }



        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public async Task<ContenedorAlmacen> UnicoAsync(Expression<Func<ContenedorAlmacen, bool>> predicado = null, Func<IQueryable<ContenedorAlmacen>, IOrderedQueryable<ContenedorAlmacen>> ordenarPor = null, Func<IQueryable<ContenedorAlmacen>, IIncludableQueryable<ContenedorAlmacen, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            ContenedorAlmacen a = await this.repo.UnicoAsync(predicado);
            return a;
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<ContenedorAlmacen>();

            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }

            Query = GetDefaultQuery(Query);

            var f = Query.Filtros.FirstOrDefault(x => x.Propiedad == "AlmacenArchivoId");
            if (f == null)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*","*");
            }

            var permitir = await seguridad.AccesoCacheAlmacenArchivo(f.Valor);
            if (!permitir)
            {
                await seguridad.EmiteDatosSesionIncorrectos("*", "*");
            }

            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesPorId(List<string> Lista)
        {
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id.Trim()));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }


        #region reportes

        /// <summary>
        /// Crea el reporte de guía simple de archivo
        /// </summary>
        /// <param name="ArchivoId">Identificador del archivo para el reporte</param>
        /// <returns></returns>
        public async Task<byte[]> ReporteCaratulaContenedor(string Dominio, string UnidadOrganizacinal, string ContenedorAlmacenId)
        {
            seguridad.EstableceDatosProceso<ContenedorAlmacen>();

            var contenedor = UDT.Context.ContenedoresAlmacen.Find(ContenedorAlmacenId);
            await seguridad.AccesoValidoContenedorAlmacen(contenedor);

            var AlmacenArchivo = UDT.Context.AlmacenesArchivo.Find(contenedor.AlmacenArchivoId);
            var Archivo = UDT.Context.Archivos.Find(contenedor.ArchivoId);
            var Posicion = UDT.Context.PosicionesAlmacen.Find(contenedor.PosicionAlmacenId);
            var Zona = UDT.Context.ZonasAlmacen.Find(contenedor.ZonaAlmacenId);

            CaratulaContenedorAlmacen g = new CaratulaContenedorAlmacen
            {
                Dominio = Dominio,
                UnidadOrganizacional = UnidadOrganizacinal,
                AlmacenArchivo = AlmacenArchivo,
                Archivo = Archivo,
                Contenedor = contenedor,
                PosicionAlmacen = Posicion,
                ZonaAlmacen = Zona

            };

            var r = await ServicioReporteEntidad.UnicoAsync(x => x.Id == "caratula-cont-almacen");
            if (r == null) throw new EXNoEncontrado("reporte: caratula-cont-almacen");
            string jsonString = JsonConvert.SerializeObject(g);

            byte[] data = Convert.FromBase64String(r.Plantilla);

            return ReporteEntidades.ReportePlantilla(data, jsonString, Config.Value.ruta_cache_fisico, true);
        }

        public Task<ContenedorAlmacen> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}

