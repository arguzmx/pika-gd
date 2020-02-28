using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PIKA.Modelo.Organizacion;
using PIKA.Servicio.Organizacion;

namespace PIKA.UI.Web.WebAPI.Organizacion
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class UnidadOrganizacionalController : ControllerBase
    {


        private ILogger<UnidadOrganizacionalController> logger;
        private IServicioUnidadOrganizacional servicioUO;

        public UnidadOrganizacionalController(ILogger<UnidadOrganizacionalController> logger, IServicioUnidadOrganizacional servicioUO)
        {
            this.logger = logger;
            this.servicioUO = servicioUO;
        }


        //[HttpGet]
        //public async Task<ActionResult<IEnumerable<UnidadOrganizacional>>> GetDominios()
        //{
        //    return await _context.Dominios.ToListAsync();
        //}

        // GET: api/UnidadOrganizacional
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/UnidadOrganizacional/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/UnidadOrganizacional
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/UnidadOrganizacional/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
