using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using RepositorioEntidades;

namespace PIKA.Servicio.Contenido.Interfaces
{
   public interface IServicioGestorSMBConfig : IServicioRepositorioAsync<GestorSMBConfig, string>, IServicioAutenticado<GestorSMBConfig>
    {
       
    }
}
