using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static Plantilla Copia(this Plantilla d)
        {
            if (d == null) return null;
            Plantilla p = new Plantilla()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId,
                AlmacenDatosId=d.AlmacenDatosId
                
            };

            foreach (PropiedadPlantilla pp in d.Propiedades)
            {
                p.Propiedades.Add(pp.Copia());

            }

            return p;
        }
    }
}
