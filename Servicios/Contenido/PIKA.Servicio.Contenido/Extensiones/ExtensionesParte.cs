using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {
        public static Parte Copia(this Parte d)
        {
            if (d == null) return null;
            return new Parte()
            {
                ElementoId = d.ElementoId,
                VersionId = d.VersionId,
                ConsecutivoVolumen = d.ConsecutivoVolumen,
                Eliminada = d.Eliminada,
                Indice = d.Indice,
                LongitudBytes = d.LongitudBytes,
                NombreOriginal = d.NombreOriginal,
                TipoMime = d.TipoMime, 
                Id = d.Id
            };
        }
    }
}
