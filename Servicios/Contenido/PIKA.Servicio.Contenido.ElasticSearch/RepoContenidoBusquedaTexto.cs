using Nest;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public partial class RepoContenidoElasticSearch : IRepoContenidoElasticSearch
    {
        public async Task<string> IndexarTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido)
        {
            var result = clienteOCR.IndexDocument(contenido);
            if (result.ApiCall.Success)
            {
                Console.WriteLine(result.Id);
                return result.Id;
            } 
            return null;
        }


        public async Task<long> IndexadoPendiente(string volumenId)
        {
            var searchResponse = await cliente.CountAsync<Modelo.Contenido.Version>(s =>
               s.Query(q =>
                q.Bool(b =>
                    b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                    Modelo.Contenido.EstadoIndexado.PorIndexar)))
            ));

            if (searchResponse.ApiCall.Success)
            {
                return searchResponse.Count;
            }

            return -1;
        }


        public async Task<bool> ActualizaEstadoOCR(string Id, Modelo.Contenido.Version version )
        {

            Console.WriteLine(Id);
            var resultado = await cliente.UpdateAsync<Modelo.Contenido.Version, object>(
                new DocumentPath<Modelo.Contenido.Version>(Id),
               u => u.Index(INDICEVERSIONES)
                   .DocAsUpsert(true)
                   .Doc(version)
                   .Refresh(Elasticsearch.Net.Refresh.True));

            Console.WriteLine(resultado.Result);

            if (resultado.Result == Result.Updated ||
                resultado.Result == Result.Noop)
            {
                return true;
            }

            return false;
        }
        public async Task<Modelo.Contenido.Version> SiguenteIndexar(string volumenId)
        {
            var searchResponse = await cliente.SearchAsync<Modelo.Contenido.Version>(s =>
            s.Size(1)
            .Query(q =>
              q.Bool(b =>
                  b.Filter(bf => bf.Term(t => t.EstadoIndexado,
                  Modelo.Contenido.EstadoIndexado.PorIndexar)))
          ));

            if (searchResponse.IsValid)
            {
                return searchResponse.Documents.FirstOrDefault();
            }
            else
            {
                Console.WriteLine($"{searchResponse.ServerError.ToString()}");
            }

            return null;
        }
    }
}
