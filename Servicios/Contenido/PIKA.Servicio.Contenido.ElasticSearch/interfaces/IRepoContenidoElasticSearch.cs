using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public  interface IRepoContenidoElasticSearch
    {
        Task<bool> CreaRepositorio();
        Task<string> CreaVersion(Modelo.Contenido.Version version);
        Task<Modelo.Contenido.Version> ObtieneVersion(string Id);
        Task<bool> ActualizaVersion(string Id, Modelo.Contenido.Version version);
        Task<bool> EliminaVersion(string Id);
    }
}
