using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;
using RepositorioEntidades;

namespace PIKA.Servicio.Seguridad.Interfaces
{
    public interface IServicioEventoAuditoria : IServicioRepositorioAsync<EventoAuditoria, string>,
        IServicioAutenticado<EventoAuditoria>
    {
    }
}
