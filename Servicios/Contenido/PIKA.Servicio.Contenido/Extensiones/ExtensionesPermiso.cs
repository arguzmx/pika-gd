using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static Permiso Copia(this Permiso d)
        {
            if (d == null) return null;
            var  o = new Permiso()
            {
                Id = d.Id,
                Leer = d.Leer,
                Escribir = d.Escribir,
                Crear = d.Crear,
                Eliminar = d.Eliminar,
            };
            
            if (d.Destinatarios != null)
            {
                foreach(DestinatarioPermiso dest in d.Destinatarios)
                {
                    o.Destinatarios.Add(dest.Copia());
                }
            }

            if (d.Elementos != null)
            {
                // No creo que vayamos a devolver esto nunca
            }


            if (d.Carpetas != null)
            {
                // No creo que vayamos a devolver esto nunca
            }

            return o;
        }
    }
}
