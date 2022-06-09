using System;
using System.Collections.Generic;
using System.Text;

namespace PikaOCR.modelo
{
    public class TrabajoOCRRemoto
    {

        public TrabajoOCRRemoto()
        {
            Paginas = new List<PaginaOCRRemoto>();
        }

        public string ProcesadorId { get; set; }
        public string VersionId { get; set; }
        public string ElementoId { get; set; }
        public string VolumenId { get; set; }
        public string CarpetaId { get; set; }
        public string PuntoMontajeId { get; set; }

        public List<PaginaOCRRemoto> Paginas { get; set; }
    }

    public class PaginaOCRRemoto
    {
        public PaginaOCRRemoto() {
            Resultados = new List<ResultadoOCR>();
        }

        public string Id { get ; set ; }
        public int Indice { get; set; }
        public long ConsecutivoVolumen { get; set; }
        public string Extension { get; set; }
        public bool EsImagen { get; set; }
        public bool EsPDF { get; set; }

        public List<ResultadoOCR> Resultados { get; set; }
    }

    public class ResultadoOCR
    {
        public string Texto { get; set; }
        public int Pagina { get; set; }
    }

    public class FinTrabajoRemoto
    {
        public string ProcesadorId { get; set; }
        public string VersionId { get; set; }
        public bool Ok { get; set; }
        public string Error { get; set; }
    }

}
