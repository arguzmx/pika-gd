using IdentityServer4.Events;
using IdentityServer4.Services;
using Newtonsoft.Json;
using PIKA.Servicio.Seguridad.Interfaces;
using PIKA.Servicio.Seguridad.Servicios;
using System;
using System.Threading.Tasks;

namespace PIKA.Identity.Server
{
    public class CustomEventSink : IEventSink
    {
        private IServicioUsuarios ServicioUsuarios;
        public CustomEventSink(IServicioUsuarios ServicioUsuarios)
        {
            this.ServicioUsuarios = ServicioUsuarios;
        }

        public Task PersistAsync(Event evt)
        {
            bool ok = false;
            string name = "";

            if (evt.Category == "Authentication" && evt.EventType== EventTypes.Success && evt.Name == "User Login Success")
            {
                ok = true;
                name = ((UserLoginSuccessEvent)evt).Username;
                var t = Task.Run(() => ServicioUsuarios.RegistroLogin(name, ok, $"{evt.RemoteIpAddress}"));
                t.Wait();
            }

            if (evt.Category == "Authentication" && evt.EventType == EventTypes.Failure && evt.Name == "User Login Failure")
            {

               name = ((UserLoginFailureEvent)evt).Username;
               var t = Task.Run(() => ServicioUsuarios.RegistroLogin(name, ok, $"{evt.RemoteIpAddress}"));
               t.Wait();
            }

            return Task.CompletedTask;
        }
    }
}
