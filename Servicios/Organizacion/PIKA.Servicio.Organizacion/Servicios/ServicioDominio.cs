using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Organizacion;
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
        private UnidadDeTrabajo<DbContextOrganizacion> UDT;
        public ServicioDominio(
            IProveedorOpcionesContexto<DbContextOrganizacion> proveedorOpciones,
            ILogger<ServicioDominio> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextOrganizacion>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Dominio>(
                new QueryComposer<Dominio>());
            this.repoOU = UDT.ObtenerRepositoryAsync<UnidadOrganizacional>(
                new QueryComposer<UnidadOrganizacional>());
        }


        public async Task<bool> Existe(Expression<Func<Dominio, bool>> predicado)
        {
            List<Dominio> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<Dominio> CrearAsync(Dominio entity, CancellationToken cancellationToken = default)
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

        public async Task ActualizarAsync(Dominio entity)
        {

            Dominio o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            if (await Existe(x =>
            x.Id != entity.Id & x.TipoOrigenId == entity.TipoOrigenId && x.OrigenId == entity.OrigenId
            && x.Nombre.Equals(entity.Nombre, StringComparison.InvariantCultureIgnoreCase)))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            UDT.Context.Entry(o).State = EntityState.Modified;
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            Dominio d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {


                    d.Eliminada = true;
                    // actualiza las unidads organizacionales para ser marcadas como eliminadas 
                    var ous = await repoOU.ObtenerAsync(x => x.DominioId == Id);

#if DEBUG
                    logger.LogDebug("Marcando Dominio {id} como eliminada ", Id);
                    logger.LogDebug("Unidades Organizacionales dependientes {0}", ous.Count);
#endif


                    foreach (var ou in ous)
                    {
#if DEBUG
                        logger.LogDebug("Marcando OU {id} como eliminada ", ou.Id);
#endif
                        ou.Eliminada = true;
                        UDT.Context.Entry(ou).State = EntityState.Modified;
                    }

                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
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
            Dominio d;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                d = await this.repo.UnicoAsync(x => x.Id == Id);
                if (d != null)
                {
                    d.Eliminada = false;

                    // actualiza las unidads organizacionales para ser marcadas como eliminadas 
                    var ous = await repoOU.ObtenerAsync(x => x.DominioId == Id);

#if DEBUG
                    logger.LogDebug("Marcando Dominio {id} como restaurado", Id);
                    logger.LogDebug("Unidades Organizacionales dependientes {0}", ous.Count);
#endif

                    foreach (var ou in ous)
                    {
#if DEBUG
                        logger.LogDebug("Marcando Unidad Organizacional {id} como restaurado", Id);
#endif
                        ou.Eliminada = false;
                        UDT.Context.Entry(ou).State = EntityState.Modified;
                    }

                    UDT.Context.Entry(d).State = EntityState.Modified;
                    listaEliminados.Add(d.Id);
                }
            }
            UDT.SaveChanges();


            return listaEliminados;
        }




        public async Task<Dominio> UnicoAsync(Expression<Func<Dominio, bool>> predicado = null, Func<IQueryable<Dominio>, IOrderedQueryable<Dominio>> ordenarPor = null, Func<IQueryable<Dominio>, IIncludableQueryable<Dominio, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            Dominio d = await this.repo.UnicoAsync(predicado);

            //Cuando llamas a serializar objetos con propiedades de navegación pueden crearse referencias circulares
            //por eso devolvemos una instancia filtrada
            //Hayq que implmenbatrlo como un método de extensión en ExtensionesDominio
            //estudiar https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/extension-methods

            return d.CopiaDominio();
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
        #endregion


    }



}
