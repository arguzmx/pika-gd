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

    [Entidad(PaginadoRelacional: false, EliminarLogico: true,
       TokenMod: ConstantesAppGestionDocumental.MODULO_ALMACENARCHIVO,
       TokenApp: ConstantesAppGestionDocumental.APP_ID)]

    public class PosicionAlmacen : Entidad<string>, IEntidadNombrada
    {
        public PosicionAlmacen()
        {
            Posiciones = new List<PosicionAlmacen>();
            Contenedores = new List<ContenedorAlmacen>();
            IncrementoContenedor = 0;
            Ocupacion = 0;
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre único de la posición de almacé
        /// </summary>
        [Prop(Required: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public string Nombre { get; set; }


        [Prop(Required: true, OrderIndex: 30)]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min:0, usemin:true, defaulvalue: 0)]
        /// <summary>
        /// Indice de la posicion en la osición padre
        /// </summary>
        public int Indice { get; set; }

        /// <summary>
        /// Nombre único de la posición de almacé
        /// </summary>
        [Prop(Required: false, OrderIndex: 35)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXTAREA, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 512)]
        public string Localizacion { get; set; }


        [Prop(Required: false, OrderIndex: 40)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 512)]
        /// <summary>
        /// Codigo de barras asociado
        /// </summary>
        public string CodigoBarras { get; set; }

        [Prop(Required: false, OrderIndex: 50)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 0, maxlen: 512)]
        /// <summary>
        /// Codigo electrónico asociado
        /// </summary>
        public string CodigoElectronico { get; set; }

        [Prop(Required: true, OrderIndex: 35, DefaultValue: "0")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, usemin: true, defaulvalue: 0, max:100, usemax:true)]
        /// <summary>
        /// Porcentaje de ocupación de la posición
        /// </summary>
        public decimal Ocupacion { get; set; }


        [Prop(Required: true, OrderIndex: 36, DefaultValue: "0")]
        [VistaUI(ControlUI: ControlUI.HTML_NUMBER, Accion: Acciones.addupdate)]
        [ValidNumeric(min: 0, usemin: true, defaulvalue: 0, max: 100, usemax: true)]
        /// <summary>
        /// Incremento automático de porcentaje de ocupación de la posición al adiconar o remover un contenedor
        /// </summary>
        public decimal IncrementoContenedor { get; set; }


        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "Archivo", DatosRemotos: true, TypeAhead: false)]
        /// <summary>
        /// Identificaodr único del archivo al qu pertenece la posición
        /// </summary>
        public string ArchivoId { get; set; }

        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "AlmacenArchivoId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "AlmacenArchivo", DatosRemotos: true, TypeAhead: false)]
        /// <summary>
        /// Identificador unico del almacen al que pertenece la posición
        /// </summary>
        public string AlmacenArchivoId { get; set; }


        [Prop(Required: false, OrderIndex: 600, Visible: false, Contextual: true, IdContextual: ConstantesModelo.PREFIJO_CONEXTO + "ZonaAlmacenId")]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.addupdate)]
        [List(Entidad: "ZonaAlmacen", DatosRemotos: true, TypeAhead: false)]
        /// <summary>
        /// Identificador unico de la zona a la que pertenece la posición
        /// </summary>
        public string ZonaAlmacenId { get; set; }



        [XmlIgnore]
        [JsonIgnore]
        /// <summary>
        /// Identificador unico de la posición padre para el elemento
        /// NO APLICA, PARA FACILITAR EL MANTENIMIENTO LOS POSICIONES HIJAS DE UN ALMACEN SE BASAN EN EL CAMPO INDICE
        /// </summary>
        public string PosicionPadreId { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public PosicionAlmacen PosicionPadre { get; set; }


        [XmlIgnore]
        [JsonIgnore]
        public ZonaAlmacen Zona { get; set; }

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
