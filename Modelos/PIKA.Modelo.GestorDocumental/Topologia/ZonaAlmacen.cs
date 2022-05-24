using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
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

    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
       TokenMod: ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO,
       TokenApp: ConstantesAppGestionDocumental.APP_ID)]
    public class ZonaAlmacen : Entidad<string>, IEntidadNombrada
    {

        public ZonaAlmacen()
        {
            Posiciones = new List<PosicionAlmacen>();
            Contenedores = new List<ContenedorAlmacen>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre único del almacén
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }

        /// <summary>
        /// Clave asociada al arhchvi en la organización
        /// </summary>
        [Prop(Required: true, OrderIndex: 20)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 200)]
        public string Clave { get; set; }

        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        /// <summary>
        /// Identificaodr único del archivo al qu pertenece la zona
        /// </summary>
        public string ArchivoId { get; set; }

        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "AlmacenArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "AlmacenArchivo", DatosRemotos: true, TypeAhead: false)]
        /// <summary>
        /// Identificador unico del almacen al que pertenece la zona
        /// </summary>
        public string AlmacenArchivoId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public AlmacenArchivo Almacen { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public Archivo Archivo { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<PosicionAlmacen> Posiciones { get; set; }

        [XmlIgnore]
        [JsonIgnore]
        public List<ContenedorAlmacen> Contenedores { get; set; }
    }
}
