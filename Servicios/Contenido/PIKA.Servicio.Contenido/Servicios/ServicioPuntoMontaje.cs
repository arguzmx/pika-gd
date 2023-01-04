using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using LazyCache;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Logging;
using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Excepciones;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Servicios
{
   public class ServicioPuntoMontaje : ContextoServicioContenido,
        IServicioInyectable, IServicioPuntoMontaje
    {
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        private IRepositorioAsync<PuntoMontaje> repo;
        private IRepositorioAsync<VolumenPuntoMontaje> repoVPM;

        public ServicioPuntoMontaje(
            IRegistroAuditoria registroAuditoria,
            IAppCache cache,
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(registroAuditoria, proveedorOpciones, Logger,
            cache, ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION)
        {
            this.repo = UDT.ObtenerRepositoryAsync<PuntoMontaje>(new QueryComposer<PuntoMontaje>());
            this.repoVPM = UDT.ObtenerRepositoryAsync<VolumenPuntoMontaje>(new QueryComposer<VolumenPuntoMontaje>());
        }


        public async Task<PermisosPuntoMontaje> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId)
        {

            bool esAdminOU = false;
            var acceso = usuario.Accesos.Where(x => x.OU == UnidaddOrganizacionalId).FirstOrDefault();
            if (acceso != null)
            {
                esAdminOU = acceso.Admin;
            }

            PermisosPuntoMontaje p = new PermisosPuntoMontaje()
            {
                Crear = false,
                Actualizar = false,
                Elminar = false,
                Leer = false,
                GestionContenido = false,
                GestionMetadatos = false,
                PuntoMontajeId = EntidadId
            };

            if (usuario.AdminGlobal || esAdminOU)
            {
                p.Actualizar = true;
                p.Elminar = true;
                p.Leer = true;
                p.GestionContenido = true;
                p.GestionMetadatos = true;
                p.Crear = true;

            }
            else
            {
                foreach (var r in usuario.Roles)
                {
                    var pm = await this.UDT.Context.PermisosPuntoMontaje.Where(x => x.DestinatarioId == r && x.PuntoMontajeId == EntidadId).SingleOrDefaultAsync();
                    if (pm != null)
                    {
                        p.Actualizar = p.Actualizar || pm.Actualizar;
                        p.Elminar = p.Elminar || pm.Elminar;
                        p.Leer = p.Leer || pm.Leer;
                        p.GestionContenido = p.GestionContenido || pm.GestionContenido;
                        p.GestionMetadatos = p.GestionMetadatos || pm.GestionMetadatos;
                        p.Crear = p.Crear || pm.Crear;
                    }
                }
            }

            return p;
        }

        public async Task<bool> Existe(Expression<Func<PuntoMontaje, bool>> predicado)
        {
            List<PuntoMontaje> l = await this.repo.ObtenerAsync(predicado);
            if (l.Count() == 0) return false;
            return true;
        }


        public async Task<PuntoMontaje> CrearAsync(PuntoMontaje entity, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PuntoMontaje>();
            await seguridad.IdEnUnidadOrg(entity.OrigenId);
            if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
            {
                await seguridad.AccesoCacheVolumen(entity.VolumenDefaultId);
            }

            if (await Existe(x=>x.Nombre == entity.Nombre 
            && x.OrigenId  == entity.OrigenId 
            && x.TipoOrigenId == entity.TipoOrigenId))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            entity.Id = System.Guid.NewGuid().ToString();
            entity.FechaCreacion = DateTime.UtcNow;
            entity.Eliminada = false;

            if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
            {
                VolumenPuntoMontaje vpm = new VolumenPuntoMontaje()
                {
                    PuntoMontajeId = entity.Id,
                    VolumenId = entity.VolumenDefaultId
                };
                this.UDT.Context.VolumenPuntosMontaje.Add(vpm);
            }

            this.UDT.Context.PuntosMontaje.Add(entity);
            UDT.SaveChanges();

            await seguridad.RegistraEventoCrear(entity.Id, entity.Nombre);

            return entity.Copia();
        }


        public async Task ActualizarAsync(PuntoMontaje entity)
        {
            seguridad.EstableceDatosProceso<PuntoMontaje>();

            PuntoMontaje o = await this.repo.UnicoAsync(x => x.Id == entity.Id);
            
            if (o == null)
            {
                throw new EXNoEncontrado(entity.Id);
            }

            string original = o.Flat();
            await seguridad.IdEnUnidadOrg(entity.OrigenId);
            if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
            {
                await seguridad.AccesoCacheVolumen(entity.VolumenDefaultId);
            }


            if (await Existe(x => x.Nombre == entity.Nombre && x.OrigenId == o.OrigenId
           && x.TipoOrigenId == o.TipoOrigenId && x.Id != entity.Id))
            {
                throw new ExElementoExistente(entity.Nombre);
            }

            o.Nombre = entity.Nombre;
            o.Eliminada = entity.Eliminada;
            o.VolumenDefaultId = entity.VolumenDefaultId;

            if (!string.IsNullOrEmpty(entity.VolumenDefaultId))
            {
                VolumenPuntoMontaje vpm = await repoVPM.UnicoAsync(x => x.VolumenId == entity.VolumenDefaultId
                && x.PuntoMontajeId == o.Id);
                if (vpm == null)
                {
                    vpm = new VolumenPuntoMontaje()
                    {
                        PuntoMontajeId = entity.Id,
                        VolumenId = entity.VolumenDefaultId
                    };
                    await repoVPM.CrearAsync(vpm);
                }
            }

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

            // Añade un filtro permanente para la unidad organizacional
            query.Filtros.RemoveAll(x => x.Propiedad == "TenantId");
            query.Filtros.RemoveAll(x => x.Propiedad == "OrigenId");
            query.Filtros.Add(new FiltroConsulta() { Propiedad = "OrigenId", Operador = FiltroConsulta.OP_EQ, Valor = RegistroActividad.UnidadOrgId });

            return query;
        }
        public async Task<IPaginado<PuntoMontaje>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            seguridad.EstableceDatosProceso<PuntoMontaje>();
            try
            {
                Query = GetDefaultQuery(Query);

                long offset = Query.indice == 0 ? 0 : ((Query.indice) * Query.tamano) - 1;
                string sqls = @$"SELECT  COUNT(*) FROM {DbContextContenido.TablaPuntoMontaje} where  (1=1) ";
                string sqlsCount = "";
                int total = 0;
                string queryNoAdmin = "";
                bool esAdmin = false;

                var FOU = Query.Filtros.Where(x => x.Propiedad == "OrigenId").SingleOrDefault();
                if (FOU != null)
                {
                    var acceso = usuario.Accesos.Where(x => x.OU == FOU.Valor).FirstOrDefault();
                    if (acceso != null)
                    {
                        esAdmin = acceso.Admin;
                    }
                }

                if (this.permisos !=null && this.permisos.Admin)
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
                    queryNoAdmin = @$" and (Id in (select PuntoMontajeId  from {DbContextContenido.TablaPermisosPuntoMontaje} where DestinatarioId in ({roles})))";
                }



                if (Query.Filtros != null)
                {
                    List<string> condiciones = MySQLQueryComposer.Condiciones<PuntoMontaje>(Query, "");
                    foreach (string s in condiciones)
                    {
                        sqls += $" and ({s})";
                    }
                }
                sqls += $" {queryNoAdmin}";

                // Consulta de conteo sin ordenamiento ni limites
                sqlsCount = sqls;

                sqls += $" order by {Query.ord_columna} {Query.ord_direccion} ";
                sqls += $" LIMIT {offset},{Query.tamano}";

                sqls = sqls.Replace("COUNT(*)", "DISTINCT *");
                
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


                Paginado<PuntoMontaje> respuesta = new Paginado<PuntoMontaje>()
                {
                    ConteoFiltrado = total,
                    ConteoTotal = total,
                    Desde = Query.indice * Query.tamano,
                    Indice = Query.indice,
                    Tamano = Query.tamano,
                    Paginas = (int)Math.Ceiling(total / (double)Query.tamano)
                };
                respuesta.Elementos = this.UDT.Context.PuntosMontaje.FromSqlRaw(sqls).ToList();

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
            seguridad.EstableceDatosProceso<PuntoMontaje>();
            
            List<PuntoMontaje> listaEliminados = new List<PuntoMontaje>();
            foreach (var Id in ids.LimpiaIds())
            {
                PuntoMontaje d = await this.UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == Id);
                if (d != null)
                {
                    await seguridad.IdEnUnidadOrg(d.OrigenId);
                    listaEliminados.Add(d);
                }
            }

            if(listaEliminados.Count>0)
            {
                foreach(var d in listaEliminados)
                {
                    d.Eliminada = true;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    await seguridad.RegistraEventoEliminar(d.Id, d.Nombre);

                }
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x=>x.Id).ToList();
        }

        public async Task<IEnumerable<string>> Restaurar(string[] ids)
        {
            seguridad.EstableceDatosProceso<PuntoMontaje>();
            List<PuntoMontaje> listaEliminados = new List<PuntoMontaje>();
            foreach (var Id in ids.LimpiaIds())
            {
                PuntoMontaje d = await this.UDT.Context.PuntosMontaje.FirstOrDefaultAsync(x => x.Id == Id);
                if (d != null)
                {
                    await seguridad.IdEnUnidadOrg(d.OrigenId);
                    listaEliminados.Add(d);
                }
            }

            if (listaEliminados.Count > 0)
            {
                foreach (var d in listaEliminados)
                {
                    string original = d.Flat();
                    d.Eliminada = false;
                    this.UDT.Context.Entry(d).State = EntityState.Modified;
                    await seguridad.RegistraEventoActualizar(d.Id, d.Nombre, original.JsonDiff(false.Flat()));
                }
                UDT.SaveChanges();
            }
            return listaEliminados.Select(x => x.Id).ToList();
        }



        public async Task<PuntoMontaje> UnicoAsync(Expression<Func<PuntoMontaje, bool>> predicado = null, Func<IQueryable<PuntoMontaje>, IOrderedQueryable<PuntoMontaje>> ordenarPor = null, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> incluir = null, bool inhabilitarSegumiento = true)
        {

            PuntoMontaje d = await this.repo.UnicoAsync(predicado, ordenarPor, incluir);

            return d.Copia();
        }


        public async Task<List<string>> Purgar()
        {
            List<PuntoMontaje> ListaCarpeta = await this.repo.ObtenerAsync(x=>x.Eliminada==true);

            throw new NotImplementedException();
        }


        public async Task<List<ValorListaOrdenada>> ObtenerParesAsync(Consulta Query)
        {
            seguridad.EstableceDatosProceso<PuntoMontaje>();

            for (int i = 0; i < Query.Filtros.Count; i++)
            {
                if (Query.Filtros[i].Propiedad.ToLower() == "texto")
                {
                    Query.Filtros[i].Propiedad = "Nombre";
                    Query.Filtros[i].Operador = FiltroConsulta.OP_CONTAINS;
                }
            }
            if (Query.Filtros.Where(x => x.Propiedad.ToLower() == "eliminada").Count() == 0)
            {
                Query.Filtros.Add(new FiltroConsulta()
                {
                    Propiedad = "Eliminada",
                    Negacion = true,
                    Operador = "eq",
                    Valor = "true"
                });
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



        #region No Implemenatdaos

        public Task<IEnumerable<PuntoMontaje>> CrearAsync(params PuntoMontaje[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<PuntoMontaje>> CrearAsync(IEnumerable<PuntoMontaje> entities, CancellationToken cancellationToken = default)
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

        public Task<List<PuntoMontaje>> ObtenerAsync(Expression<Func<PuntoMontaje, bool>> predicado)
        {
            return this.repo.ObtenerAsync(predicado);
        }

        public Task<List<PuntoMontaje>> ObtenerAsync(string SqlCommand)
        {
            return this.repo.ObtenerAsync(SqlCommand);
        }

        public Task<IPaginado<PuntoMontaje>> ObtenerPaginadoAsync(Expression<Func<PuntoMontaje, bool>> predicate = null, Func<IQueryable<PuntoMontaje>, IOrderedQueryable<PuntoMontaje>> orderBy = null, Func<IQueryable<PuntoMontaje>, IIncludableQueryable<PuntoMontaje, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
        #endregion
    }



}