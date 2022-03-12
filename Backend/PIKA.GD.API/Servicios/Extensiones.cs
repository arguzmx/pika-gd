using Microsoft.AspNetCore.Http;

namespace PIKA.GD.API.Servicios
{
    public static class Extensiones
    {

        public static string EndpointURl(this HttpRequest r, string version, string controller)
        {
            return $"{(r.IsHttps ? "https://" : "http://")}{r.Host}/api/v{version}/{controller}";
        }

    }
}
