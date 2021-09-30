using PIKA.Modelo.Contenido;
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
        Task<long> IndexadoPendiente(string volumenId);
        Task<Modelo.Contenido.Version> SiguenteIndexar(string volumenId);
        Task<bool> ActualizaEstadoOCR(string Id, Modelo.Contenido.Version version);
        Task<string> IndexarTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido);
    }
}
