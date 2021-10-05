using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.Text;

namespace PikaOCR
{
    public static class Extensiones
    {
   
        public static ContenidoTextoCompleto ParteAContenidoTextoCompleto(this Parte Parte, string Texto, string PuntoMontajeId = "", string CarpetaId="", int Pagina = 1)
        {
            return new ContenidoTextoCompleto()
            {
                Texto = Texto,
                ElementoId = Parte.ElementoId,
                Eliminado = false,
                Activo = true,
                ParteId = Parte.Id,
                Pagina = Pagina,
                VolumenId = Parte.VolumenId,
                CarpetaId = CarpetaId,
                PuntoMontajeId = PuntoMontajeId,
                VersionId = Parte.VersionId,
                DocumentoId = Parte.ElementoId 
            };
        }

    }
}
