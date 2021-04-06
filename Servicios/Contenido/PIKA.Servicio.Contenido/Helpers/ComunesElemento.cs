using Microsoft.EntityFrameworkCore;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Helpers
{
    public class ComunesElemento 
    {
        private IRepositorioAsync<Elemento> repo;
        public ComunesElemento(UnidadDeTrabajo<DbContextContenido> UDT)
        {
            this.repo = UDT.ObtenerRepositoryAsync<Elemento>(new QueryComposer<Elemento>());
        }


        public async Task<Elemento> ObtieneElemento(string Id)
        {
            Elemento el = await repo.UnicoAsync(x => x.Id.Equals(Id, StringComparison.InvariantCultureIgnoreCase));
            return el;
        }



    }
}
