using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Constantes
{
    public class ConstantesAplicacion
    {
        /// <summary>
        /// Identificador único de la aplciacción
        /// </summary>
        public const string Id = "PIKA-GD";
        /// <summary>
        /// Version actual de la aplciación
        /// </summary>
        /// 
        public const string Version = "1.0";
        
        /// <summary>
        /// Obtiene la instancia base para la creación de aplicaciones PIKA-GD
        /// </summary>
        /// <returns></returns>
        public static Aplicacion AplicacionPikaGD()
        {
            Aplicacion a = new Aplicacion() { Id = ConstantesAplicacion.Id, 
                Descripcion="Pika Gestión documental ofrece las funcoines necesarias para el cotnrol del ciclo de vida documental", 
                Nombre ="PIKA Gestión documental", 
                UICulture = "ex-MX", 
                Version =ConstantesAplicacion.Version, 
                ReleaseIndex =0 };
            return a;

        }




    }
}
