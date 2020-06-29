using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static PIKA.Modelo.Metadatos.PropAttribute;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// El dominio es el contenedor todos los recursos asociados a una organización
    /// </summary>
    [Entidad(EliminarLogico: true)]
    [EntidadVinculada(Entidad: "UnidadOrganizacional", Cardinalidad: TipoCardinalidad.UnoVarios, Padre: "Id", Hijo: "DominioId")]
    public class Dominio : Entidad<string>, IEntidadNombrada, IEntidadRelacionada, IEntidadEliminada
    {
        
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_GLOBAL;

        public Dominio() {
            UnidadesOrganizacionales = new HashSet<UnidadOrganizacional>();
            this.TipoOrigenId = this.TipoOrigenDefault;
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0 )]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        [Prop(Required: true, OrderIndex: 1)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString( minlen: 2, maxlen: 200 )]
                /// <summary>
        /// NOmbre de la unodad organizacional
        /// </summary>
        public string Nombre { get; set; }


        [Prop(OrderIndex: 2, DefaultValue: "false")]
        [VistaUI(ControlUI: ControlUI.HTML_TOGGLE, Accion: Acciones.update)]
        /// <summary>
        /// Especifica si el dominio ha sido eliminado de manera lógica
        /// </summary>
        public bool Eliminada { get; set; }


        [Prop(Required: true, OrderIndex: 3, Searchable: false, DefaultValue: ConstantesModelo.IDORIGEN_GLOBAL, ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        /// <summary>
        /// Identificador de relación de origem, en este caso se utiliza
        /// para vincular la unidad con su dueño en el modo MultiTenant 
        /// </summary>
        public string TipoOrigenId { get; set; }


        [Prop(Required: true, OrderIndex: 4, Searchable:false, DefaultValue: ConstantesModelo.IDORIGEN_GLOBAL,  ShowInTable: false)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        /// <summary>
        /// Identficador único del dueño, TENANT
        /// El Id de ralción es el identificador de un dominio
        /// </summary>
        public string OrigenId { get; set; }

               
        /// <summary>
        /// Propiedad de navegación para unidades organizacionales
        /// </summary>
        public ICollection<UnidadOrganizacional> UnidadesOrganizacionales { get; set; }
        
    }
}
