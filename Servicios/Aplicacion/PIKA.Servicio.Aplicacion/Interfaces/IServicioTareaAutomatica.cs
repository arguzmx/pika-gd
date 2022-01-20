using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Tareas;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    public interface IServicioTareaAutomatica: IServicioRepositorioAsync<TareaAutomatica, string>
    {
        PermisoAplicacion permisos { get; set; }
        UsuarioAPI usuario { get; set; }
    }
}
