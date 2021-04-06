using RepositorioEntidades;
using Nest;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIKA.Modelo.Contenido
{
    public class Parte : Entidad<string>, IEntidadEliminada 
    {

        public Parte()
        {

        }

        /// <summary>
        /// Esta propiedad se ignora para elastci search el ID a utilizar en el del indice
        /// </summary>
        [Ignore]
        
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Id del elemento contenido padre
        /// No se utiliza en elastc search
        /// </summary>
        [Ignore]
        public string ElementoId { get; set; }
        //#GUID requerida, CLAVE PRIAMRIA

        /// <summary>
        /// Version del elemento al que pertenecen las partes, 
        /// si el elemento no es versionable esta cadena es nula
        /// No se utiliza en elastc search
        /// </summary>
        [Ignore]
        public string VersionId { get; set; }

        /// <summary>
        /// POsición relativa de la parte dentro del elemento de contenido
        /// </summary>
        [Number(NumberType.Integer, Name = "i")]
        public int Indice { get; set; }

        /// <summary>
        /// Consecutivo del elemento para el alamcenamiento, esta propieda tambié existe en el Volumen 
        /// para poder asociar un Id de tipo String, con uno númerico si es necesario
        /// </summary>
        [Number(NumberType.Long, Name = "c")]
        public long ConsecutivoVolumen { get; set; }
        //Requeirod, default=0

        /// <summary>
        /// Tipo MIME asociado al contenido
        /// </summary>
        [Ignore]
        public string TipoMime { get; set; }


        /// <summary>
        /// Extensión del archivo recibido
        /// </summary>
        [Keyword(Name = "ex")]
        public string Extension { get; set; }

        /// <summary>
        /// Longidut en bytes de la parte
        /// </summary>
        [Number(NumberType.Long, Name = "l")]
        public long LongitudBytes { get; set; }

        /// <summary>
        /// Nombre original de la parte, corresponde con el nombre del archivo electrónico
        /// o un nombre especificado por el usuario para la parte
        /// </summary>
        [Ignore]
        public string NombreOriginal { get; set; }

        /// <summary>
        /// Indica si el ekemebnto ha sido marcado para eliminar, en el caso de las partes el proceso de eliminación 
        /// es asíncrono basado en esta propiedad
        /// </summary>
        [Boolean(Name = "x")]
        public bool Eliminada { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo imagen
        /// </summary>
        [Boolean(Name = "eim")]
        public bool EsImagen { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo audio
        /// </summary>
        [Boolean(Name = "eau")]
        public bool EsAudio { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        [Boolean(Name = "evi")]
        public bool EsVideo { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        [Boolean(Name = "pdf")]
        public bool EsPDF { get; set; }

        /// <summary>
        /// Indica si la parte tiene una miniatura generada
        /// </summary>
        [Boolean(Name = "min")]
        public bool TieneMiniatura { get; set; }

        /// <summary>
        /// Determina si el contenido ha sido indexado
        /// </summary>
        [Boolean(Name = "ix")]
        public bool Indexada { get; set; }

        /// <summary>
        /// Identificador único del volumen para la parte
        /// </summary>
        [Ignore]
        public string VolumenId { get; set; }

        [JsonIgnore, XmlIgnore, Ignore, NotMapped]
        public virtual Elemento Elemento { get; set; }

        [JsonIgnore, XmlIgnore, Ignore, NotMapped]
        public virtual Version Version { get; set; }

        [JsonIgnore, XmlIgnore, Ignore, NotMapped]
        public virtual Volumen Volumen { get; set; }

    }
}
