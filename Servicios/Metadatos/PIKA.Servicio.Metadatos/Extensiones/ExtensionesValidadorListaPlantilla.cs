﻿using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Metadatos;

namespace PIKA.Servicio.Metadatos
{
    public static partial class Extensiones
    {
        public static ValorListaPlantilla Copia(this ValorListaPlantilla d)
        {
            if (d == null) return null;
            return new ValorListaPlantilla()
            {
                Id = d.Id,
                PropiedadId=d.PropiedadId,
                Texto=d.Texto,
                Indice=d.Indice
                
            };
        }
    }
}


