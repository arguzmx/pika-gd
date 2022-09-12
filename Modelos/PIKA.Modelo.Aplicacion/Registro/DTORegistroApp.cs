using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Aplicacion.Registro
{
    public class DTORegistroApp
    {
        /// <summary>
        /// Nmbre de la empresa a las que se registra el software
        /// </summary>
        public string NombreEmpresa { get; set; }

        /// <summary>
        /// Nombre de la persona que relia el registro
        /// </summary>
        public string NombreContacto { get; set; }

        /// <summary>
        /// Correo de activación
        /// </summary>
        public string Email { get; set; }
        
        /// <summary>
        /// Huella digital del equipo para el registro
        /// </summary>
        public string HuellaDigital { get; set; }
    }
}
