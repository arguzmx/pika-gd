using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.GestionDocumental.Servicios.Reporte
{
    public class PropiedadesImportadorActivos
    {
        public byte[] archivo { get; set; }
        public string ArchivoId { get; set; }
        public string TipoOrigenId { get; set; }
        public string OrigenId { get; set; }
        public string FormatoFecha { get; set; }
    }
}
