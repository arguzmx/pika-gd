using Microsoft.CodeAnalysis.CSharp.Syntax;
using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido.Extensiones
{
    public static class ExtensionesVisor
    {
        public static Pagina APagina(this Parte parte, string Id)
        {
            Pagina p = new Pagina()
            {
                Alto = 0,
                Ancho = 0,
                EsImagen = parte.EsImagen,
                Extension = parte.Extension,
                Id = !string.IsNullOrEmpty(parte.IdentificadorExterno) ? parte.IdentificadorExterno :  Id,  // POr ejemplo laserfiche
                Indice = parte.Indice,
                Nombre = parte.NombreOriginal,
                Rotacion = 0,
                TamanoBytes = parte.LongitudBytes,
                Url = "", // Son generadas en el cliente.
                UrlThumbnail = "",
                EsAudio = parte.EsAudio,
                EsPDF = parte.EsPDF,
                EsVideo = parte.EsVideo,
                Indexada = parte.Indexada,
                TieneMiniatura = parte.TieneMiniatura,
                VolumenId = parte.VolumenId
            };

            return p;
        }
    }
}
