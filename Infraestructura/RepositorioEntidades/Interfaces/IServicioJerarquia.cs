using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RepositorioEntidades
{
    /// <summary>
    /// Gestiona jerarquiás de entidades
    /// </summary>
    /// <typeparam name="T">Tipo de datos para el ID</typeparam>
    public interface IServicioJerarquia<T>
    {


        /// <summary>
        /// Ontiene una lista de descendientes de manera recursiva
        /// </summary>
        /// /// <param name="IdJerarquia">Identificador de la jerarquia a la que pertenece el nodo</param>
        /// <param name="Id">Identificador único del padre</param>
        /// <param name="N">Niveles de recursipon</param>
        /// <returns></returns>
        Task<List<NodoJerarquico>> ObtenerDescendientes(T IdJerarquia, T Id, int N);


        /// <summary>
        /// Obtiene una lista de elemntos raíz
        /// </summary>
        /// <param name="IdJerarquia">Identificadorúnico de la jerarquia</param>
        /// <param name="N">Niveles de recursipon</param>
        /// <returns></returns>
        Task<List<NodoJerarquico>> ObtenerRaices(T IdJerarquia, int N);

    }
}
