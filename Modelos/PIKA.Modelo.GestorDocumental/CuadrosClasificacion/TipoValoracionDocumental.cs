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
    public class TipoValoracionDocumental: EntidadCatalogo<string, TipoValoracionDocumental>
    {
        public const string ADMINISTRATIVA = "admin";
        public const string LEGAL = "legal";
        public const string FISCAL = "fiscal";
        //public const string TESTIMONIAL = "testimonial";
        //public const string EVIDENCIAL = "evidencial";
        //public const string INFORMATIVA = "evidencial";


        public TipoValoracionDocumental()
        {
            ValoracionEntradas = new HashSet<ValoracionEntradaClasificacion>();
        }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Id { get => base.Id; set => base.Id = value; }

        [Prop(Required: true, isId: true, Visible: true, OrderIndex: 10)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        public override List<TipoValoracionDocumental> Seed()
        {
            List<TipoValoracionDocumental> lista = new List<TipoValoracionDocumental>();

            lista.Add(new TipoValoracionDocumental() { Id = ADMINISTRATIVA, Nombre = "Administrativa" });
            lista.Add(new TipoValoracionDocumental() { Id = LEGAL, Nombre = "Legal" });
            lista.Add(new TipoValoracionDocumental() { Id = FISCAL, Nombre = "Fiscal" });
            //lista.Add(new TipoValoracionDocumental() { Id = TESTIMONIAL, Nombre = "Testimonial" });
            //lista.Add(new TipoValoracionDocumental() { Id = EVIDENCIAL, Nombre = "Evidencial" });
            //lista.Add(new TipoValoracionDocumental() { Id = INFORMATIVA, Nombre = "Informativa" });
            



            return lista;
        }

        [XmlIgnore]
        [JsonIgnore]
        public ICollection<ValoracionEntradaClasificacion> ValoracionEntradas { get; set; }
    }
}
