using PIKA.Infraestructura.Comun.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun
{
    public interface IServicioAutenticado<T>
    {
        /// <summary>
        /// Datos del usuario en sesión
        /// </summary>
        UsuarioAPI usuario { get; set; }
        
        /// <summary>
        /// Sobre el controlador de acuerdo al usuario en sesión
        /// </summary>
        PermisoAplicacion permisos { get; set; }

        /// <summary>
        /// Obtiene los permisos a través de el usuario en sesión y los valores
        /// propercionados en la llamada para el dominio y la unidad organizacional
        /// </summary>
        /// <param name="EntidadId"></param>
        /// <param name="DominioId"></param>
        /// <param name="UnidaddOrganizacionalId"></param>
        /// <returns></returns>
        Task<T> ObtienePerrmisos(string EntidadId, string DominioId, string UnidaddOrganizacionalId);
    }
}
