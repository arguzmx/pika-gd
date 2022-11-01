using LazyCache;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PIKA.Servicio.Contenido.ElasticSearch;
using PIKA.ServicioBusqueda.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
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
        public async Task<ActionResult<Paginado<string>>> BuscarIds([FromBody] BusquedaContenido request)
        {
            try
            {
                var data = await busqueda.BuscarIds(request);
                return Ok(data);
            }
            catch (Exception ex)
            {

                throw;
            }
        }


        [HttpPost("sinopsis/{consultaid}")]
        public async Task<ActionResult<List<HighlightHit>>> SinopisPorIds(string consultaId, List<string> Ids)
        {
            try
            {
                var data = await busqueda.BuscarSinopsis(consultaId, Ids);
                return Ok(data);
            }
            catch (Exception ex)
            {
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
