using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    internal interface IRegistroApp
    {
        Task<bool> Registrada();
        Task<bool> Registrar(int id);
    }
}
