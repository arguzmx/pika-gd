﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    public class Parte : Entidad<string>, IEntidadEliminada 
    {


        public Parte()
        {

        }

        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Id del elemento contenido padre
        /// </summary>
        public string ElementoId { get; set; }
        //#GUID requerida, CLAVE PRIAMRIA

        /// <summary>
        /// Version del elemento al que pertenecen las partes, 
        /// si el elemento no es versionable esta cadena es nula
        /// </summary>
        public string VersionId { get; set; }
        //#GUID requerida, CLAVE PRIAMRIA

        /// <summary>
        /// POsición relativa de la parte dentro del elemento de contenido
        /// </summary>
        public int Indice { get; set; }
        //Requeirod, default=0

        /// <summary>
        /// Consecutivo del elemento para el alamcenamiento, esta propieda tambié existe en el Volumen 
        /// para poder asociar un Id de tipo String, con uno númerico si es necesario
        /// </summary>
        public long ConsecutivoVolumen { get; set; }
        //Requeirod, default=0

        /// <summary>
        /// Tipo MIME asociado al contenido
        /// </summary>
        public string TipoMime { get; set; }
        //Longutidud  =  crear longiutd MIME con tamanaño 50

        /// <summary>
        /// Extensión del archivo recibido
        /// </summary>
        public string Extension { get; set; }

        /// <summary>
        /// Longidut en bytes de la parte
        /// </summary>
        public long LongitudBytes { get; set; }

        /// <summary>
        /// Nombre original de la parte, corresponde con el nombre del archivo electrónico
        /// o un nombre especificado por el usuario para la parte
        /// </summary>
        public string NombreOriginal { get; set; }
        //# opcional, longitud nombre

        /// <summary>
        /// Indica si el ekemebnto ha sido marcado para eliminar, en el caso de las partes el proceso de eliminación 
        /// es asíncrono basado en esta propiedad
        /// </summary>
        public bool Eliminada { get; set; }
        //Default=false

        /// <summary>
        /// Indica si la parte es del tipo imagen
        /// </summary>
        public bool EsImagen { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo audio
        /// </summary>
        public bool EsAudio { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        public bool EsVideo { get; set; }

        /// <summary>
        /// Indica si la parte es del tipo video
        /// </summary>
        public bool EsPDF { get; set; }

        /// <summary>
        /// Indica si la parte tiene una miniatura generada
        /// </summary>
        public bool TieneMiniatura { get; set; }

        /// <summary>
        /// Determina si el contenido ha sido indexado
        /// </summary>
        public bool Indexada { get; set; }

        /// <summary>
        /// Identificador único del volumen para la parte
        /// </summary>
        public string VolumenId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public virtual Elemento Elemento { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public virtual Version Version { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public virtual Volumen Volumen { get; set; }

    }
}
