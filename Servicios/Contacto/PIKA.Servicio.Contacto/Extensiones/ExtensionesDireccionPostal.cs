using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public static partial class Extensiones
    {

        public static DireccionPostal Copia(this DireccionPostal d)
        {
            if (d == null) return null;

            DireccionPostal dp =
            new DireccionPostal()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                EstadoId = d.EstadoId,
                Municipio = d.Municipio,
                CP = d.CP,
                Calle = d.Calle,
                Colonia = d.Colonia,
                NoExterno = d.NoExterno,
                NoInterno = d.NoInterno,
                PaisId = d.PaisId,
                OrigenId = d.OrigenId,
                TipoOrigenId = d.TipoOrigenId
            };

            if (d.Estado != null)
            {
                dp.Estado = d.Estado.Copia();
            }

            if (d.Pais != null)
            {
                dp.Pais = d.Pais.Copia();
            }

            return dp;
        }

       
    }
}
