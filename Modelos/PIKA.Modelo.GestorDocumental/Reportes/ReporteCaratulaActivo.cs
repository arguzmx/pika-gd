using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes
{
    public class ReporteCaratulaActivo: IProveedorReporte
    {
        private const string nombre = "Carátula del acervo";
        private const string url = "reporte/caractula/{id}";
        private const string id = "caratulaactivo";
        private List<ParametroReporte> parametroes;
        private List<FormatoReporte> formatos;

        public ReporteCaratulaActivo()
        {
            parametroes = new List<ParametroReporte>();
            formatos = new List<FormatoReporte>();

            parametroes.Add(new ParametroReporte()
            {
                Id = "id",
                Nombre = "Identificador único del activo",
                Tipo = TipoDato.tString,
                Contextual = true,
                IdContextual = ConstantesModelo.PREFIJO_CONEXTO + "Id"
            });

            formatos.Add(new FormatoReporte() { Id = FormatoReporte.WORD, Nombre = "Word" });

            this.Nombre = nombre;
            this.Url = url;
            this.Id = id;
            this.DatosJson = true;
            this.Parametros = parametroes;
            this.FormatosDisponibles = formatos;

        }

        public bool DatosJson { get; set; }

        public string Id { get; set; }

        public string Nombre { get; set; }
        public string Url { get; set; }

        public List<ParametroReporte> Parametros { get; set; }


        public List<FormatoReporte> FormatosDisponibles { get; set; }
    }
}
