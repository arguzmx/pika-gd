using LazyCache;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Organizacion;
using PIKA.Modelo.Organizacion.Estructura;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;


namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioDominio : ContextoServicioOrganizacion,
        IServicioInyectable, IServicioDominio
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Dominio> repo;
        private IRepositorioAsync<UnidadOrganizacional> repoOU;

        public ServicioDominio(
         IAppCache cache,
         IRegistroAuditoria registroAuditoria,
         IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
                  ILogger<ServicioLog> Logger
         ) : base(registroAuditoria, proveedorOpciones, Logger,
                 cache, ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO)
        {
            this.repo = UDT.ObtenerRepositoryAsync<Dominio>(
                new QueryComposer<Dominio>());
            this.repoOU = UDT.ObtenerRepositoryAsync<UnidadOrganizacional>(
                new QueryComposer<UnidadOrganizacional>());
        }


        public async Task<ActDominioOU> OntieneDominioOU(string DominioId, string OUId)
        {
            seguridad.EstableceDatosProceso<Dominio>();
            var d = await this.UDT.Context.Dominios.Where(x => x.Id == RegistroActividad.DominioId).SingleOrDefaultAsync();
            var ou = await this.UDT.Context.UnidadesOrganizacionales.Where(x => x.Id == RegistroActividad.UnidadOrgId).SingleOrDefaultAsync();

            if (d != null && ou != null)
            {
                return new ActDominioOU() { Dominio = d.Nombre, OU = ou.Nombre };
            }

            return null;
        }

        public async Task<bool> ActualizaDominioOU(ActDominioOU request, string DominioId, string OUId )
        {
            seguridad.EstableceDatosProceso<Dominio>();

            var d = await this.UDT.Context.Dominios.Where(x => x.Id == RegistroActividad.DominioId).SingleOrDefaultAsync();
            var ou = await this.UDT.Context.UnidadesOrganizacionales.Where(x => x.Id == RegistroActividad.UnidadOrgId).SingleOrDefaultAsync();

            if(d!=null && ou != null)
            {
                if(!string.IsNullOrWhiteSpace(request.Dominio) && !string.IsNullOrWhiteSpace(request.OU))
                {
                    d.Nombre = request.Dominio;
                    ou.Nombre = request.OU;
                    await this.UDT.Context.SaveChangesAsync();
                    await seguridad.RegistraEvento(AplicacionOrganizacion.EventosAdicionales.ActualizaDatosOrganizacion.GetHashCode(), true, JsonConvert.SerializeObject(request));
                    return true;
                }
            }
            return false;
        }

        public async Task<bool> Existe(Expression<Func<Dominio, bool>> predicado)
        {
            List<Dominio> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Dominio> CrearAsync(Dominio entity, CancellationToken cancellationToken = default)
        {
            if(!RegistroActividad.Claims.Any(c=>c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }

        public async Task ActualizarAsync(Dominio entity)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            Dominio o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.TipoOrigenId == entity.TipoOrigenId && x.OrigenId == entity.OrigenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }
            o.Eliminada = entity.Eliminada;
            o.Nombre = entity.Nombre;
            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await MarcaOUDelDominio(o.Id, entity.Eliminada);

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
        public async Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query = GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query, null);

            return respuesta;
        }


        public async Task EjecutarSql(string sqlCommand)
        {
            await this.contexto.Database.ExecuteSqlRawAsync(sqlCommand);
        }

        public async Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            foreach (string s in sqlCommand)
            {
                await this.contexto.Database.ExecuteSqlRawAsync(s);
            }
        }


        private async Task MarcaOUDelDominio(string Id, bool eliminada)
        {
            // actualiza las unidads organizacionales para ser marcadas como eliminadas 
            var ous = await repoOU.ObtenerAsync(x => x.DominioId == Id);
             foreach (var ou in ous)
            {
                ou.Eliminada = true;
                UDT.Context.Entry(ou).State = EntityState.Modified;
            }
            UDT.SaveChanges();
        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            Dominio o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                o = await this.repo.UnicoAsync(x => x.Id == Id.Trim());
                if (o != null)
                {
                    try
                    {
                        o = await this.repo.UnicoAsync(x => x.Id == Id);
                        if (o != null)
                        {
                            await this.repo.Eliminar(o);
                        }
                        this.UDT.SaveChanges();
                        listaEliminados.Add(o.Id);
                    }
                    catch (DbUpdateException)
                    {
                        throw new ExErrorRelacional(Id);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }

        public Task<List<Dominio>> ObtenerAsync(Expression<Func<Dominio, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<Dominio>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }
            Dominio d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = false;
                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                    await MarcaOUDelDominio(Id, false);
                }
            }
            UDT.SaveChanges();


            return listaEliminados;
        }




        public async Task<Dominio> UnicoAsync(Expression<Func<Dominio, bool>> predicado = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> ordenarPor = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Dominio d = await this.repo.UnicoAsync(predicado);

            return d.CopiaDominio();
        }


        public async Task<string[]> Purgar()
        {
            if (!RegistroActividad.Claims.Any(c => c.Subject.Equals("administracion")))
            {
                await seguridad.EmiteDatosSesionIncorrectos();
            }

            List<Dominio> ListaDominio = await this.repo.ObtenerAsync(x=>x.Eliminada==true).ConfigureAwait(false);
            if (ListaDominio.Count() > 0)
            {
                List<UnidadOrganizacional> ListaUnidad = await this.repoOU.ObtenerAsync(x=>x.DominioId.Contains(ListaDominio.Select(x=>x.Id).FirstOrDefault())).ConfigureAwait(false);
                await this.repoOU.EliminarRango(ListaUnidad).ConfigureAwait(false);
                 this.UDT.SaveChanges();
                await this.repo.EliminarRango(ListaDominio).ConfigureAwait(false);
                this.UDT.SaveChanges();


            }
            throw new NotImplementedException();
        }

        #region Sin implementar

        public Task<IEnumerable<Dominio>> CrearAsync(params Dominio[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Dominio>> CrearAsync(IEnumerable<Dominio> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Dominio>> ObtenerPaginadoAsync(Expression<Func<Dominio, bool>> predicate = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> orderBy = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<Dominio> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }


        #endregion


    }



}
