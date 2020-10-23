using System.IO;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PIC = DocumentFormat.OpenXml.Drawing.Pictures;

namespace PIKA.Infraestrctura.Reportes
{
    public static class ImagenesWord
    {
        private static string AddGraph(WordprocessingDocument wpd, string filepath)
        {
            ImagePart ip = wpd.MainDocumentPart.AddImagePart(ImagePartType.Png);
            using (FileStream fs = new FileStream(filepath, FileMode.Open))
            {
                if (fs.Length == 0) return string.Empty;
                ip.FeedData(fs);
            }
            return wpd.MainDocumentPart.GetIdOfPart(ip);
        }

        public static void InsertaImagen(WordprocessingDocument wpd,
            OpenXmlElement parent,
            string ruta, int ancho, int alto, bool delete)
        {
            string relationId = AddGraph(wpd, ruta);
            if (!string.IsNullOrEmpty(relationId))
            {


                Int64Value width = ancho * 9525;
                Int64Value height = alto * 9525;

                var draw = new Drawing(
                    new DW.Inline(
                        new DW.Extent() { Cx = width, Cy = height },
                        new DW.EffectExtent()
                        {
                            LeftEdge = 0L,
                            TopEdge = 0L,
                            RightEdge = 0L,
                            BottomEdge = 0L
                        },
                        new DW.DocProperties()
                        {
                            Id = (UInt32Value)1U,
                            Name = "barcode"
                        },
                        new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks() { NoChangeAspect = true }),
                        new A.Graphic(
                            new A.GraphicData(
                                new PIC.Picture(
                                    new PIC.NonVisualPictureProperties(
                                        new PIC.NonVisualDrawingProperties()
                                        {
                                            Id = (UInt32Value)0U,
                                            Name = relationId
                                        },
                                        new PIC.NonVisualPictureDrawingProperties()),
                                        new PIC.BlipFill(
                                            new A.Blip(
                                                new A.BlipExtensionList(
                                                    new A.BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" })
                                                )
                                            {
                                                Embed = relationId,
                                                CompressionState =
                                                A.BlipCompressionValues.Print
                                            },
                                                new A.Stretch(
                                                    new A.FillRectangle())),
                                                    new PIC.ShapeProperties(
                                                        new A.Transform2D(
                                                            new A.Offset() { X = 0L, Y = 0L },
                                                            new A.Extents() { Cx = width, Cy = height }),
                                                            new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })))
                            { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" })
                                                            )
                    {
                        DistanceFromTop = (UInt32Value)0U,
                        DistanceFromBottom = (UInt32Value)0U,
                        DistanceFromLeft = (UInt32Value)0U,
                        DistanceFromRight = (UInt32Value)0U,
                        EditId = "50D07946"
                    });

                parent.Append(draw);

                if(delete)
                {
                    try
                    {
                        File.Delete(ruta);
                    }
                    catch (System.Exception)
                    {
                    }
                }
            }
        }

    }
}
