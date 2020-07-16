using RepositorioEntidades;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class EstadoCuadroClasificacion : EntidadCatalogo<string, EstadoCuadroClasificacion>
    {
        public const string ESTADO_ACTIVO = "on";
        public const string ESTADO_INACTIVO = "off";

        public EstadoCuadroClasificacion()
        {
            Cuadros = new HashSet<CuadroClasificacion>();
        }
        public override List<EstadoCuadroClasificacion> Seed() {
            List<EstadoCuadroClasificacion> l = new List<EstadoCuadroClasificacion>();
            l.Add(new EstadoCuadroClasificacion() { Id = ESTADO_ACTIVO, Nombre = "Activo" });
            l.Add(new EstadoCuadroClasificacion() { Id = ESTADO_INACTIVO, Nombre = "Inactivo" });
            return l;
        }

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<CuadroClasificacion> Cuadros { get; set; }
    }
}
