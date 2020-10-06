using DocumentFormat.OpenXml.Office2010.ExcelAc;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivoPrestamo : IServicioRepositorioAsync<ActivoPrestamo, string>
    {
        
    }
}

