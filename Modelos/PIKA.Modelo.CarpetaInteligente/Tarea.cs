using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public class Tarea: Entidad<Guid>
    {
        public Tarea()
        {
            Ejecutores = new List<Persona>();
        }

        /// <summary>
        /// Identificador único de la tarea
        /// </summary>
        public override Guid Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Nombre de la tarea
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Descripción  de la tarea
        /// </summary>
        public string Descripcion { get; set; }


        /// <summary>
        /// Establece la configuración de ejecución de la tarea
        /// </summary>
        public ConfiguracionEjecucion ConfiguracionEjecucion { get; set; }

        /// <summary>
        /// Determina si la tarea debe ser verificada
        /// </summary>
        public bool RequiereVerificacion { get; set; }


        /// <summary>
        /// Establece la configuración de verificación de la tarea
        /// </summary>
        public ConfiguracionEjecucion ConfiguracionVerificacion { get; set; }



        public List<Persona> Ejecutores { get; set; }



    }
}
