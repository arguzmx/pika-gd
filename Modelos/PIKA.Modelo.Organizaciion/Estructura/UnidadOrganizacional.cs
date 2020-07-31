using Microsoft.CodeAnalysis.CSharp.Syntax;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Infraestructura.Comun;

namespace PIKA.Modelo.Organizacion
{
    [Entidad(EliminarLogico: true)]
    [EntidadVinculada( EntidadHijo: "direccionpostal", Cardinalidad: TipoCardinalidad.UnoUno, 
        PropiedadPadre: "Id", PropiedadHijo: "OrigenId") ]
    /// <summary>
    /// Las unidades organizacionales agrupan recursos para la organización del trabajo
    /// </summary>
    public class UnidadOrganizacional : Entidad<string>, IEntidadNombrada, IEntidadEliminada
    {
        public const string CampoDominioId = "DominioId";

        /// <summary>
        /// Identificador único de la UI
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }



        /// <summary>
        /// NOmbre de la unodad organizacional
        /// </summary>
        [Prop(Required: true, OrderIndex: 1)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre {get; set;}


        /// <summary>
        /// Destermina si la unidad ha sido eliminada
        /// </summary>
        [Prop(OrderIndex: 2, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        public bool Eliminada {get; set;}

        /// <summary>
        /// Identiicador único del cominio al que se asocia la UO
        /// </summary>

        [Prop(Required: true,  Visible: false, OrderIndex: 3, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_DOMINIOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string DominioId { get; set; }

        [JsonIgnore]
        [XmlIgnore]
        public Dominio Dominio { get; set; }

    }
}
