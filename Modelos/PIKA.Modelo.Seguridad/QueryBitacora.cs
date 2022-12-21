using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Seguridad
{
    public class QueryBitacora
    {
        public string AppId { get; set; }
        public string ModuloId { get; set; }
        public List<TipoEventoAuditoria> Eventos { get; set; }
        public List<string> UsuarioIds { get; set; }
        public DateTime? FechaInicial { get; set; }
        public DateTime? FechaFinal { get; set; }
        public string CampoOrdenamiento { get; set; }
        public string ModoOrdenamiento { get; set; }
        public int? Indice { get; set; }
        public int? TamanoPagina { get; set; }

    }
}
