using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public static class ExtensionesCargaContenido
    {
        public static ElementoTransaccionCarga ConvierteETC(this ElementoCargaContenido el) {

            ElementoTransaccionCarga etc = new ElementoTransaccionCarga()
            {
                ElementoId = el.ElementoId,
                Indice = el.Indice,
                NombreOriginal = el.file.FileName,
                PuntoMontajeId = el.PuntoMontajeId,
                TamanoBytes = el.file.Length,
                TransaccionId = el.TransaccionId,
                VolumenId = el.VolumenId,
                Procesado = false,
                Error = false,
                FechaCarga = DateTime.UtcNow,
                VersionId = el.VersionId
            };
            return etc;

        }

        public static Parte ConvierteParte(this ElementoTransaccionCarga e)
        {

            List<string> imagenes = new List<string>() { ".jpg", ".jpeg", ".png", ".gif", ".bpm", ".tif", ".tiff", ".jfif" };
            List<string> videos = new List<string>() { ".mp4", ".ogg", ".webm" };
            List<string> audios = new List<string>() { ".mp3", ".ogg", ".wav" };
            List<string> pdfs = new List<string>() { ".pdf"};

            bool esImagen = imagenes.IndexOf(Path.GetExtension(e.NombreOriginal).ToLower()) >= 0;
            bool esVideo = videos.IndexOf(Path.GetExtension(e.NombreOriginal).ToLower()) >= 0;
            bool esAudio = audios.IndexOf(Path.GetExtension(e.NombreOriginal).ToLower()) >= 0;
            bool esPDF = pdfs.IndexOf(Path.GetExtension(e.NombreOriginal).ToLower()) >= 0;
            string mimeTyoe = MimeTypes.GetMimeType(e.NombreOriginal);


            Parte p = new Parte()
            {
                ConsecutivoVolumen = 0,
                ElementoId = e.ElementoId,
                Eliminada = false,
                EsImagen = esImagen,
                EsAudio = esAudio,
                EsVideo = esVideo,
                EsPDF = esPDF,
                Indexada = false,
                Indice = 0,
                LongitudBytes = e.TamanoBytes,
                NombreOriginal = e.NombreOriginal,
                TieneMiniatura = false,
                Extension = Path.GetExtension(e.NombreOriginal),
                TipoMime = mimeTyoe,
                VersionId = e.VersionId,
                VolumenId = e.VolumenId,
                Id = e.Id
            };
            return p;

        }
    }
}
