using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{

    public enum ArchivoTipo
    {
        otro = 0, tramite =1, concentracion=2, historico=3
    }

    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
        TokenMod: ConstantesAppGestionDocumental.MODULO_CAT_ARCHIVO,
        TokenApp: ConstantesAppGestionDocumental.APP_ID
        )]
    public class TipoArchivo : EntidadCatalogo<string, TipoArchivo> 
    {
        public const string IDARCHIVO_TRAMITE = "a-tra";
        public const string IDARCHIVO_CONCENTRACION = "a-con";
        public const string IDARCHIVO_HISTORICO = "a-his";
        public const string IDARCHIVO_CORRESPONDENCIA = "a-cor";
        public TipoArchivo()
        {
            Archivos = new HashSet<Archivo>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, isId: false, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        [Prop(Required: true, isId: false, Visible: true, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_SELECT, Accion: Acciones.addupdate)]
        [List(Entidad: "", DatosRemotos: false, TypeAhead: false, ValoresCSV: "1|entidades.propiedades.tipoarchivo.enum.t,2|entidades.propiedades.tipoarchivo.enum.c,3|entidades.propiedades.tipoarchivo.enum.h")]
        public ArchivoTipo? Tipo { get; set; }


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

        public override List<TipoArchivo> Seed()
        {
            List<TipoArchivo> l = new List<TipoArchivo>();
            // l.Add(new TipoArchivo() { Id = IDARCHIVO_CORRESPONDENCIA, Nombre = "Correspondencoia"});
            l.Add(new TipoArchivo() { Id = IDARCHIVO_TRAMITE, Nombre = "Trámite", Tipo = ArchivoTipo.tramite });
            l.Add(new TipoArchivo() { Id = IDARCHIVO_HISTORICO  , Nombre = "Histórico", Tipo = ArchivoTipo.historico});
            l.Add(new TipoArchivo() { Id = IDARCHIVO_CONCENTRACION , Nombre = "Concentración", Tipo = ArchivoTipo.concentracion });
            return l;
        }

      

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<Archivo> Archivos { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public IEnumerable<Activo> Activos { get; set; }

    }
}
