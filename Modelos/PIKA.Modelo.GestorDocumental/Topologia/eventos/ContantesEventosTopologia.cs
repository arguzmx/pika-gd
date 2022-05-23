using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.eventos
{
    public enum TipEventoContenedorAlmacen
    {
        /// <summary>
        /// Colocar el contenedor en una nueva posición
        /// </summary>
        Mover =1, 
        /// <summary>
        /// Añadir un activo al contenedor
        /// </summary>
        InsertarActivo=10, 
        /// <summary>
        /// Extraer un activo del contenedor
        /// </summary>
        ExtraerActivo=11
    }

}