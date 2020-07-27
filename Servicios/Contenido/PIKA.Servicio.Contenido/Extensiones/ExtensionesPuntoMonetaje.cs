using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static PuntoMontaje Copia(this PuntoMontaje d)
        {
            if (d == null) return null;
            var o = new PuntoMontaje()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                CreadorId = d.CreadorId,
                Eliminada = d.Eliminada,
                FechaCreacion = d.FechaCreacion,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId,
                VolumenDefaultId = d.VolumenDefaultId
            };

            if (d.VolumenDefault!=null)
            {
                o.VolumenDefault = d.VolumenDefault.Copia();
            }

            if (d.VolumenesPuntoMontaje != null)
            {
                foreach(var v in d.VolumenesPuntoMontaje)
                {
                    o.VolumenesPuntoMontaje.Add(v.Copia());
                }
            }

            return o;
        }
    }
}
