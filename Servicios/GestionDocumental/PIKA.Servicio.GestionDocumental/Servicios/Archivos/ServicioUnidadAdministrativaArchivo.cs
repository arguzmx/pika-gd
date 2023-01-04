using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Data;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.GestorDocumental.Seguridad;
using LazyCache;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using Newtonsoft.Json;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public class ServicioUnidadAdministrativaArchivo : ContextoServicioGestionDocumental,
        IServicioInyectable, IServicioUnidadAdministrativaArchivo
    {
        private const string DEFAULT_SORT_COL = "UnidadAdministrativa";
        private const string DEFAULT_SORT_DIRECTION = "asc";

        private IRepositorioAsync<UnidadAdministrativaArchivo> repo;
        public ServicioUnidadAdministrativaArchivo(
            IAppCache cache,
            IRegistroAuditoria registroAuditoria,
            IProveedorOpcionesContexto<DBContextGestionDocumental> proveedorOpciones,
            ILogger<ServicioLog> Logger) : base(registroAuditoria, proveedorOpciones, Logger,
                cache, ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_UNIDADESADMIN)
        {
            this.repo = UDT.ObtenerRepositoryAsync(new QueryComposer<UnidadAdministrativaArchivo>());
        }



        public async Task<bool> Existe(Expression<Func<UnidadAdministrativaArchivo, bool>> predicado)
        {
            List<UnidadAdministrativaArchivo> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }

  

        public async Task<UnidadAdministrativaArchivo> CrearAsync(UnidadAdministrativaArchivo entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<UnidadAdministrativaArchivo>();
            await seguridad.IdEnUnidadOrg(entity.OrigenId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoConcentracionId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoHistoricoId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoTramiteId);


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

            await seguridad.RegistraEventoCrear(entity.Id, "*");

            return entity.Copia();
        }

        public async Task ActualizarAsync(UnidadAdministrativaArchivo entity)
        {
            seguridad.EstableceDatosProceso<UnidadAdministrativaArchivo>();
            await seguridad.IdEnUnidadOrg(entity.OrigenId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoConcentracionId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoHistoricoId);
            await seguridad.AccesoValidoArchivo(entity.ArchivoTramiteId);

            UnidadAdministrativaArchivo o = await this.repo.UnicoAsync(x => x.Id == entity.Id);

            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            await seguridad.IdEnUnidadOrg(o.OrigenId);

            string original = o.Flat();

            if (await Existe(x => x.UnidadAdministrativa.Equals(entity.UnidadAdministrativa, StringComparison.InvariantCultureIgnoreCase)
               && x.Id != entity.Id && x.OrigenId.Equals(entity.OrigenId, StringComparison.InvariantCultureIgnoreCase)
               ))
            {
                throw new ExElementoExistente(entity.UnidadAdministrativa);
            }

            o = entity.Copia();

            UDT.Context.Entry(o).State = EntityState.Modified;
            UDT.SaveChanges();

            await seguridad.RegistraEventoActualizar(o.Id, "*", original.JsonDiff(o.Flat()));
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
            query.Filtros.Add(new FiltroConsulta()
            {
                Negacion = false,
                NivelFuzzy = 0,
                Operador = FiltroConsulta.OP_EQ,
                Propiedad = "OrigenId",
                Valor = RegistroActividad.UnidadOrgId,
                ValorString = RegistroActividad.UnidadOrgId
            });

            return query;
        }

        public async Task<IPaginado<UnidadAdministrativaArchivo>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<UnidadAdministrativaArchivo>, IIncludableQueryable<UnidadAdministrativaArchivo, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<UnidadAdministrativaArchivo>();
            Query = GetDefaultQuery(Query);

            long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
            string sqls = @$"SELECT  COUNT(*) FROM {DBContextGestionDocumental.TablaUnidadAdministrativaArchivo} where  (1=1) and OrigenId='{RegistroActividad.UnidadOrgId}' ";
            string sqlsCount = "";
            int total = 0;
            string queryNoAdmin = "";
            bool esAdmin = false;

            var acceso = usuario.Accesos.Where(x => x.OU == RegistroActividad.UnidadOrgId).FirstOrDefault();
            if (acceso != null)
            {
                esAdmin = acceso.Admin;
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

        public async Task<ICollection<string>> Eliminar(string[] ids)
        {
            seguridad.EstableceDatosProceso<UnidadAdministrativaArchivo>();
            List<UnidadAdministrativaArchivo> listaEliminados = new List<UnidadAdministrativaArchivo>();
            foreach (var Id in ids)
            {
                UnidadAdministrativaArchivo c = await this.repo.UnicoAsync(x => x.Id == Id);
                if (c != null)
                {
                    await seguridad.IdEnUnidadOrg(c.OrigenId);


                    if (UDT.Context.Activos.Any(x=>x.UnidadAdministrativaArchivoId == c.Id))
                    {
                        throw new ExElementoExistente();
                    }

                    listaEliminados.Add(c);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach(var c in listaEliminados)
                {
                    UDT.Context.Entry(c).State = EntityState.Deleted;
                    await seguridad.RegistraEventoEliminar(c.Id, "*");
                }
                UDT.SaveChanges();
            }

            return listaEliminados.Select(x => x.Id).ToList(); ;
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
            seguridad.EstableceDatosProceso<UnidadAdministrativaArchivo>();
            Query = GetDefaultQuery(Query);

            var qarchivo = Query.Filtros.FirstOrDefault(x=>x.Propiedad == "ArchivoId");
            

            long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
            string sqls = @$"SELECT  COUNT(*) FROM {DBContextGestionDocumental.TablaUnidadAdministrativaArchivo} where  (1=1) and OrigenId='%TENANTID%' 
and (ArchivoConcentracionId='{qarchivo.Valor}' OR ArchivoHistoricoId='{qarchivo.Valor}' or ArchivoTramiteId='{qarchivo.Valor}')";

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


        #region Sin Implementación

        public Task<PermisoUnidadAdministrativaArchivo> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {
            throw new NotImplementedException();
        }
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
