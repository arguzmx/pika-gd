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
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;

namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioTransferencia : IServicioRepositorioAsync<Transferencia, string>, 
        IServicioAutenticado<Transferencia>, IServicioBuscarTexto<Transferencia>
    {
        Task<Transferencia> CrearDesdeTemaAsync(Transferencia entity, string TemaId, bool EliminarTema = true, CancellationToken cancellationToken = default);

        Task EstadoTrasnferencia(string TransferenciaId, string EstadoId);

        Task<RespuestaComandoWeb> ComandoWeb(string command, object payload);


        Task<byte[]> ReporteTransferencia(string TransferenciaId, string[] Columnas);

        Task<string[]> EliminarRelaciones(List<Archivo>listaArchivos);
    }
}
