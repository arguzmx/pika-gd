using PIKA.Modelo.Organizacion;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion
{
    public interface IServicioUnidadOrganizacional: IServicioRepositorioAsync<UnidadOrganizacional,string>
    {
    }
}
