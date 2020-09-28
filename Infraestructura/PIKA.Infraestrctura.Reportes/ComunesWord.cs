using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PIKA.Infraestrctura.Reportes
{
    public static class ComunesWord
    {
        public static byte[] UnirDocumentos(List<byte[]> ListaArchivos)
        {
            return CombinarDocumentos(ListaArchivos);
        }
        public static byte[] UnirDocumentos(List<string> ListaRutas)
        {
            byte[] f = { };
            List<byte[]> listafile = new List<byte[]>();
            foreach (string namefile in ListaRutas)
            {
                f = File.ReadAllBytes(namefile);
                listafile.Add(f);
            }
            return CombinarDocumentos(listafile);
        }
        public static byte[] CombinarDocumentos(List<byte[]> DocumentoFusionado)
        {
            int x = 0;
            byte[] fileOld = { };
            byte[] resultado = { };
            foreach (byte[] documentByteArray in DocumentoFusionado)
            {
                if (x == 0)
                { fileOld = documentByteArray; }
                else
                    using (MemoryStream stream = new MemoryStream())
                    {
                        stream.Write(documentByteArray, 0, (int)documentByteArray.Length);
                        using (WordprocessingDocument myDoc =
                    WordprocessingDocument.Open(stream, true))
                        {
                            string altChunkId = "AltChunkId1";
                            MainDocumentPart mainPart = myDoc.MainDocumentPart;
                            AlternativeFormatImportPart chunk =
                                mainPart.AddAlternativeFormatImportPart(
                                AlternativeFormatImportPartType.WordprocessingML, altChunkId);
                            using (MemoryStream file = new MemoryStream(fileOld))
                                chunk.FeedData(file);
                            AltChunk altChunk = new AltChunk();
                            altChunk.Id = altChunkId;
                            mainPart.Document
                                .Body
                                .InsertBefore(altChunk, mainPart.Document.Body
                                .Elements<Paragraph>().First());
                            mainPart.Document.Save();
                        }

                        fileOld = stream.ToArray();
                        resultado = stream.ToArray();
                    }
                x++;

            }
            return resultado;
        }
    }
}
