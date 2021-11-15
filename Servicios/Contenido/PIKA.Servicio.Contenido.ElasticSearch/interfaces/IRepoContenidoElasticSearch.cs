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
        Task<bool> ActualizarTextoCompleto(string Id, Modelo.Contenido.ContenidoTextoCompleto contenido);
        Task<long> ContarPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy);
        Task<List<string>> IdsPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy);
        Task<List<HighlightHit>> SinopsisPorIDs(string texto, List<string> Ids, string HtmlTag, int Tamano);
        Task<string> ExisteTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido);
    }
}
