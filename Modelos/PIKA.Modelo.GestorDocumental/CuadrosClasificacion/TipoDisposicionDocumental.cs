using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class TipoDisposicionDocumental : EntidadCatalogo<string, TipoDisposicionDocumental>
    {
        public TipoDisposicionDocumental()
        {
            EntradaClasificacion = new HashSet<EntradaClasificacion>();
        }

        public const string SELECCION_COMPLETA = "seleccion-completa";
        public const string ELIMINACION_COMPLETA = "eliminacion-completa";
        public const string MUESTREO = "muestreo";
        public const string CAMBIO_SOPORTE = "cambio-soporte";


        public override List<TipoDisposicionDocumental> Seed()
        {
            List<TipoDisposicionDocumental> lista = new List<TipoDisposicionDocumental>();

            lista.Add(new TipoDisposicionDocumental() { Id = SELECCION_COMPLETA, Nombre = "Selección completa para histórico" });
            lista.Add(new TipoDisposicionDocumental() { Id = ELIMINACION_COMPLETA, Nombre = "Eliminación completa del acervo" });
            lista.Add(new TipoDisposicionDocumental() { Id = MUESTREO, Nombre = "Muestreo para caonservación histórica" });
            lista.Add(new TipoDisposicionDocumental() { Id = CAMBIO_SOPORTE, Nombre = "Cambio de medio de soporte, microfilmación o digitalización" });

            return lista;
        }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<EntradaClasificacion> EntradaClasificacion { get; set; }
    }
}
