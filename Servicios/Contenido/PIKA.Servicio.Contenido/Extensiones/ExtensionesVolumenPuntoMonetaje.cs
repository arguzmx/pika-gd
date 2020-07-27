using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static VolumenPuntoMontaje Copia(this VolumenPuntoMontaje d)
        {
            if (d == null) return null;
            var o= new  VolumenPuntoMontaje()
            {
                 PuntoMontajeId = d.PuntoMontajeId,
                  VolumenId = d.VolumenId 
            };

            if (d.Volumen != null)
            {
                o.Volumen = d.Volumen.Copia();
            }

            return o;
        }
    }
}
