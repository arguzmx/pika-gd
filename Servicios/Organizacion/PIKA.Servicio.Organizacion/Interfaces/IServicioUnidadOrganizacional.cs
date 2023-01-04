using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Organizacion
{
    public interface IServicioUnidadOrganizacional: IServicioRepositorioAsync<UnidadOrganizacional,string>, IServicioAutenticado<UnidadOrganizacional>
    {
        Task<string[]> Purgar();
       
}
}
