using Microsoft.EntityFrameworkCore.Query;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    public interface IRegistroAuditoria
    {
        Task<EventoAuditoria> InsertaEvento(EventoAuditoria ev);
    }
}
