using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace PIKA.Modelo.Seguridad.Base
{
    public class UsuarioDominio
    {

        /// <summary>
        /// Especifica si el usuario e administardor del dominio
        /// </summary>
        public bool EsAdmin { get; set; }

        [NotMapped]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        /// <summary>
        ///  Identificador único del tipo de origen, en el caso de los usuarios
        ///  Corresponde al dominio
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificaodrúnico del dominio
        /// </summary>
        public string OrigenId { get; set; }

        public string ApplicationUserId { get; set; }

        public ApplicationUser ApplicationUser { get; set; }

    }
}
