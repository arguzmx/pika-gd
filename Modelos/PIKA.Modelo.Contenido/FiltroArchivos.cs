using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public static class FiltroArchivos
    {
        static FiltroArchivos()
        {
            _extensiones = new List<string>();
        }
        public static long minimo { get; set; }
        public static long maximo { get; set; }

        private static List<string> _extensiones;

        public static List<string> extensionesValidas
        {
            get { return _extensiones; }
        }

        public static void addExt(string ext)
        {
            _extensiones.Add(ext);
        }

        public static Dictionary<string, List<byte[]>> firmasArchivo
        {
            get { return _fileSignature; }
        }
        private static readonly Dictionary<string, List<byte[]>> _fileSignature = 
            new Dictionary<string, List<byte[]>>
            {
                #region Documentos (pdf,word,excel,pp)
                {".pdf", new List<byte[]>
                    {
                        new byte[] { 0x25, 0x50, 0x44, 0x46}
                    }
                },
                {".doc", new List<byte[]>
                    {
                        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1}
                    }
                },
                {".docx", new List<byte[]>
                    {
                        new byte[] { 0x50, 0x4B, 0x03, 0x04},
                        new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                    }
                },
                 {".xls", new List<byte[]>
                    {
                        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 }
                    }
                },
                 {".xlsx", new List<byte[]>
                    {
                        new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                        new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                    }
                },
                 {".ppt", new List<byte[]>
                    {
                        new byte[] { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1  },
                    }
                },
                   {".pptx", new List<byte[]>
                    {
                        new byte[] { 0x50, 0x4B, 0x03, 0x04 },
                        new byte[] { 0x50, 0x4B, 0x03, 0x04, 0x14, 0x00, 0x06, 0x00 }
                    }
                },
            #endregion

                #region Imágenes
                {".jpeg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE2 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE3 },
                    }
                },
                {".jpg", new List<byte[]>
                    {
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE0 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE1 },
                        new byte[] { 0xFF, 0xD8, 0xFF, 0xE8 }
                    }
                },
                {".png", new List<byte[]>
                    {
                        new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }
                    }
                },
                #endregion

                #region Audio
                {".wma", new List<byte[]>
                    {
                        new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 },
                    }
                },
                {".mp3", new List<byte[]>
                    {
                        new byte[] { 0x49, 0x44, 0x33},
                    }
                },
                {".midi", new List<byte[]>
                    {
                        new byte[] { 0x4D, 0x54, 0x68, 0x64 }
                    }
                },
                {".wav", new List<byte[]>
                    {
                        new byte[] { 0x52, 0x49, 0x46, 0x46 }
                    }
                },
                {".flac", new List<byte[]>
                    {
                        new byte[] { 0x66, 0x4C, 0x61, 0x43, 0x00, 0x00, 0x00, 0x22 }
                    }
                },
                {".ogg", new List<byte[]>
                    {
                        new byte[] { 0x4F, 0x67, 0x67, 0x53, 0x00, 0x02, 0x00, 0x00 }
                    }
                },
            #endregion

                #region Video
                {".avi", new List<byte[]>
                    {
                        new byte[] { 0x52, 0x49, 0x46, 0x46 },
                    }
                },
                {".wmv", new List<byte[]>
                    {
                        new byte[] { 0x30, 0x26, 0xB2, 0x75, 0x8E, 0x66, 0xCF, 0x11 }
                    }
                },
                //{".mov", new List<byte[]>
                //    {
                //        new byte[] { 0x6D, 0x6F, 0x6F, 0x76},
                //    }
                //},  
                // ********** NO FUNCIONA ***************
                {".mov", new List<byte[]>
                    {
                        new byte[] { 0x73, 0x6B, 0x69, 0x70},
                    }
                },
            #endregion

                #region Archivos comprimidos
                 {".zip", new List<byte[]>
                    {
                        new byte[] { 0x57, 0x69, 0x6E, 0x5A, 0x69, 0x70 },
                    }
                },
                {".rar", new List<byte[]>
                    {
                        new byte[] { 0x52, 0x61, 0x72, 0x21, 0x1A, 0x07, 0x00 },
                    }
                },
	            #endregion
            };

    }
}

