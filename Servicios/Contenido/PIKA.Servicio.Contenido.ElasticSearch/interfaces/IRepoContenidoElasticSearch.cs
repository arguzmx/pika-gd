using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.ElasticSearch.modelos;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.ElasticSearch
{
    public  interface IRepoContenidoElasticSearch
    {
        Task<EstadisticaVolumen> ObtieneEstadisticasVolumen(string VolId);
        Task<bool> CreaRepositorio();
        Task<string> CreaVersion(Modelo.Contenido.Version version);
        Task<Modelo.Contenido.Version> ObtieneVersion(string Id);
        Task<bool> ActualizaVersion(string Id, Modelo.Contenido.Version version, bool MarcarPorIndexar);
        Task<bool> EliminaVersion(string Id);
        Task<bool> EstadoVersion(string Id, bool Activa);
        Task<long> IndexadoPendiente(string volumenId);
        Task<Modelo.Contenido.Version> SiguenteIndexar(string volumenId, bool EstablacerEnproceso, string IdProcesador);
        Task<bool> ActualizaEstadoOCR(string Id, Modelo.Contenido.Version version);
        
        Task<bool> ActualizaEstadoOCR(string Id, string ProcesadorId, EstadoIndexado Estado);

        Task<bool> EliminaOCR(string Id, Modelo.Contenido.Version version);
        Task<bool> EliminaOCRVersion(Modelo.Contenido.Version version);
        Task<string> IndexarTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido);
        Task<bool> ActualizarTextoCompleto(string Id, Modelo.Contenido.ContenidoTextoCompleto contenido);
        Task<long> ContarPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy);
        Task<List<string>> IdsPorConsulta(string texto, string PuntoMontajeId, string IdJerarquico, int NivelFuzzy);
        Task<List<HighlightHit>> SinopsisPorIDs(string texto, List<string> Ids, string HtmlTag, int Tamano);
        Task<string> ExisteTextoCompleto(Modelo.Contenido.ContenidoTextoCompleto contenido);
        Task<EstadoOCR> OntieneEstadoOCR();
        Task ReiniciarOCRErroneos();
    }
}
