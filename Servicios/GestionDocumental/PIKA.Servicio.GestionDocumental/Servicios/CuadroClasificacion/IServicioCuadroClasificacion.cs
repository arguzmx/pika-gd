using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioCuadroClasificacion: IServicioRepositorioAsync<CuadroClasificacion, string>,
          IServicioValorTextoAsync<CuadroClasificacion>, IServicioAutenticado<CuadroClasificacion>
    {
        Task<byte[]> ExportarCuadroCalsificacionExcel(string CuadroClasificacionId);
        Task<string[]> Purgar();

    }
}
