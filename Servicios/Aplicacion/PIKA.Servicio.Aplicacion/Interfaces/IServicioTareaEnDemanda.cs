using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Tareas;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.AplicacionPlugin.Interfaces
{
    public interface IServicioTareaEnDemanda: IServicioRepositorioAsync<TareaEnDemanda, string>
    {
        PermisoAplicacion permisos { get; set; }
        UsuarioAPI usuario { get; set; }
        Task EliminarTarea(Guid Id);
        Task CompletarTarea(Guid Id, bool Exito, string OutputPayload, string Error);
        Task ActualizaEstadoTarea(Guid Id, Infraestructura.Comun.Tareas.EstadoTarea Estado);
        Task<List<Infraestructura.Comun.Tareas.PostTareaEnDemanda>> TareasUsuario(string UsuarioId, string DominioId, string TenantId);
        Task<List<TareaEnDemanda>> TareasPendientesUsuario(string UsuarioId, string DominioId, string TenantId);

        Task<bool> EliminaTareaUsuario(string UsuarioId, string DominioId, string TenantId, Guid TareaId);
    }
}
