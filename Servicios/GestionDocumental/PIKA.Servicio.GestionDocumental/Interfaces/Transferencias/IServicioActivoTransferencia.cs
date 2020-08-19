using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioActivoTransferencia : IServicioRepositorioAsync<ActivoTransferencia, string>
    {
        Task<ICollection<string>> EliminarActivoTransferencia(string TransferenciaId, string[] ids);
    }
}
