using PIKA.Modelo.Aplicacion.Tareas;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    public interface IServicioBitacoraTarea: IServicioRepositorioAsync<BitacoraTarea, string>
    {
   
    }
}
