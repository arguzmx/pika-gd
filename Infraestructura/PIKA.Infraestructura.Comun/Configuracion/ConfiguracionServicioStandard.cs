using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Configuracion
{
    public abstract class ConfiguracionServicioStandard
    {
        public string  Tipo { get; set; }

        public DatosConexion DatosConexion { get; set; }
        public virtual string CadenaConexion()
        {
            return "";
        }
    }

    public class DatosConexion
    {
        public DatosConexion()
        {
            Id = "";
            Protocolo = "";
            Puerto = 0;
            Url = "";
            Usuario = "";
            Contrasena = "";
            Reintentos = 0;
        }

        public string Id { get; set; }
        public string Protocolo { get; set; }
        public int Puerto { get; set; }
        public string Url { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public int Reintentos { get; set; }
    }

}
