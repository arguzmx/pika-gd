using PIKA.Infraestructura.Comun.Configuracion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class ConfiguracionRepoMetadatos: ConfiguracionServicioStandard
    {
        public const string ELASTICSEARCH = "elasticsearch";

        public const string RABBITMQ = "rabbitmq";

        public override string CadenaConexion()
        {
            StringBuilder sb = new StringBuilder();
            switch (Tipo)
            {
                case ELASTICSEARCH:
                    sb = new StringBuilder();
                    sb.Append(this.DatosConexion.Protocolo);
                    if(!(string.IsNullOrEmpty(this.DatosConexion.Usuario) &&
                        string.IsNullOrEmpty(this.DatosConexion.Contrasena)))
                    {
                        sb.Append(this.DatosConexion.Usuario + ":" + this.DatosConexion.Contrasena + "@");
                    }

                    sb.Append(this.DatosConexion.Url);
                    sb.Append(":" + this.DatosConexion.Puerto);

                    return sb.ToString();

                case RABBITMQ:
                    sb = new StringBuilder();
                    sb.Append( string.IsNullOrEmpty(this.DatosConexion.Protocolo) ? "amqp://" : this.DatosConexion.Protocolo);
                    if (!(string.IsNullOrEmpty(this.DatosConexion.Usuario) &&
                        string.IsNullOrEmpty(this.DatosConexion.Contrasena)))
                    {
                        sb.Append(this.DatosConexion.Usuario + ":" + this.DatosConexion.Contrasena + "@");
                    }

                    sb.Append(this.DatosConexion.Url);
                    sb.Append(":" + this.DatosConexion.Puerto);

                    return sb.ToString();

                default:
                    sb = new StringBuilder();
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
