using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Modelo.Contacto;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json.Serialization;
using static PIKA.Modelo.Metadatos.PropAttribute;

namespace PIKA.Modelo.Seguridad
{
    // [EntidadVinculada(Entidad: "UnidadOrganizacional", Cardinalidad: TipoCardinalidad.UnoVarios, Padre: "Id", Hijo: "DominioId")]
    [Entidad(EliminarLogico: true, 
        TokenApp: ConstantesAppSeguridad.APP_ID, 
        TokenMod: ConstantesAppSeguridad.MODULO_USUARIOS)]
    public class PropiedadesUsuario
    {

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public string UsuarioId { get; set; }

        /// <summary>
        /// Esta propeidad viene de la tabla AspNetUser en oeraciones GET, debe incluirse en POST
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.add)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string username{ get; set; }


        /// <summary>
        /// Esta propiedad solo es util al creal el usuario y no debe persisttirse
        /// </summary>
        [NotMapped]
        [Prop(Required: true, OrderIndex: 20, ShowInTable:false, Visible:false, Searchable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_PASSWORD_CONFIRM, Accion: Acciones.add)]
        [ValidString(minlen: 6, maxlen: 50, regexp: ValidStringAttribute.REGEXP_USERPASS)]
        public string password { get; set; }

        /// <summary>
        ///  OIDC Claims se alamcenana en la tabla AspNetUserClaims
        /// </summary>
        [Prop(Required: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.add)]
        [ValidString(minlen: 2, maxlen: 200, regexp: ValidStringAttribute.REGEXP_EMAIL)]
        public string email { get; set; }

        /// <summary>
        /// Especifica si la cuenta se encuentra inactiva
        /// </summary>
        [Prop(Required: false, OrderIndex: 40, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.addupdate)]
        public bool Inactiva { get; set; }


        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string name { get; set; }

        [Prop(Required: false, OrderIndex: 60)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string family_name { get; set; }

        [Prop(Required: false, OrderIndex: 70)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string given_name { get; set; }

        [Prop(Required: false, OrderIndex: 80)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.add)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string middle_name { get; set; }

        [Prop(Required: false, OrderIndex: 90)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string nickname { get; set; }

        [Prop(Required: false, OrderIndex: 100)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Genero", DatosRemotos: true, TypeAhead: false)]
        public string generoid { get; set; }

        /// <summary>
        ///  Propeidades de la aplciación
        /// </summary>
        [Prop(Required: false, OrderIndex: 110)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Pais", DatosRemotos: true, TypeAhead: false, Default: "MEX")]
        [JsonPropertyName("PaisId")]
        public string paisid { get; set; }

        [Prop(Required: false, OrderIndex: 120)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "Estado", DatosRemotos: true, TypeAhead: false)]
        [Event(Entidad: "PaisId", Evento: Eventos.AlCambiar, Operacion: Operaciones.Actualizar, "PaisId")]
        [JsonPropertyName("EstadoId")]
        public string estadoid { get; set; }

     

        /// <summary>
        /// Especifica si la cuenta ha sido marcada para eliminar
        /// </summary>
        public bool Eliminada { get; set; }


        public string gmt { get; set; }


       public float? gmt_offset { get; set; }

        public DateTime? updated_at { get; set; }

        public bool? email_verified { get; set; }


        /// <summary>
        /// Esta propiedad se alamcena y debe obtenerse de la tabla de los claims del usuario 
        /// No debe incluirse en consultas debido a que puede degradar el performance
        /// </summary>
        [NotMapped]

        public string picture { get; set; }





        /// <summary>
        ///  Esta propeida se actualzai vía el login del usuario
        /// </summary>
        public DateTime? Ultimoacceso { get; set; }

        //public Pais pais { get; set; }
        //public Estado estado { get; set; }

        public Genero genero { get; set; }

        public ApplicationUser Usuario { get; set; }


    }
}
