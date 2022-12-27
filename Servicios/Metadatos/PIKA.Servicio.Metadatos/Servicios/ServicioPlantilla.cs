using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Nest;
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Metadatos;
using PIKA.Servicio.Metadatos.Data;
using PIKA.Servicio.Metadatos.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.Servicios
{
    public class ServicioPlantilla : ContextoServicioMetadatos,IServicioInyectable, IServicioPlantilla
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<Plantilla> repo;


        public ServicioPlantilla(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextMetadatos> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS)
        {
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<Plantilla>());
        }

        public async Task<bool> Existe(Expression<Func<Plantilla, bool>> predicado)
        {
            List<Plantilla> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Plantilla> CrearAsync(Plantilla entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            if(!await seguridad.IdEnDominio(entity.OrigenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }


            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada == false & x.OrigenId == entity.OrigenId && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            entity.Eliminada = false;
            entity.AlmacenDatosId = "default";
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }

        public async Task ActualizarAsync(Plantilla entity)
        {
            Plantilla o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            seguridad.EstableceDatosProceso<Plantilla>();
            if (!await seguridad.IdEnDominio(o.OrigenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }


            if (await Existe(x => x.Id != entity.Id && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)
            && x.Eliminada == false & x.OrigenId == entity.OrigenId && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            string original = o.Flat();
            o.Nombre = entity.Nombre.Trim();
            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();
            
            await seguridad.RegistraEventoActualizar(o.Id, o.Nombre, original.JsonDiff(o.Flat()));
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

            query.Filtros.RemoveAll(x => x.Propiedad == "OrigenId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "OrigenId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.DominioId });


            return query;
        }
        public async Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);
            return respuesta;
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            List<Plantilla> listaEliminados = new List<Plantilla>();
            foreach (var Id in ids)
            {
                Plantilla p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {
                 
                    if(!await seguridad.IdEnDominio(p.OrigenId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(p.Id, p.Nombre);
                    }

                    listaEliminados.Add(p);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var p in listaEliminados)
                {
                    p.Eliminada = true;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                    await seguridad.RegistraEventoEliminar(p.Id, p.Nombre);
                }

                UDT.SaveChanges();
            }
            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public async Task<Plantilla> UnicoAsync(Expression<Func<Plantilla, bool>> predicado = null, 
            Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> ordenarPor = null, 
            Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> incluir = null, 
            bool inhabilitarSegumiento = true)
        {

            Plantilla d = await this.repo.UnicoAsync(predicado,ordenarPor, incluir);

            if (!await seguridad.IdEnDominio(d.OrigenId))
            {
                await seguridad.EmiteDatosSesionIncorrectos(d.Id, d.Nombre);
            }
            return d.Copia();
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            List<Plantilla> listaEliminados = new List<Plantilla>();
            foreach (var Id in ids)
            {
                Plantilla p = await this.repo.UnicoAsync(x => x.Id == Id);
                if (p != null)
                {

                    if (!await seguridad.IdEnDominio(p.OrigenId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(p.Id, p.Nombre);
                    }

                    listaEliminados.Add(p);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var p in listaEliminados)
                {
                    string original = p.Flat();
                    p.Eliminada = false;
                    UDT.Context.Entry(p).State = EntityState.Modified;
                    await seguridad.RegistraEventoActualizar(p.Id, p.Nombre, original.JsonDiff(p.Flat()));
                }

                UDT.SaveChanges();
            }
            return listaEliminados.Select(x => x.Id).ToList();
        }
        public Task<List<Plantilla>> ObtenerAsync(string SqlCommand)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public  Task<List<Plantilla>> ObtenerAsync(Expression<Func<Plantilla, bool>> predicado)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<List<ValorListaPlantilla>> ObtenerValores(string PropiedadId)
        {
            seguridad.EstableceDatosProceso<Plantilla>();
            PropiedadPlantilla pp = UDT.Context.PropiedadPlantilla.Where(x=>x.Id == PropiedadId).FirstOrDefault();
            if (pp != null)
            {
                Plantilla p = await this.repo.UnicoAsync(x => x.Id == pp.PlantillaId);
                if (p != null)
                {
                    if (!await seguridad.IdEnDominio(p.OrigenId))
                    {
                        await seguridad.EmiteDatosSesionIncorrectos(p.Id, p.Nombre);
                    }
                    var lista = await this.UDT.Context.ValoresListaPropiedad.Where(x => x.PropiedadId == PropiedadId).ToListAsync();
                    return lista;
                }
            }

            return new List<ValorListaPlantilla>();
           
        }

        #region Sin Implementar
        public Task<IEnumerable<Plantilla>> CrearAsync(params Plantilla[] entities)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<Plantilla>> CrearAsync(IEnumerable<Plantilla> entities, CancellationToken cancellationToken = default)
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
       
        
        public Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Expression<Func<Plantilla, bool>> predicate = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> orderBy = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task Purgar()
        {
            throw new NotImplementedException();
        }

        public Task<Plantilla> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion


    }
}
