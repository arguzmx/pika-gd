using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using PIKA.Modelo.Aplicacion.Plugins;
using RepositorioEntidades;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    public interface IServicioPlugin : IServicioRepositorioAsync<Plugin, string>
    {
        Task<List<string>> Purgar();
    }
}
