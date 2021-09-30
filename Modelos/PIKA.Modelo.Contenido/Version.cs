using Nest;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{
    public class Version: Entidad<string>, IEntidadRegistroCreacion ,IEntidadEliminada
    {

        public Version()
        {
            this.EstadoIndexado = EstadoIndexado.PorIndexar;
        }

        /// <summary>
        /// Identificador único de la version
        /// </summary>
        [Keyword]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Identificador único del elemento al que pertenece la versión
        /// </summary>
        [Keyword(Name = "el_id")]
        public string ElementoId { get; set; }

        /// <summary>
        /// Indica si la versión es la activa, sólo pude existir una versión activa por elemento
        /// </summary>
        [Boolean(Name = "act")]
        public bool Activa { get; set; }


        /// <summary>
        /// Esatdo de indexado de la versión de contenido
        /// </summary>
        [Number(NumberType.Integer, Name = "eidx")]
        public EstadoIndexado EstadoIndexado { get; set; }

        /// <summary>
        /// Fecha de ceración de la versión
        /// </summary>
        [Date(Name = "cr_f")]
        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Identificadro único del creador de la versión
        /// </summary>
        [Keyword(Name = "cr_id")]
        public string CreadorId { get; set; }

        /// <summary>
        /// Especifica si la versión ha sido eliminada
        /// </summary>
        [Boolean(Name = "elim")]
        public bool Eliminada { get; set; }

        /// <summary>
        /// Mantiene la cuenta del número de partes asociadas a la versión
        /// </summary>
        [Number(NumberType.Integer, Name = "con_p")]
        public int ConteoPartes { get; set; }


        /// <summary>
        /// Mantiene el tamaño en bytes de las partes de la versión
        /// </summary>
        [Number(NumberType.Integer, Name = "tam")]
        public long TamanoBytes { get; set; }


        /// <summary>
        /// Identificador único del volumen para la version
        /// </summary>
        [Keyword(Name = "vol_id")]
        public string VolumenId { get; set; }

        /// <summary>
        /// Mantiene el indice del número de partes de la versión
        /// </summary>
        [Number(NumberType.Integer, Name = "max_p")]
        public int MaxIndicePartes { get; set; }

        /// <summary>
        /// LIsta de partes que componene la versión
        /// </summary>
        [JsonIgnore, XmlIgnore, NotMapped]
        [Object( Name = "partes")]
        public virtual List<Parte> Partes { get; set; }


        [JsonIgnore, XmlIgnore, Ignore, NotMapped]
        public virtual Elemento Elemento { get; set; }

        [JsonIgnore, XmlIgnore, Ignore, NotMapped]
        public virtual Volumen Volumen { get; set; }
    }
}
