using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: false, 
        TokenMod: ConstantesAppGestionDocumental.MODULO_CATALOGOSCC, 
        TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    public class TipoAmpliacion: EntidadCatalogo<string, TipoAmpliacion>
    {
        public TipoAmpliacion() {
            this.Ampliaciones = new HashSet<Ampliacion>();
        }
        public const string RESERVA = "reserva";
        public const string CONFIDENCIALIDAD = "confidencialidad";
        public const string SOLICITUD_INFORMACION = "solicitud-informacion";

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        /// <summary>
        /// Identificaor único del dominio al que pertenece el catáloco
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, OrderIndex: 200, Contextual: true, ShowInTable: false, IdContextual: ConstantesModelo.GLOBAL_DOMINIOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string DominioId { get; set; }

        /// <summary>
        /// Identificaor único de la unidad  organizacional al que pertenece el catáloco
        /// </summary>
        [Prop(Required: false, isId: false, Visible: false, OrderIndex: 200, Contextual: true, ShowInTable: false, IdContextual: ConstantesModelo.GLOBAL_UOID)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        public string UOId { get; set; }


        public override List<TipoAmpliacion> Seed()
        {
            List<TipoAmpliacion> lista = new List<TipoAmpliacion>();
            lista.Add(new TipoAmpliacion() { Id = RESERVA, Nombre = "Reserva" });
            lista.Add(new TipoAmpliacion() { Id = CONFIDENCIALIDAD, Nombre = "Confidencialidad" });
            lista.Add(new TipoAmpliacion() { Id = SOLICITUD_INFORMACION, Nombre = "Solicitud de información" });
            return lista;
        }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<Ampliacion> Ampliaciones { get; set; }
    }
}
