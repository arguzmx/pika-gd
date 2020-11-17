using PIKA.Infraestructura.Comun.Configuracion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contenido.Servicios
{
    public class ConfiguracionRepoContenido : ConfiguracionServicioStandard
    {
        public const string ELASTICSEARCH = "elasticsearch";


        public override string CadenaConexion()
        {
            switch (Tipo)
            {
                case ELASTICSEARCH:
                    StringBuilder sb = new StringBuilder();
                    sb.Append(this.DatosConexion.Protocolo);
                    if (!(string.IsNullOrEmpty(this.DatosConexion.Usuario) &&
                        string.IsNullOrEmpty(this.DatosConexion.Contrasena)))
                    {
                        sb.Append(this.DatosConexion.Usuario + ":" + this.DatosConexion.Contrasena + "@");
                    }

                    sb.Append(this.DatosConexion.Url);
                    sb.Append(":" + this.DatosConexion.Puerto);

                    return sb.ToString();
            }

            return "";
        }
    }
}
