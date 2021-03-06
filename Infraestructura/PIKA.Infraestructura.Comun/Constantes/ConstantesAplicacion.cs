﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Constantes
{
    public class ConstantesAplicacion
    {
        /// <summary>
        /// Identificador único de la aplciacción
        /// </summary>
        public const string IdAppGlobal = "PIKA-GD";
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
            Aplicacion a = new Aplicacion() { Id = ConstantesAplicacion.IdAppGlobal, 
                Descripcion="Pika Gestión documental ofrece las funciones necesarias para el cotnrol del ciclo de vida documental", 
                Nombre ="PIKA Gestión documental", 
                UICulture = "es-MX", 
                Version =ConstantesAplicacion.Version, 
                ReleaseIndex =0 };
            return a;

        }




    }
}
