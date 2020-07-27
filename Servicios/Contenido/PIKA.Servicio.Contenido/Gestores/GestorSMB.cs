using PIKA.Modelo.Contenido;
using SimpleImpersonation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PIKA.Servicio.Contenido.Gestores
{
    public class GestorSMB : IGestorES
    {

        public string RutaCompartida { get; set; }
        public string Dominio { get; set; }
        public string Usuario { get; set; }
        public string Contraena { get; set; }

        public string TipoGestorId => TipoGestorES.SMB;

        public bool ConexionValida()
        {

            bool valido = false;
            try
            {
                var credentials = new UserCredentials(Dominio, Usuario, Contraena);

                var result = Impersonation.RunAsUser(credentials, LogonType.NewCredentials, () =>
                {
                    valido = true;
                    return System.IO.Directory.GetFiles(RutaCompartida);
                });
            }
            catch (Exception)
            {

            }


            return valido;
        }

      

        public bool AplicaConfiguracion(string entidadSerializada)
        {
            try
            {
                var gestor = JsonSerializer.Deserialize<GestorSMB>(entidadSerializada);

                this.RutaCompartida = gestor.RutaCompartida;
                this.Dominio = gestor.Dominio;
                this.Usuario = gestor.Usuario;
                this.Contraena = gestor.Contraena;
                return true;
            }
            catch (Exception)
            {
                return false;

            }
        }

 

        public string ObtieneConfiguracion()
        {
            return JsonSerializer.Serialize(this);
        }
    }


}
