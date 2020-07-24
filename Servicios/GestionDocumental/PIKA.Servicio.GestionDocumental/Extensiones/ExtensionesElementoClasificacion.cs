using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ElementoClasificacion Copia(this ElementoClasificacion d)
        {
            if (d == null) return null;
            var e= new ElementoClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Eliminada = d.Eliminada,
                CuadroClasifiacionId=d.CuadroClasifiacionId,
                ElementoClasificacionId=d.ElementoClasificacionId,
                Activos=d.Activos,
                Clave=d.Clave,
                Posicion=d.Posicion
            };
            if (d.Padre != null) 
            {
                e.Padre = d.Padre.Copia();
            }
            if (d.CuadroClasificacion != null) 
            {
                e.CuadroClasificacion = d.CuadroClasificacion.Copia();
            }
            return e;
        }
    }
}
