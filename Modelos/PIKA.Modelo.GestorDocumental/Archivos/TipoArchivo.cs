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


    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
        TokenMod: ConstantesAppGestionDocumental.MODULO_CAT_ARCHIVO,
        TokenApp: ConstantesAppGestionDocumental.APP_ID
        )]
    public class TipoArchivo : EntidadCatalogo<string, TipoArchivo> 
    {
        public const string IDARCHIVO_TRAMITE = "a-tra";
        public const string IDARCHIVO_CONSERVACION = "a-con";
        public const string IDARCHIVO_HISTORICO = "a-his";
        public const string IDARCHIVO_CORRESPONDENCIA = "a-cor";
        public TipoArchivo()
        {
            Archivos = new HashSet<Archivo>();
        }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        public override List<TipoArchivo> Seed()
        {
            List<TipoArchivo> l = new List<TipoArchivo>();
            // l.Add(new TipoArchivo() { Id = IDARCHIVO_CORRESPONDENCIA, Nombre = "Correspondencoia"});
            l.Add(new TipoArchivo() { Id = IDARCHIVO_TRAMITE, Nombre = "Trámite" });
            l.Add(new TipoArchivo() { Id = IDARCHIVO_HISTORICO  , Nombre = "Histórico"});
            l.Add(new TipoArchivo() { Id = IDARCHIVO_CONSERVACION , Nombre = "Concentración"});
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
