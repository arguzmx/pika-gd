using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{

    /// <summary>
    /// Proporciona informción de la apalicación
    /// </summary>
    public interface IInformacionAplicacion
    {

        /// <summary>
        /// Otiene una instancia de la información de la aplicación
        /// </summary>
        /// <returns></returns>
        Aplicacion Info();


        /// <summary>
        /// Obtiene los módulos asociados a la aplicación actual
        /// </summary>
        /// <returns></returns>
        List<ModuloAplicacion> ModulosAplicacion();

        List<TipoAdministradorModulo> TiposAdministrados();


    }
}
