using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using PIKA.GD.API.Model;
using System.IO;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.GD.API.Servicios.Registro
{
    public class RegistroPIKA : IRegistroPIKA
    {

        private IWebHostEnvironment env;
        private ILogger<RegistroPIKA> logger;
        public RegistroPIKA(ILogger<RegistroPIKA> logger, IWebHostEnvironment env)
        {
            this.env = env;
            this.logger = logger;
        }

        private FirmaServer LeeActivacion(string Texto = null)
        {

            string cifrado = Texto;
            if (string.IsNullOrEmpty(cifrado))
            {
                string ruta = RutaActivacion();
                if (File.Exists(ruta))
                {
                    cifrado = File.ReadAllText(ruta);
                }

            }

            if(!string.IsNullOrEmpty(cifrado))
            {
                try
                {
                    string descifrado = CifradoSimetrico.DecryptString(Dns.GetHostName(), cifrado);
                    var activacion = JsonSerializer.Deserialize<FirmaServer>(descifrado);
                    return activacion;
                }
                catch (System.Exception ex)
                {
                    logger.LogError($"Error al leer activación {ex.Message}");
                }
            }

            return null;
        }

        private void GuardaActivacion(FirmaServer f)
        {
            string ruta = RutaActivacion();
            if (File.Exists(ruta))
            {
                File.Delete(ruta);
            }

            string cifrado = CifradoSimetrico.EncryptString(Dns.GetHostName(), JsonSerializer.Serialize(f));
            File.WriteAllText(ruta, cifrado);
            logger.LogInformation($"Activación actualizada");
        }
        
        private string RutaActivacion ()
        {
            return Path.Combine(env.ContentRootPath, "activacion.lic");
        }

        public Task<bool> ActivarLicencia(string CodigoActivacion)
        {
            try
            {
                var serializado = CodigoActivacion.Base64Decode();
                var f = LeeActivacion(serializado);
                if (f != null)
                {
                    var actual = LeeActivacion();
                    actual.Activacion = f.Activacion;
                    actual.FechaActivacion = f.FechaActivacion;
                    GuardaActivacion(f);
                    return Task.FromResult(true);
                }
            }
            catch (System.Exception) {}

            return Task.FromResult(false);

        }

        public Task<bool> LicenciaValida()
        {
            FirmaServer f = LeeActivacion();
            if (f != null)
            {
                return Task.FromResult(f.Activacion.HasValue);
            }
            return Task.FromResult(false);
        }



        public Task<string> ObtenerFingerprint()
        {
            FirmaServer f = LeeActivacion();
            if(f == null )
            {
                f = new FirmaServer()
                {
                    HostName = Dns.GetHostName(),
                    IdServer = System.Guid.NewGuid()
                };
                GuardaActivacion(f);
            }
            return Task.FromResult (JsonSerializer.Serialize(f).Base64Encode());
        }
    }
}
