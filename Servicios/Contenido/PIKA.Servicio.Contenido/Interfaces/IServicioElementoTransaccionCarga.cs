using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioElementoTransaccionCarga
    {
        Task<ElementoTransaccionCarga> CrearAsync(ElementoTransaccionCarga entity, CancellationToken cancellationToken = default);
        Task ProcesoElemento(string ElementoId, bool Error, string Motivo);
        Task EliminarTransaccion(string TransaccionId);
        Task<List<ElementoTransaccionCarga>> OtieneElementosTransaccion(string TransaccionId);
        Task<List<Pagina>> ProcesaTransaccion(string TransaccionId, string VolumenId, IGestorES gestor);
        Task<string> ObtieneVolumenIdTransaccion(string TransaccionId);
    }
}
