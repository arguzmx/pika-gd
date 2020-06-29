using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using static PIKA.Modelo.Metadatos.EventAttribute;

namespace PIKA.Modelo.Contacto
{

    /// <summary>
    /// Detalla la ubicació espacial de un elemento en forma de una dirección postal
    /// Las direcciones postales se  enlazan a los objeto a através de una entidad de relación
    /// con la finalidad de establecer relaciones varios a varios
    /// </summary>

    [Entidad(EliminarLogico: false)]
    public class DireccionPostal : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {
     
        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_NULO;


        public DireccionPostal()
        {
            this.TipoOrigenId = this.TipoOrigenDefault;
        }


        /// <summary>
        /// Identificador únioc de la dirección postal
        /// </summary>
        public override string Id { get => base.Id; set => base.Id = value; }


        /// <summary>
        /// NOmbre corto para reconocer la dirección por ejemplo: Casa, Oficina, etc
        /// </summary>
        public string Nombre { get; set; }


        /// <summary>
        /// Calle de la dirección
        /// </summary>
        public string Calle { get; set; }

        /// <summary>
        /// No. interior de la dirección
        /// </summary>
        public string NoInterno { get; set; }


        /// <summary>
        /// No. exterior de la dirección
        /// </summary>
        public string NoExterno { get; set; }


        /// <summary>
        /// Colonia o barrio de la dirección
        /// </summary>
        public string Colonia { get; set; }


        /// <summary>
        /// Código postal de la dirección
        /// </summary>
        public string CP { get; set; }


        /// <summary>
        /// Municipio o alcaldía de la dirección
        /// </summary>
        public string Municipio { get; set; }



        /// <summary>
        /// País de la dirección
        /// </summary>
        public string PaisId { get; set; }



        /// <summary>
        /// Estado del país de la dirección
        /// </summary>
        public string EstadoId { get; set; }


        /// <summary>
        /// Identifica si la dirección postal es la dirección por defecto para el origen
        /// </summary>
        public bool EsDefault { get; set; }

        /// <summary>
        /// Tipo de ojeto origen asociado a la dirección, pr ejemplo UO
        /// </summary>
        public string TipoOrigenId { get; set; }


        /// <summary>
        /// Identificador único del objeto origen, por ejemplo el id de una UO
        /// </summary>
        public string OrigenId { get; set; }

        


        public virtual Pais Pais { get; set; }

        public virtual Estado Estado { get; set; }
    }
}
