using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
   


    public class TipoArchivo : EntidadCatalogo<string, TipoArchivo> 
    {
        
        public TipoArchivo()
        {
            Archivos = new HashSet<Archivo>();
        }
        public override List<TipoArchivo> Seed()
        {
            List<TipoArchivo> l = new List<TipoArchivo>();
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_CORRESPONDENCIA, Nombre = "Correspondencoia"});
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_TRAMITE, Nombre = "Trámite" });
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_HISTORICO  , Nombre = "Histórico"});
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_CONSERVACION , Nombre = "Conservación"});
            return l;
        }

      

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<Archivo> Archivos { get; set; }

    }
}
