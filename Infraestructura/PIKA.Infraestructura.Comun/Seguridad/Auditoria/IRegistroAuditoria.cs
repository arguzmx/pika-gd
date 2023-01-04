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

        /// <summary>
        /// Añade un evento al registro de auditoría
        /// </summary>
        /// <param name="ev"></param>
        /// <returns></returns>
        Task<EventoAuditoria> InsertaEvento(EventoAuditoria ev);

        /// <summary>
        /// Obtiene la lista de los eventos auditables para el dominio y unidad organizacional
        /// </summary>
        /// <param name="DominioId"></param>
        /// <param name="OUId"></param>
        /// <returns></returns>
        Task<List<EventoAuditoriaActivo>> EventosAuditables(string DominioId, string OUId);

    }
}
