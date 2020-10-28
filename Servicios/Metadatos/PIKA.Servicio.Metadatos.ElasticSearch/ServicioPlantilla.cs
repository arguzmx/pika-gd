using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Configuration;
using Nest;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Interfaces;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Metadatos.ElasticSearch
{
    public class ServicioPlantilla : IServicioPlantilla, IServicioInyectable
    {
        public const string INDICEPLANTILLASMETADATOS = "PIKAMETADATOS";
        private readonly IConfiguration Configuration;
        private ElasticClient cliente;
        public ServicioPlantilla(IConfiguration configuration)
        {
            this.Configuration = configuration;
            ConfiguracionRepoMetadatos options = new ConfiguracionRepoMetadatos();
            Configuration.GetSection("Metadatos").Bind(options);

            var settings = new ConnectionSettings(new Uri(options.CadenaConexion()))
                            .DefaultIndex(INDICEPLANTILLASMETADATOS);
            cliente = new ElasticClient();
        }

        #region PLantilla


        private async Task<Plantilla> ValidaPlantilla(Plantilla plantilla, bool actualziar)
        {
            var respuesta = await cliente.SearchAsync<Plantilla>(s => s
            .Query(q => q
                .Bool(b => b
                    .Must(mu => mu
                        .Term(m => m.Nombre, plantilla.Nombre)
                    ).
                    MustNot(mn => mn
                        .Term(m => m.Id, plantilla.Id)
                    )
                    .Filter(fi => fi.Bool(b => b
                            .Must(mu => mu.Term(m => m.OrigenId, plantilla.OrigenId))
                            .Must(mu => mu.Term(m => m.TipoOrigenId, plantilla.TipoOrigenId))
                        )
                    )
                )
            )
          );


            if (respuesta.Total > 0)
            {

            }

            if(actualziar)
            {

            } else
            {
                plantilla.Id = Guid.NewGuid().ToString();
            }


            return plantilla;
        }

        public async Task<Plantilla> CrearAsync(Plantilla plantilla, CancellationToken cancellationToken = default)
        {
            plantilla = await ValidaPlantilla(plantilla, false);

            var indexResponseAsync = await cliente.IndexDocumentAsync(plantilla);
            if (indexResponseAsync.Result == Result.Created)
            {
                return plantilla;
            }

            return null;
        }




        public async Task<bool> Actualizar(string plantillaId, Plantilla plantilla)
        {

            var respuesta = await cliente.UpdateAsync<Plantilla, dynamic>(new DocumentPath<Plantilla>(plantillaId), 
                u => u.Index(INDICEPLANTILLASMETADATOS).Doc(plantilla));

            return (respuesta.Result == Result.Updated);

        }

        public async Task<bool> Eliminar(string plantillaId)
        {
            var respuesta = await cliente.DeleteAsync(new DocumentPath<Plantilla>(plantillaId));
            return (respuesta.Result == Result.Updated);
        }

        #endregion


        public Task<bool> ActualizarPropiedad(string plantillaId, PropiedadPlantilla propiedad)
        {
            throw new NotImplementedException();
        }

    

        public Task<Plantilla> CrearPropiedad(string plantillaId, PropiedadPlantilla propiedad)
        {
            throw new NotImplementedException();
        }

   

        public Task<bool> EliminarPropiedad(string plantillaId, string propidadId)
        {
            throw new NotImplementedException();
        }

        public Task<Plantilla> Leer(string plantillaId)
        {
            throw new NotImplementedException();
        }

        public Task<Plantilla> UnicoAsync(Expression<Func<Plantilla, bool>> predicado = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> ordenarPor = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> incluir = null, bool inhabilitarSegumiento = true)
        {
            throw new NotImplementedException();
        }

        public Task<List<Plantilla>> ObtenerAsync(Expression<Func<Plantilla, bool>> predicado)
        {
            throw new NotImplementedException();
        }

        public Task<List<Plantilla>> ObtenerAsync(string SqlCommand)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Expression<Func<Plantilla, bool>> predicate = null, Func<IQueryable<Plantilla>, IOrderedQueryable<Plantilla>> orderBy = null, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, int index = 0, int size = 20, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IPaginado<Plantilla>> ObtenerPaginadoAsync(Consulta Query, Func<IQueryable<Plantilla>, IIncludableQueryable<Plantilla, object>> include = null, bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        

        public Task<IEnumerable<Plantilla>> CrearAsync(params Plantilla[] entities)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Plantilla>> CrearAsync(IEnumerable<Plantilla> entities, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task ActualizarAsync(Plantilla entity)
        {
            throw new NotImplementedException();
        }

        public Task<ICollection<string>> Eliminar(string[] ids)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<string>> Restaurar(string[] ids)
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

        public Task<bool> Existe(Expression<Func<Plantilla, bool>> predicado)
        {
            throw new NotImplementedException();
        }
    }
}
