using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class ActivoAcervo
    {
        public string Dominio { get; set; }
        public string UnidadOrganizacional { get; set; }
        public string FechaApertura { get; set; }
        public string FechaCierre { get; set; }
        public string FechaRetencionAC { get; set; }
        public string FechaRetencionAT { get; set; }

        public string TecnicaSeleccion { get; set; }

        public EntradaClasificacion EntradaClasificacion { get; set; }

        public List<TipoValoracionDocumental> Valoraciones { get; set; }

        public Activo Activo { get; set; }

    }
}
