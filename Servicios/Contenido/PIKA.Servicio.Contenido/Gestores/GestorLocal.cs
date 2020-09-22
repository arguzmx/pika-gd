using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Servicio.Contenido.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Gestores
{


    public class GestorLocal : GestorBase, IGestorES
    {
        private GestorLocalConfig configGestor;
        private ConfiguracionServidor configServidor;
        private Volumen volumen;
        private IServicioElemento servicioElemento;
        private IServicioVersion servicioVersion;
        private IServicioParte servicioPartes;
        private ILogger logger;


        /// <summary>
        /// Constructur para validación de la conexión
        /// </summary>
        /// <param name="configGestor"></param>
        /// <param name="volumen"></param>
        /// <param name="opciones"></param>
        public GestorLocal(
            GestorLocalConfig configGestor,
            Volumen volumen,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.volumen = volumen;
            this.configGestor = configGestor;
            this.configServidor = opciones.Value;
        }


        /// <summary>
        /// Constructor para la ingesta de archivos
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="servicioElemento"></param>
        /// <param name="servicioVersion"></param>
        /// <param name="servicioPartes"></param>
        /// <param name="configGestor"></param>
        /// <param name="volumen"></param>
        /// <param name="opciones"></param>
        public GestorLocal(
            ILogger logger,
            IServicioElemento servicioElemento,
            IServicioVersion servicioVersion,
            IServicioParte servicioPartes,
            GestorLocalConfig configGestor,
            Volumen volumen,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.logger = logger;
            this.servicioVersion = servicioVersion;
            this.servicioElemento = servicioElemento;
            this.servicioPartes = servicioPartes;
            this.volumen = volumen;
            this.configGestor = configGestor;
            this.configServidor = opciones.Value;
        }

        public bool ConexionValida()
        {
            try
            {
                string[] l = Directory.GetFiles(this.configGestor.Ruta);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        private void CreaMiniatura(string rutaMinuaturas, FileInfo informacion, int tamanoCuadro, Parte p )
        {
            if (!Directory.Exists(rutaMinuaturas)) Directory.CreateDirectory(rutaMinuaturas);

            using (Image image = Image.Load(informacion.FullName))
            {
                if(image.Width > image.Height)
                {
                    int h = (int)((((float)image.Height) / ((float)image.Width)) * tamanoCuadro);
                    // Landscape
                    image.Mutate(x => x.Resize(tamanoCuadro, h));

                } else
                {
                    //Portrait
                    int w = (int)((((float)image.Width) / ((float)image.Height)) * tamanoCuadro);
                    // Landscape
                    image.Mutate(x => x.Resize(w, tamanoCuadro));

                }

                string nombreArchivo = p.VersionId + "-" + p.ConsecutivoVolumen.ToString().PadLeft(8, '0') + ".PNG";
                string rutaFinal = Path.Combine(rutaMinuaturas, nombreArchivo);
                image.Save(rutaFinal); 
            }
        }

        public async Task<long> EscribeBytes(string Id, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            logger.LogWarning($"1> {informacion.FullName}");
            // Valida que el volumen sea apto para escriura
            if ( volumen.Activo ) {

                logger.LogWarning($"2> VOL OK");

                Modelo.Contenido.Version v = await servicioVersion.UnicoAsync(x => x.ElementoId == Id && x.Activa == true);
                if (v != null)
                {

                    logger.LogWarning($"2> VER OK");
                    string ruta = Path.Combine(this.configGestor.Ruta, Id);
                    string rutaMiniaturas = Path.Combine(this.configGestor.Ruta, Id, "thumbnails");

                    if (!Directory.Exists(ruta)) Directory.CreateDirectory(ruta);

                    // Añade el registro de la parte al repositorio
                    Parte p = new Parte() { 
                       ElementoId = Id, VersionId = v.Id ,
                        LongitudBytes = (int)informacion.Length,
                        NombreOriginal = informacion.Name,
                        TipoMime = informacion.Extension
                    };
                    p = await servicioPartes.CrearAsync(p);

                    // Calcula las variables para guardar en el nuevo FS
                    string nombreArchivo =  p.VersionId + "-" + p.ConsecutivoVolumen.ToString().PadLeft(8, '0') + informacion.Extension.ToUpper();
                    string rutaFinal = Path.Combine(ruta, nombreArchivo );

                    logger.LogWarning($"3> {rutaFinal}");

                    File.Copy(informacion.FullName, rutaFinal);

                    if (EsImagen(informacion))
                    {
                        logger.LogWarning($"4>  es imagen {rutaFinal}");
                        CreaMiniatura(rutaMiniaturas, informacion, 200, p);
                    }
                    logger.LogWarning($"XXX> HECHO");
                    return informacion.Length;

                } else
                {
                    logger.LogWarning($"1X> Sin version");
                    throw new Exception($"El elemento de contenido no tiene una versión activa");
                }
            } else
            {
                logger.LogWarning($"1X> sin volumen");
                throw new Exception($"Volumen {volumen.Nombre} no válido para escritura");
            }

           
        }

    }
}
