using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PIKA.Servicio.Contenido.Gestores
{


    public class GestorLocal : IGestorES
    {
        private GestorLocalConfig configGestor;
        private ConfiguracionServidor configServidor;
        private Volumen volumen;


        public GestorLocal(GestorLocalConfig configGestor,
            Volumen volumen,
            IOptions<ConfiguracionServidor> opciones)
        {
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

        public long EscribeBytes(string Id, byte[] contenido, FileInfo informacion, bool sobreescribir)
        {
            throw new NotImplementedException();
        }
    }
}
