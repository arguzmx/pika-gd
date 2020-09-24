using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Gestores
{
    public class GestorSMB : IGestorES
    {

        private GestorSMBConfig configGestor;
        private ConfiguracionServidor configServidor;
        private Volumen volumen;

        public GestorSMB(GestorSMBConfig configGestor,
            Volumen volumen,
            IOptions<ConfiguracionServidor> opciones)
        {
            this.volumen = volumen;
            this.configGestor = configGestor;
            this.configServidor = opciones.Value;
        }

        public string TipoGestorId => TipoGestorES.SMB;

        public bool ConexionValida()
        {

            bool valido = false;
            try
            {
                var credentials = new UserCredentials(configGestor.Dominio,
                    configGestor.Usuario, configGestor.Contrasena);

                var result = Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                {
                    valido = true;
                    return System.IO.Directory.GetFiles(configGestor.Ruta);
                });
            }
            catch (Exception)
            {

            }

            return valido;
        }

        public async Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            await Task.Delay(1);
            ValidaEscritura(ElementoId, contenido, informacion);
            if (ConexionValida())
            {
                string ruta = Path.Combine(configGestor.Ruta, volumen.Id, $"{ElementoId},{informacion.Extension}");
                if (File.Exists(ruta) )
                {
                    if(!sobreescribir)
                    {
                        throw new Exception("Archivo existente");
                    } else
                    {
                        File.Delete(ruta);
                    }
                }
                File.WriteAllBytes(ruta, contenido);
                return contenido.Length;
            }
            return -1;
        }

       

        public Task<long> EscribeBytes(string ParteId, string ElementoId, string VersionId, string archivoFuente, FileInfo informacion, bool sobreescribir)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> LeeBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            throw new NotImplementedException();
        }

        public Task<byte[]> LeeThumbnailBytes(string ElementoId, string ParteId, string VersionId, string VolumenId, string Extension)
        {
            throw new NotImplementedException();
        }

        private void ValidaEscritura(string Id, byte[] contenido, FileInfo informacion)
        {

            if (string.IsNullOrEmpty(Id))
                throw new Exception("Identificador no válido");

            if (contenido == null || contenido.Length == 0)
                throw new Exception("Contenido no válido");

            if (!volumen.Activo)
                throw new Exception("El volumen no se encuentra activo");

            if (!volumen.EscrituraHabilitada)
                throw new Exception("El volumen no tiene la escritura habilitada");

            if (volumen.TamanoMaximo != 0)
            {
                if (volumen.Tamano + informacion.Length > volumen.TamanoMaximo)
                    throw new Exception("El tamaño del contenido excede el disponible del volumen");
            }
        }
    }


}
