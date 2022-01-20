using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Infraestructura.Comun.Tareas
{
    public class TareaAutomatica: ProgramacionTarea, IEntidadNombrada, IEntidadRelacionada
    {

        public TareaAutomatica()
        {

        }

          /// <summary>
        /// NOmbre de la tarea
        /// </summary>
        public virtual string Nombre { get; set; }

        [NotMapped]
        public virtual string TipoOrigenDefault => ConstantesModelo.IDORIGEN_DOMINIO;

        /// <summary>
        /// El tipo de origen para las tareas programadas es por dominio
        /// </summary>
        public virtual string TipoOrigenId { get; set ; }

        /// <summary>
        /// Identificador único del dominio para la ejecución de tareas
        /// </summary>
        public virtual string OrigenId { get; set; }

        /// <summary>
        /// Nombre completo del ensamblado que proporciona la tarea
        /// </summary>
        public virtual string Ensamblado { get; set; }


 

    }
}
