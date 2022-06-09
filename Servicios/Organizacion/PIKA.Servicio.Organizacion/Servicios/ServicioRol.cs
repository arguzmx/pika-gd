using Microsoft.AspNetCore.Razor.Language;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion.Interfaces;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion.Servicios
{
    public class ServicioRol : ContextoServicioOrganizacion,
        IServicioInyectable, IServicioRol
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<Rol> repo;
        private IRepositorioAsync<UsuariosRol> repoU;
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        
        public ServicioRol(
        IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
        ILogger<ServicioRol> Logger) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Rol>(
                new QueryComposer<Rol>());
            this.repoU = UDT.ObtenerRepositoryAsync<UsuariosRol>(
                new QueryComposer<UsuariosRol>());
        }


        public async Task<bool> Existe(Expression<Func<Rol, bool>> predicado)
        {
            List<Rol> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<Rol> CrearAsync(Rol entity, CancellationToken cancellationToken = default)
        {
            if (await Existe(x => x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();
            return entity;
        }


        public async Task ActualizarAsync(Rol entity)
        {
            Rol rol = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (rol == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id && x.Id == entity.Id
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            rol.Nombre = entity.Nombre;
            rol.Descripcion = entity.Descripcion;
            UDT.Context.Entry(rol).State = EntityState.Modified;
            UDT.SaveChanges();
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
        public async Task<IPaginado<Rol>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Rol>, IIncludableQueryable<Rol, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            Query= this.GetDefaultQuery(Query);
            var respuesta = await this.repo.ObtenerPaginadoAsync(Query);
            return respuesta;
        }

        

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            try
            {
                Rol r;
                ICollection<string> listaEliminados = new HashSet<string>();
                foreach (var Id in ids)
                {


                    r = await this.repo.UnicoAsync(x => x.Id == Id);

                    if (r != null)
                    {
                        UDT.Context.Entry(r).State = EntityState.Deleted;
                        listaEliminados.Add(r.Id);
                    }
                }
                UDT.SaveChanges();
                return listaEliminados;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
        }


        /// <summary>
        /// Vincula los identificadores de usuairo al rol
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ICollection<string>> Vincular(string rolId, string[] ids)
        {

            UsuariosRol r;
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids)
            {
                r = await this.repoU.UnicoAsync(x => x.RolId == rolId && x.ApplicationUserId == Id);
                if (r == null)
                {
                    await this.repoU.CrearAsync(new UsuariosRol() { ApplicationUserId = Id, RolId = rolId });
                }
                lista.Add(Id);
            }
            UDT.SaveChanges();
            return lista;
        }

        /// <summary>
        /// Vincula los identificadores de usuairo al rol
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        public async Task<ICollection<string>> Desvincular(string rolId, string[] ids)
        {

            UsuariosRol r;
            ICollection<string> lista = new HashSet<string>();
            foreach (var Id in ids)
            {
                r = await this.repoU.UnicoAsync(x => x.RolId == rolId && x.ApplicationUserId == Id);
                if (r != null)
                {
                    await this.repoU.Eliminar(new UsuariosRol() { ApplicationUserId = Id, RolId = rolId });
                    lista.Add(Id);
                }
            }
            UDT.SaveChanges();
            return lista;
        }

        /// <summary>
        ///  Obtiene los roles disponibles en el dominio
        /// </summary>
        /// <param name="idDominio"></param>
        /// <returns></returns>
        public async Task<ICollection<Rol>> ObtieneRoles(string idDominio)
        {
            return (await this.repo.ObtenerAsync(x => x.TipoOrigenId == ConstantesModelo.IDORIGEN_DOMINIO 
                 && x.OrigenId == idDominio))
                .OrderBy(x => x.Nombre).ToList();
        }

        public Task<List<Rol>> ObtenerAsync(Expression<Func<Rol, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<Rol>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Rol>> ObtenerPaginadoAsync(Expression<Func<Rol, bool>> predicate = null, Func<IQueryable<Rol>, IOrderedQueryable<Rol>> orderBy = null, Func<IQueryable<Rol>, IIncludableQueryable<Rol, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<Rol> UnicoAsync(Expression<Func<Rol, bool>> predicado = null, Func<IQueryable<Rol>, IOrderedQueryable<Rol>> ordenarPor = null, Func<IQueryable<Rol>, IIncludableQueryable<Rol, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            Rol r = await this.repo.UnicoAsync(predicado);
            return r.CopiaRol();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }
           
            Query = GetDefaultQuery(Query);
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
            var resultados = await this.repo.ObtenerAsync(x => Lista.Contains(x.Id));
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.Nombre
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }



        #region sin implemetar

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Rol>> CrearAsync(params Rol[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Rol>> CrearAsync(IEnumerable<Rol> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task EjecutarSql(string sqlCommand)
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
