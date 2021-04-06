using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PIKA.Infraestructura.Comun.Servicios;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using PIKA.Modelo.Metadatos.Busqueda;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Servicios.Busqueda
{
    [ContextoBusqeda(Id: "carpeta",  Contexto: "contenido" )]
    public class ServicioBusquedaCarpeta : ContextoServicioContenido, IServicioBusqueda<Carpeta, string>
    {
        private IRepositorioAsync<Carpeta> repo;
        private UnidadDeTrabajo<DbContextContenido> UDT;
        private const string DEFAULT_SORT_COL = "Nombre";
        private const string DEFAULT_SORT_DIRECTION = "asc";
        protected readonly DbSet<Carpeta> _dbSet;

        public ServicioBusquedaCarpeta(
            IProveedorOpcionesContexto<DbContextContenido> proveedorOpciones,
            ILogger<ServicioLog> Logger
        ) : base(proveedorOpciones, Logger)
        {
            this.UDT = new UnidadDeTrabajo<DbContextContenido>(contexto);
            this.repo = UDT.ObtenerRepositoryAsync<Carpeta>(new QueryComposer<Carpeta>());
            _dbSet = contexto.Set<Carpeta>();
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


        public async Task<IPaginado<Carpeta>> ObtenerConteoAsync(Consulta consulta, CancellationToken tokenCancelacion = default)
        {
            consulta = GetDefaultQuery(consulta);
            var respuesta = await this.repo.ObtenerConteoAsync(consulta);
            return respuesta;
        }

        public async Task<IPaginado<Carpeta>> ObtenerPaginadoAsync(Consulta consulta, CancellationToken tokenCancelacion = default)
        {
            consulta = GetDefaultQuery(consulta);
            var respuesta = await this.repo.ObtenerPaginadoAsync(consulta);
            return respuesta;
        }

        public async Task<IPaginado<string>> ObtenerPaginadoIds(Consulta consulta, CancellationToken tokenCancelacion = default)
        {
            consulta = GetDefaultQuery(consulta);
            consulta.indice = 0;
            consulta.tamano = int.MaxValue;
            var c = new QueryComposer<Elemento>();

            List<Expression<Func<Elemento, bool>>> filtros = null;
            if (filtros == null) filtros = new List<Expression<Func<Elemento, bool>>>();

            IQueryable<Carpeta> query = _dbSet;
            query = query.AsNoTracking();

            if (consulta.Filtros.Count > 0 || filtros.Count > 0)
            {

                var type = typeof(Elemento);
                ParameterExpression pe = Expression.Parameter(type, "search");


                Expression predicateBody = c.Componer(pe, consulta);
                Expression<Func<Elemento, bool>> lambdaPredicado = null;
                if (predicateBody != null)
                {
                    lambdaPredicado = Expression.Lambda<Func<Elemento, bool>>(predicateBody, pe);
                    filtros.Insert(0, lambdaPredicado);
                };

                if (filtros.Count > 0)
                {
                    var filtro = filtros[0];
                    for (int i = 1; i < filtros.Count; i++)
                    {
                        filtro = filtro.AndAlso(filtros[i]);
                    }

                    MethodCallExpression whereCallExpression = Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { query.ElementType },
                    query.Expression,
                    filtro);
                    query = query.Provider.CreateQuery<Carpeta>(whereCallExpression);
                }
            }
            return await query.PaginadoCarpetasAsync(consulta.indice, consulta.tamano);
        }

        public async Task<MetadataInfo> ObtieneMetadatosBusqueda()
        {
            await Task.Delay(1);

            MetadataInfo m = new MetadataInfo()
            {
                Tipo = "carpeta",
                FullName = "carpeta",
                Propiedades = new List<Propiedad>()
            };

            var el = new Carpeta();

            m.Propiedades.Add(new Propiedad()
            {
                Id = nameof(el.Id),
                Nombre = nameof(el.Id),
                TipoDatoId = TipoDato.tString,
                Buscable = true,
                AlternarEnTabla = true,
                ControlHTML = ControlUI.HTML_TEXT,
                EsIdRegistro = true,
                Visible = true,
                Etiqueta = false,
                MostrarEnTabla = true,
                IndiceOrdenamiento =0,
                IndiceOrdenamientoTabla =0
            });

            m.Propiedades.Add(new Propiedad()
            {
                Id = nameof(el.Nombre),
                Nombre = nameof(el.Nombre),
                TipoDatoId = TipoDato.tString,
                Buscable = true,
                AlternarEnTabla = true,
                ControlHTML = ControlUI.HTML_TEXT,
                EsIdRegistro = true,
                Visible = true,
                Etiqueta = false,
                MostrarEnTabla = true,
                IndiceOrdenamiento = 1,
                IndiceOrdenamientoTabla = 1
            });

            m.Propiedades.Add(new Propiedad()
            {
                Id = nameof(el.FechaCreacion),
                Nombre = nameof(el.FechaCreacion),
                TipoDatoId = TipoDato.tDateTime,
                Buscable = true,
                AlternarEnTabla = true,
                ControlHTML = ControlUI.HTML_DATETIME,
                EsIdRegistro = true,
                Visible = true,
                Etiqueta = false,
                MostrarEnTabla = true,
                IndiceOrdenamiento = 2,
                IndiceOrdenamientoTabla = 2
            });

            m.Propiedades.Add(new Propiedad()
            {
                Id = nameof(el.Eliminada),
                Nombre = nameof(el.Eliminada),
                TipoDatoId = TipoDato.tBoolean,
                Buscable = true,
                AlternarEnTabla = true,
                ControlHTML = ControlUI.HTML_CHECKBOX,
                EsIdRegistro = true,
                Visible = true,
                Etiqueta = false,
                MostrarEnTabla = true,
                IndiceOrdenamiento = 3,
                IndiceOrdenamientoTabla = 3
            });
            return m;
        }
    }
}
