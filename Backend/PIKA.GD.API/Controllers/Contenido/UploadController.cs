using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace PIKA.GD.API.Controllers.Contenido
{
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private ILogger<UploadController> logger;
        private IConfiguration Config;
        public UploadController(ILogger<UploadController> logger, IConfiguration config)
        {
            this.logger = logger;
            this.Config = config;
        }

        [HttpPost]
        public async Task<IActionResult> Post(IFormFile file)
        {
            long size = file.Length;
            var filePath = "";
            if (size > 0)
            {
                var path = Config["FILE_PATH"];
                filePath = Path.Combine(path, Path.GetRandomFileName() + Path.GetExtension(file.FileName));
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream).ConfigureAwait(false);
                }
            }
            return Ok(new { size, filePath });
        }
    }
}