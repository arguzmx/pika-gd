using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestrctura.Reportes;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
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
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental.Seguridad;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioUnidadAdministrativaArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioUnidadAdministrativaArchivo
    {
        private const string DEFAULT_SORT_COL = "UnidadAdministrativa";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<UnidadAdministrativaArchivo> repo;
        private IOptions<ConfiguracionServidor> Config;
        private IServicioReporteEntidad ServicioReporteEntidad;

        private UnidadDeTrabajo<DBContextGestionDocumental> UDT;

        public UsuarioAPI usuario { get; set; }
        public PermisoAplicacion permisos { get ; set ; }
        public ContextoRegistroActividad RegistroActividad { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public ServicioUnidadAdministrativaArchivo(
            IRegistroAuditoria registroAuditoria,
            IServicioReporteEntidad ServicioReporteEntidad,
            IOptions<ConfiguracionServidor> Config,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger)
        {
            this.ServicioReporteEntidad = ServicioReporteEntidad;
            this.Config = Config;
            this.UDT = new UnidadDeTrabajo<DBContextGestionDocumental>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<UnidadAdministrativaArchivo>(new QueryComposer<UnidadAdministrativaArchivo>());
        }

        public Task<PermisoUnidadAdministrativaArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> Existe(Expression<Func<UnidadAdministrativaArchivo, bool>> predicado)
        {
            List<UnidadAdministrativaArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

        public async Task<UnidadAdministrativaArchivo> CrearAsync(UnidadAdministrativaArchivo entity, CancellationToken cancellationToken = default)
        {

            if (await Existe(x=>x.UnidadAdministrativa.Equals(entity.UnidadAdministrativa, StringComparison.InvariantCultureIgnoreCase)
            &&  x.OrigenId.Equals(entity.OrigenId,StringComparison.InvariantCultureIgnoreCase)
            ))
            {
                throw new ExElementoExistente(entity.UnidadAdministrativa);
            }

            entity.UnidadAdministrativa = entity.UnidadAdministrativa.Trim();
            entity.Id = System.Guid.NewGuid().ToString();
            await this.repo.CrearAsync(entity);
            UDT.SaveChanges();

            return entity.Copia();
        }

        public async Task ActualizarAsync(UnidadAdministrativaArchivo entity)
        {
            try
            {
                UnidadAdministrativaArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

                if (o == null)
                {
                    throw new EXNoEncontrado(entity.Id);
                }

                if (await Existe(x => x.UnidadAdministrativa.Equals(entity.UnidadAdministrativa, StringComparison.InvariantCultureIgnoreCase)
                   && x.Id != entity.Id && x.OrigenId.Equals(entity.OrigenId, StringComparison.InvariantCultureIgnoreCase)
                   ))
                {
                    throw new ExElementoExistente(entity.UnidadAdministrativa);
                }

                o = entity.Copia();

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

        public async Task<IPaginado<UnidadAdministrativaArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UnidadAdministrativaArchivo>, IIncludableQueryable<UnidadAdministrativaArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {

            try
            {
                Query = GetDefaultQuery(Query);

                long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
                string sqls = @$"SELECT  COUNT(*) FROM {DBContextGestionDocumental.TablaUnidadAdministrativaArchivo} where  (1=1) and OrigenId='%TENANTID%' ";
                string sqlsCount = "";
                int total = 0;
                string queryNoAdmin = "";
                bool esAdmin = false;

                var FOU = Query.Filtros.Where(x => x.Propiedad == "TenantId").SingleOrDefault();
                if (FOU != null)
                {
                    sqls = sqls.Replace("%TENANTID%", FOU.Valor);
                    var acceso = usuario.Accesos.Where(x => x.OU == FOU.Valor).FirstOrDefault();
                    if (acceso != null)
                    {
                        esAdmin = acceso.Admin;
                    }
                } else
                {
                    sqls = sqls.Replace("%TENANTID%", "");
                }

                if (usuario.AdminGlobal  || (this.permisos != null && this.permisos.Admin))
                {
                    esAdmin = true;
                }

                if (!esAdmin)
                {
                    string roles = "";
                    usuario.Roles.ForEach(r =>
                    {
                        roles += $"'{r}',";
                    });
                    roles = roles.TrimEnd(',');

                    // en caso de que el usuario no sea administrador y no tengo roles esto bloqueará la llamada
                    if (string.IsNullOrEmpty(roles)) roles = "empty";
                    queryNoAdmin = @$" and (Id in (select UnidadAdministrativaArchivoId  from {DBContextGestionDocumental.TablaPermisoUnidadAdministrativaArchivo} where DestinatarioId in ({roles})))";
                }



                if (Query.Filtros != null)
                {
                    List<string> condiciones = MySQLQueryComposer.Condiciones<UnidadAdministrativaArchivo>(Query, "");
                    foreach (string s in condiciones)
                    {
                        sqls += $" and ({s})";
                    }
                }
                sqls += $" {queryNoAdmin}";

                // Consulta de conteo sin ordenamiento ni limites
                sqlsCount = sqls;
                // Console.WriteLine(sqlsCount);

                sqls += $" order by {Query.ord_columna} {Query.ord_direccion} ";
                sqls += $" LIMIT {offset},{Query.tamano}";

                sqls = sqls.Replace("COUNT(*)", "DISTINCT *");
                
                // Console.WriteLine(sqls);

                using (var command = UDT.Context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = sqlsCount;
                    UDT.Context.Database.OpenConnection();
                    using (var result = await command.ExecuteReaderAsync())
                    {
                        if (result.Read())
                        {
                            total = result.GetInt32(0);
                        }
                    }
                }


                Paginado<UnidadAdministrativaArchivo> respuesta = new Paginado<UnidadAdministrativaArchivo>()
                {
                    ConteoFiltrado = total,
                    ConteoTotal = total,
                    Desde = Query.indice * Query.tamano,
                    Indice = Query.indice,
                    Tamano = Query.tamano,
                    Paginas = (int)Math.Ceiling(total / (double)Query.tamano)
                };
                respuesta.Elementos = this.UDT.Context.UnidadesAdministrativasArchivo.FromSqlRaw(sqls).ToList();

                return respuesta;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }


        }

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            UnidadAdministrativaArchivo c;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    try
                    {
                        UDT.Context.Entry(c).State = EntityState.Deleted;
                        listaEliminados.Add(c.Id);
                    }
                    catch (Exception)
                    { }
                }
            }
            UDT.SaveChanges();

            return listaEliminados;

        }
        public Task<List<UnidadAdministrativaArchivo>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }
        public Task<List<UnidadAdministrativaArchivo>> ObtenerAsync(Expression<Func<UnidadAdministrativaArchivo, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public async Task<UnidadAdministrativaArchivo> UnicoAsync(Expression<Func<UnidadAdministrativaArchivo, bool>> predicado = null, Func<IQueryable<UnidadAdministrativaArchivo>, IOrderedQueryable<UnidadAdministrativaArchivo>> ordenarPor = null, Func<IQueryable<UnidadAdministrativaArchivo>, IIncludableQueryable<UnidadAdministrativaArchivo, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            UnidadAdministrativaArchivo a = await this.repo.UnicoAsync(predicado);
            return a.Copia();
        }

        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            Query = GetDefaultQuery(Query);

            long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
            string sqls = @$"SELECT  COUNT(*) FROM {DBContextGestionDocumental.TablaUnidadAdministrativaArchivo} where  (1=1) and OrigenId='%TENANTID%' ";

            string queryNoAdmin = "";
            bool esAdmin = false;

            var FOU = Query.Filtros.Where(x => x.Propiedad == "TenantId").SingleOrDefault();
            if (FOU != null)
            {
                sqls = sqls.Replace("%TENANTID%", FOU.Valor);
                var acceso = usuario.Accesos.Where(x => x.OU == FOU.Valor).FirstOrDefault();
                if (acceso != null)
                {
                    esAdmin = acceso.Admin;
                }
            }
            else
            {
                sqls = sqls.Replace("%TENANTID%", "");
            }

            if (usuario.AdminGlobal || (this.permisos != null && this.permisos.Admin))
            {
                esAdmin = true;
            }

            if (!esAdmin)
            {
                string roles = "";
                usuario.Roles.ForEach(r =>
                {
                    roles += $"'{r}',";
                });
                roles = roles.TrimEnd(',');

                // en caso de que el usuario no sea administrador y no tengo roles esto bloqueará la llamada
                if (string.IsNullOrEmpty(roles)) roles = "empty";
                queryNoAdmin = @$" and (Id in (select UnidadAdministrativaArchivoId  from {DBContextGestionDocumental.TablaPermisoUnidadAdministrativaArchivo} where DestinatarioId in ({roles})))";
            }



            if (Query.Filtros != null)
            {
                List<string> condiciones = MySQLQueryComposer.Condiciones<UnidadAdministrativaArchivo>(Query, "");
                foreach (string s in condiciones)
                {
                    sqls += $" and ({s})";
                }
            }
            sqls += $" {queryNoAdmin}";
            sqls += $" order by {Query.ord_columna} {Query.ord_direccion} ";
            sqls = sqls.Replace("COUNT(*)", "DISTINCT *");

            var resultados = this.UDT.Context.UnidadesAdministrativasArchivo.FromSqlRaw(sqls).ToList();
            List<ValorListaOrdenada> l = resultados.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.UnidadAdministrativa
            }).ToList();


            return l.OrderBy(x => x.Texto).ToList();
        }

 
        public async Task<List<ValorListaOrdenada>> ObtenerParesAsyncOld(Consulta Query)
        {
            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "UnidadAdministrativa";
                }
            }
            
            
            Query = GetDefaultQuery(Query);
            var resultados = await this.repo.ObtenerPaginadoAsync(Query);
            List<ValorListaOrdenada> l = resultados.Elementos.Select(x => new ValorListaOrdenada()
            {
                Id = x.Id,
                Indice = 0,
                Texto = x.UnidadAdministrativa
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
                Texto = x.UnidadAdministrativa
            }).ToList();

            return l.OrderBy(x => x.Texto).ToList();
        }

        private string[] ListaIdEliminar(string[]ids) 
        {
            return ids;
        }
        private async Task<ICollection<string>> EliminarArchivo(string[] ids)
        {
            UnidadAdministrativaArchivo o;
            ICollection<string> listaEliminados = new HashSet<string>();
            foreach (var Id in ids)
            {
                try
                {
                    o = await this.repo.UnicoAsync(x => x.Id == Id);
                    if (o != null)
                    {
                        await this.repo.Eliminar(o);
                        listaEliminados.Add(o.Id);
                    }
                    this.UDT.SaveChanges();

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
            UDT.SaveChanges();

            return listaEliminados;

        }

        #region Sin Implementación
        public Task<IPaginado<UnidadAdministrativaArchivo>> ObtenerPaginadoAsync(Expression<Func<UnidadAdministrativaArchivo, bool>> predicate = null, Func<IQueryable<UnidadAdministrativaArchivo>, IOrderedQueryable<UnidadAdministrativaArchivo>> orderBy = null, Func<IQueryable<UnidadAdministrativaArchivo>, IIncludableQueryable<UnidadAdministrativaArchivo, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        
        public Task EjecutarSqlBatch(List<string> sqlCommand)
        {
            throw new NotImplementedException();
        }
        public Task<IEnumerable<UnidadAdministrativaArchivo>> CrearAsync(params UnidadAdministrativaArchivo[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UnidadAdministrativaArchivo>> CrearAsync(IEnumerable<UnidadAdministrativaArchivo> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task EjecutarSql(string sqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public void EstableceContextoSeguridad(UsuarioAPI usuario, ContextoRegistroActividad RegistroActividad)
        {
            throw new NotImplementedException();
        }

        #endregion

    }
}
