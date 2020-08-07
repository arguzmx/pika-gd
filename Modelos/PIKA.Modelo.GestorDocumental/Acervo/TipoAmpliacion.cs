using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class TipoAmpliacion: EntidadCatalogo<string, TipoAmpliacion>
    {
        public TipoAmpliacion() {
            this.Ampliaciones = new HashSet<Ampliacion>();
        }
        public const string RESERVA = "reserva";
        public const string CONFIDENCIALIDAD = "confidencialidad";
        public const string SOLICITUD_INFORMACION = "solicitud-informacion";

        
        public override List<TipoAmpliacion> Seed()
        {
            List<TipoAmpliacion> lista = new List<TipoAmpliacion>();
            lista.Add(new TipoAmpliacion() { Id = RESERVA, Nombre = "Reserva" });
            lista.Add(new TipoAmpliacion() { Id = CONFIDENCIALIDAD, Nombre = "Confidencialidad" });
            lista.Add(new TipoAmpliacion() { Id = SOLICITUD_INFORMACION, Nombre = "Solicitud de información" });
            return lista;
        }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Ampliacion> Ampliaciones { get; set; }
    }
}
