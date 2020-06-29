using Microsoft.CodeAnalysis.CSharp.Syntax;
using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// Las unidades organizacionales agrupan recursos para la organización del trabajo
    /// </summary>
    public class UnidadOrganizacional : Entidad<string>, IEntidadNombrada, IEntidadEliminada
    {
        public const string CampoDominioId = "DominioId";
       
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// NOmbre de la unodad organizacional
        /// </summary>
        public string Nombre {get; set;}


        /// <summary>
        /// Destermina si la unidad ha sido eliminada
        /// </summary>
        public bool Eliminada {get; set;}

        /// <summary>
        /// Identiicador único del cominio al que se asocia la UO
        /// </summary>
        public string DominioId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Dominio Dominio { get; set; }

    }
}
