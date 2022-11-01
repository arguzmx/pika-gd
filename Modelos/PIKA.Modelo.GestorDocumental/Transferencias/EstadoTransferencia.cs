using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.GestorDocumental
{
    public class EstadoTransferencia: EntidadCatalogo<string, EstadoTransferencia>
    {
        /// <summary>
        /// Este es el estado inicial al crear una nueva trasnferencia
        /// </summary>
        public const string ESTADO_NUEVA = "nueva";

        /// <summary>
        /// Indica que los activos han sido incluidos y se encuentran es espera de aprobación
        /// </summary>
        public const string ESTADO_ESPERA_APROBACION = "espera";

        ///// <summary>
        ///// Indica que los activos han sido aprobados para su envío
        ///// </summary>
        //public const string ESTADO_APROBADA = "aprobada";


        ///// <summary>
        ///// Indica que la trasnfernecia se encuentra en tránsito hacia su destino
        ///// </summary>
        //public const string ESTADO_EN_TRANSITO = "transito";

        /// <summary>
        /// Indica que la transferencia ha sido recibida satisfactoriamente por el archiv destino
        /// </summary>
        public const string ESTADO_RECIBIDA = "recibida";

        /// <summary>
        /// Indica que la transferencia ha sido recibida satisfactoriamente pero algunos activos fueron declinados
        /// </summary>
        public const string ESTADO_RECIBIDA_PARCIAL = "recibida_parcial";

        ///// <summary>
        ///// Indica que la transferencia ha sido canelada en alguna de las etapas
        ///// </summary>
        //public const string ESTADO_CANCELADA = "cancelada";

        /// <summary>
        /// Indica que la transferencia ha sido declinada por el receptor y reenviada al orígen
        /// </summary>
        public const string ESTADO_DECLINADA = "declinada";

        public EstadoTransferencia()
        {
            Transferencias = new HashSet<Transferencia>();
            Eventos = new HashSet<EventoTransferencia>();
        }

        /// <summary>
        /// Propiedad de navegacion
        /// </summary>
        ///   [XmlIgnore]
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<Transferencia> Transferencias { get; set; }

        /// <summary>
        /// Prop de nav
        /// </summary>
        [XmlIgnore]
        [JsonIgnore]
        public virtual ICollection<EventoTransferencia> Eventos { get; set; }

        public override List<EstadoTransferencia> Seed()
        {
            List<EstadoTransferencia> l = new List<EstadoTransferencia>();
            l.Add(new EstadoTransferencia() { Id = ESTADO_NUEVA, Nombre = "En preparación"});
            l.Add(new EstadoTransferencia() { Id = ESTADO_ESPERA_APROBACION, Nombre = "Espera de aprobación"});
            //l.Add(new EstadoTransferencia() { Id = ESTADO_APROBADA, Nombre = "Aprobada"});
            //l.Add(new EstadoTransferencia() { Id = ESTADO_EN_TRANSITO, Nombre = "En tránsito"});
            l.Add(new EstadoTransferencia() { Id = ESTADO_RECIBIDA, Nombre = "Recibida" });
            l.Add(new EstadoTransferencia() { Id = ESTADO_RECIBIDA_PARCIAL, Nombre = "Recibida parcial"});
            //l.Add(new EstadoTransferencia() { Id = ESTADO_CANCELADA, Nombre = "Cancelada"});
            l.Add(new EstadoTransferencia() { Id = ESTADO_DECLINADA, Nombre = "Declinada"});

            return l;

        }

    }
}
