using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using PIKA.Constantes.Aplicaciones.GestorDocumental;

namespace PIKA.Modelo.GestorDocumental
{
    [Entidad(PaginadoRelacional: false, EliminarLogico: false,
     TokenMod: ConstantesAppGestionDocumental.MODULO_CATALOGOSCC,
     TokenApp: ConstantesAppGestionDocumental.APP_ID
     )]
    public class TipoDisposicionDocumental : EntidadCatalogo<string, TipoDisposicionDocumental>
    {
        public TipoDisposicionDocumental()
        {
            EntradaClasificacion = new HashSet<EntradaClasificacion>();
        }

        public const string SELECCION_COMPLETA = "seleccion-completa";
        public const string ELIMINACION_COMPLETA = "eliminacion-completa";
        public const string MUESTREO = "muestreo";
        public const string CAMBIO_SOPORTE = "cambio-soporte";


        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        public override List<TipoDisposicionDocumental> Seed()
        {
            List<TipoDisposicionDocumental> lista = new List<TipoDisposicionDocumental>();

            lista.Add(new TipoDisposicionDocumental() { Id = SELECCION_COMPLETA, Nombre = "Selección completa para histórico" });
            lista.Add(new TipoDisposicionDocumental() { Id = ELIMINACION_COMPLETA, Nombre = "Eliminación completa del acervo" });
            lista.Add(new TipoDisposicionDocumental() { Id = MUESTREO, Nombre = "Muestreo para conservación histórica" });
            lista.Add(new TipoDisposicionDocumental() { Id = CAMBIO_SOPORTE, Nombre = "Cambio de medio de soporte, microfilmación o digitalización" });

            return lista;
        }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<EntradaClasificacion> EntradaClasificacion { get; set; }
    }
}
