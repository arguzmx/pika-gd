using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static CuadroClasificacion Copia(this CuadroClasificacion d)
        {
            if (d == null) return null;
            var c = new CuadroClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId,
                EstadoCuadroClasificacionId=d.EstadoCuadroClasificacionId,
                Eliminada=d.Eliminada
                
            };

            if (d.Estado != null)
            {
                c.Estado = new EstadoCuadroClasificacion() { Id = d.Estado.Id, Nombre = d.Estado.Nombre, Cuadros = null };
            }

            return c;
        }
    }
}
