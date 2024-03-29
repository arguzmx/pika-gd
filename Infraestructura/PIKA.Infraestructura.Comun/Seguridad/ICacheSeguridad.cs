﻿using PIKA.Infraestructura.Comun.Menus;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun.Seguridad
{
    /// <summary>
    /// Interfaz de evaluación de permisos de la aplcación
    /// </summary>
    public interface ICacheSeguridad
    {
        /// <summary>
        /// DEtermina si el usuario tiene el derecho de acceso al módulo seleccionado
        /// </summary>
        /// <param name="UserId"></param>
        /// <param name="DomainId"></param>
        /// <param name="AppId"></param>
        /// <param name="ModuleId"></param>
        /// <param name="Method">Médodo HTTP</param>
        /// <returns></returns>
        Task<bool> AllowMethod(string UserId, string DomainId, string AppId, string ModuleId, string Method);

        Task<List<EventoAuditoriaActivo>> EventosAuditables(string DomainId, string OUId);
        Task DatosUsuarioSet(UsuarioAPI Usuario);

        Task<UsuarioAPI> DatosUsuarioGet(string Id);

    }
}
