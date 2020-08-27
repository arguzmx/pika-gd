using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static AsociacionPlantilla Copia(this AsociacionPlantilla d)
        {
            if (d == null) return null;
            return new AsociacionPlantilla()
            {
                Id = d.Id,
                IdentificadorAlmacenamiento = d.IdentificadorAlmacenamiento,
                OrigenId = d.OrigenId,
                PlantillaId = d.PlantillaId,
                TipoOrigenId = d.TipoOrigenId,
                
            };
        }
    }
}
