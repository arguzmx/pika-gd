using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Organizacion
{

    //[EntidadMiembro(ObjetoMiembro: "propiedadesusuario", ObjetoMembresia: "usuariosrol",
    //   ColumnaIdMiembro: "ApplicationUserId", ColumnaIdMembresia: "RolId",
    //   PropiedadPadre: "Id")]
    /// <summary>
    /// Rol laboral en la organización
    /// </summary>
    [Entidad(EliminarLogico: false)]
    [EntidadVinculada(EntidadHijo: "usuariosrol", Cardinalidad: TipoCardinalidad.UnoVarios,
        PropiedadPadre: "Id", PropiedadHijo: "RolId", PropiedadIdMiembro: "ApplicationUserId",
        TipoDespliegueVinculo: TipoDespliegueVinculo.Membresia)]
    public class Rol : Entidad<string>, IEntidadNombrada, IEntidadRelacionada
    {

       [XmlIgnore]
       [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;


        public Rol() {
            this.UsuariosRol = new HashSet<UsuariosRol>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre del rol
        /// </summary>

        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre {get; set;}

        /// <summary>
        /// Descripción del rol
        /// </summary>
        [Prop(Required: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 1, maxlen: 200)]
        public string Descripcion { get; set; }

        /// <summary>
        /// Los roles se relacionan con un dominio o una unidad organizacional
        /// que representa el contexto de uso para el rol
        /// Los roles de de dominios son globales a la organización
        /// Los roles de unidad oerganizacional sólo son aplicables a la OU
        /// </summary>
        [Prop(Required: true, OrderIndex: 3, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_DOMINIO, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string TipoOrigenId {get; set;}

        /// <summary>
        /// Identificador único del dominio que contiene los roles
        /// </summary>
        [Prop(Required: true, Visible: false, OrderIndex: 3, Contextual: true, IdContextual: ConstantesModelo.GLOBAL_DOMINIOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string OrigenId {get; set;}

        
        /// <summary>
        /// USuarios participantes en el rol
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public ICollection<UsuariosRol> UsuariosRol { get; set; }
    }
}
