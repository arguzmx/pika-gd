using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes
{
    public class ReporteCuadroClasificacion : IProveedorReporte
    {
        private const string nombre = "Cuadro de clasificación";
        private const string url = "reporte/cc/{id}";
        private List<ParametroReporte> parametroes ;
        private List<FormatoReporte> formatos;
        
        public ReporteCuadroClasificacion()
        {
            parametroes = new List<ParametroReporte>();
            formatos = new List<FormatoReporte>();

            parametroes.Add(new ParametroReporte() { Id = "id", 
                Nombre = "Identificador único cuadro clasifiacion", 
                Tipo = TipoDato.tString , 
                Contextual = true,
                IdContextual = ConstantesModelo.PREFIJO_CONEXTO + "Id"
            });

            formatos.Add(new FormatoReporte() { Id = FormatoReporte.EXCEL, Nombre = "Excel" });
        }

        public string Nombre { get => nombre; set { }  }
        public string Url { get => url; set { } }

        public List<ParametroReporte> Parametros { get => parametroes; set { } }

        
        public List<FormatoReporte> FormatosDisponibles { get => formatos; set { } }
    }
}
