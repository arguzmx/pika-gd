using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace PIKA.Infraestructura.Comun.Seguridad
{

    /// <summary>
    /// Almacena los prmiso de acceso a los módulos aministrados por la organización
    /// </summary>
    public class SecurityToken
    {
        public SecurityToken()
        {
            TicketId = Guid.NewGuid().ToString();
        }

        /// <summary>
        /// I´d único para solicitar acceso al toke, se entrega al cliente
        /// </summary>
        public string TicketId { get; set; }

        /// <summary>
        /// Idunico del dominio al que pertenece el token
        /// </summary>
        public string DomainId { get; set; }

        /// <summary>
        /// Id Uinco de la aplicación a la que pertenece el tocken
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// Id del usuario asociado a los permisos
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// fecha d creacion del token
        /// </summary>
        public DateTime CreatedOn { get; set; }


        /// <summary>
        /// Identificador único del host donde es válido el ticket
        /// </summary>
        public string HostId { get; set; }

        /// <summary>
        /// Contenido serializado del token
        /// </summary>
        public string Content { get; set; }

        //Serializa y actualiza la propiedad content
        public string SerializeGrants(List<AppDomainGrant> Grants)
        {

            this.Content = JsonSerializer.Serialize(Grants);
            return this.Content;
        }


        /// <summary>
        /// Deserializa el contenido 
        /// </summary>
        /// <param name="Content">si el pa´rametro es dferente de nulo reemplaza rl contenido</param>
        /// <returns></returns>
        public static List<AppDomainGrant> DeserializeGrants(string Content)
        {

            try
            {
                List<AppDomainGrant> l = JsonSerializer.Deserialize<List<AppDomainGrant>>(Content);
                return l;
            }
            catch (Exception ex)
            {
                return null;
            }

        }

    }
}
