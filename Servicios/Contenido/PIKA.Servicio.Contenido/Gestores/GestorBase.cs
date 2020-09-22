using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PIKA.Servicio.Contenido.Gestores
{
    public abstract class GestorBase
    {
        private List<string> ExtensionesImagenes()
        {
            return new List<string>()
            { "jpg", "jpeg", "png", "tiff", "tif", "bpm", "gif" };
        }

        public bool EsImagen(FileInfo fi)
        {
            if (this.ExtensionesImagenes().IndexOf(fi.Extension.TrimStart('.').ToLower()) >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
