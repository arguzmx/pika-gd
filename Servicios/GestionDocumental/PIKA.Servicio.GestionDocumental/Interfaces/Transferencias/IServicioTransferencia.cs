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
    public interface IServicioTransferencia : IServicioRepositorioAsync<Transferencia, string>
    {
        Task<byte[]> ReporteTransferencia(string TransferenciaId, string[] Columnas);

        Task<string[]> EliminarRelaciones(List<Archivo>listaArchivos);
    }
}
