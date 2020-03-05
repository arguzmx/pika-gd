using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Refit.PIKA.GD
{
    public interface IDominioAPI
    {
        [Get("/users/{id}")]
        Task<Dominio> GetDominio(string id);
    }
}
