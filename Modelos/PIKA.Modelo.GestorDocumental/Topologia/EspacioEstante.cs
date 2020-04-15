using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Topologia
{
    public class EspacioEstante : Entidad<string>, IEntidadNombrada, IEntidadIdElectronico
    {
        public override string Id { get; set; }

        /// <summary>
        /// Nombre comun del espacio del rack para uso humano
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Codigo de barras para el espacio  del Estante
        /// </summary>
        public string CodigoOptico { get; set; }
        //#Longutod 2028, opcional

        /// <summary>
        /// Código electrónico para el espacio del  Estante por ejemplo de RFID
        /// </summary>
        public string CodigoElectronico { get; set; }
        //#Longutod 2028, opcional

        /// <summary>
        /// Identificador único del estante al que perenece el espacio
        /// </summary>
        public string EstanteId { get; set; }

        /// <summary>
        /// Posición relativa del espacio en relación a los demas espacios del estante
        /// </summary>
        public int Posicion { get; set; }

        public virtual Estante Estante { get; set; }

    }
}
