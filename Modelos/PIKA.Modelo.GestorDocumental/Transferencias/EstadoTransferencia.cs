using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

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

        /// <summary>
        /// Indica que los activos han sido aprobados para su envío
        /// </summary>
        public const string ESTADO_APROBADA = "aprobada";


        /// <summary>
        /// Indica que la trasnfernecia se encuentra en tránsito hacia su destino
        /// </summary>
        public const string ESTADO_EN_TRANSITO = "transito";

        /// <summary>
        /// Indica que la transferencia ha sido recibida satisfactoeiamente por el archiv destino
        /// </summary>
        public const string ESTADO_RECIBIDA = "recibida";

        /// <summary>
        /// Indica que la transferencia ha sido recibida satisfactoeiamente pero algunos activos fueron declinados
        /// </summary>
        public const string ESTADO_RECIBIDA_PARCIAL = "recibida_parcial";

        /// <summary>
        /// Indica que la transferencia ha sido canelada en alguna de las etapas
        /// </summary>
        public const string ESTADO_CANCELADA = "cancelada";

        /// <summary>
        /// Indica que la transferencia ha sido declinada por el receptor y reenviada al orígen
        /// </summary>
        public const string ESTADO_DECLINADA = "declinada";

        public EstadoTransferencia()
        {
            Transferencias = new HashSet<Transferencia>();

        }

        /// <summary>
        /// Propiedad de navegacion
        /// </summary>
        public ICollection<Transferencia> Transferencias { get; set; }

        public override List<EstadoTransferencia> Seed()
        {
            ///Implementtar este método

            throw new NotImplementedException();

        }

    }
}
