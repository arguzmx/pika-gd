using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIKA.ServicioBusqueda.Contenido;
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
        public async Task<ActionResult<string>> BuscarIds([FromBody] BusquedaContenido request)
        {
            try
            {
                
                return Ok(await busqueda.BuscarIds(request));
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
                throw;
            }

        }


        //[HttpPost]
        //public async Task<ActionResult<IPaginado<ElementoBusqueda>>> Buscar([FromBody] BusquedaContenido request)
        //{
        //    try
        //    {
        //        return Ok(await busqueda.Buscar(request));
        //    }
        //    catch (Exception ex)
        //    {

        //        Console.WriteLine(ex.ToString());
        //        throw;
        //    }
            
        //}
        

    }
}
