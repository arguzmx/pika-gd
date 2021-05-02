using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PIKA.GD.API.Filters;
using PIKA.GD.API.Servicios.Caches;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Extractores;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.Servicio.Metadatos.Interfaces;
using PIKA.ServicioBusqueda.Contenido;
using RepositorioEntidades;
using System;
using System.Threading.Tasks;

namespace PIKA.GD.API.Controllers.Contenido
{
    [Authorize]
    [ApiVersion("1.0")]
    [ApiController]
    [Route("api/v{version:apiVersion}/contenido/[controller]")]
    public class BusquedaController : ControllerBase
    {

        private IServicioBusquedaContenido busqueda;

        public BusquedaController(
            IServicioBusquedaContenido busqueda ) {
            this.busqueda = busqueda;
        }

        [HttpPost]
        public async Task<ActionResult<IPaginado<Elemento>>> Buscar([FromBody] BusquedaContenido request)
        {
            try
            {
                return Ok(await busqueda.Buscar(request));
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                throw;
            }
            
        }
        

    }
}
