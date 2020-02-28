using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using IdentityModel.Client;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using PIKA.Infraestructura.Comun;

namespace PIKA.UI.Web.WebAPI.jwt
{

    
    [Route("api/[controller]")]
    [ApiController]
    public class JwtController : ControllerBase
    {

        private ConfiguracionServidor config;
        public JwtController(IOptions<ConfiguracionServidor> options)
        {
            config = options.Value;

        }


        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }



        [HttpPost]
        [Route("~/api/jwt/refresh/{id}")]
        public async Task<ActionResult<string>> Refresh()
        {
            string accessToken = User.FindFirst("access_token")?.Value;

            if (string.IsNullOrEmpty(accessToken))
            {
                return BadRequest();
            }

            try
            {
                var client = new HttpClient();

                var response = await client.RequestRefreshTokenAsync(new RefreshTokenRequest
                {
                    Address = $"{config.jwtauth.TrimEnd('/')}/connect/token",
                    ClientId = config.jwtclient,
                    ClientSecret = config.jwtclientsecret,
                    RefreshToken = accessToken
                });


                return Ok(response.AccessToken) ;

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }

    }
}