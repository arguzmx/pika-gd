using NetBarcode;
using QRCoder;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace PIKA.Infraestrctura.Reportes
{
    public static class CodigosOpticos
    {
        public static string GeneraBarcodeLineal(TokenCB1D config, string rutaTemporales)
        {

            string nombreArchivo = System.Guid.NewGuid().ToString() + ".png";
            string rutaArchivo = Path.Combine(rutaTemporales, nombreArchivo);

            if (config != null)
            {
                NetBarcode.Type tipo = NetBarcode.Type.Code39;
                switch (config.Tipo.ToUpper())
                {
                    case "CODE39":
                        config.Dato = "*" + config.Dato.ToUpper() + "*";
                        tipo = NetBarcode.Type.Code39;
                        break;

                    case "CODE11":
                        tipo = NetBarcode.Type.Code11;
                        break;

                    case "CODE128":
                        tipo = NetBarcode.Type.Code128;
                        break;

                    case "CODE128A":
                        tipo = NetBarcode.Type.Code128A;
                        break;

                    case "CODE128B":
                        tipo = NetBarcode.Type.Code128B;
                        break;

                    case "CODE128C":
                        tipo = NetBarcode.Type.Code128C;
                        break;

                    case "CODE93":
                        tipo = NetBarcode.Type.Code93;
                        break;

                    case "EAN13":
                        tipo = NetBarcode.Type.EAN13;
                        break;

                    case "EAN8":
                        tipo = NetBarcode.Type.EAN8;
                        break;
                }
                try
                {
                    var barcode = new Barcode(config.Dato, tipo, true, Cm2Pixeles(config.Ancho), Cm2Pixeles(config.Alto));
                    barcode.SaveImageFile(rutaArchivo, ImageFormat.Png);
                }
                catch (Exception) { }
            }

            return File.Exists(rutaArchivo) ? rutaArchivo : "";
        }

        public static string GeneraBarcodeQR(TokenCB2D config, string rutaTemporales)
        {

            string nombreArchivo = System.Guid.NewGuid().ToString() + ".png";
            string rutaArchivo = Path.Combine(rutaTemporales, nombreArchivo);

            if (config != null)
            {
                
                try
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    QRCodeData qrCodeData = qrGenerator.CreateQrCode(config.Dato, QRCodeGenerator.ECCLevel.Q);
                    QRCode qrCode = new QRCode(qrCodeData);
                    Bitmap qrCodeImage = qrCode.GetGraphic(20);
                    qrCodeImage.Save(rutaArchivo);
                }
                catch (Exception) { }
            }

            return File.Exists(rutaArchivo) ? rutaArchivo : "";
        }


        public static int Cm2Pixeles(float cms)
        {
            return ((int)((cms * 96) / 2.54));
        }

    }
}
