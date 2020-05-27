using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Infraestructura.Comun
{
    public interface IAPICache<T>
    {
        Task<T> Obtiene(string id, string modulo);

        Task Inserta(string id, string modulo, T entidad, TimeSpan expira);

        Task Elimina(string id, string modulo);

        /// <summary>
        /// Elimina las entradas del cache si las claves ctienen la cadena referenciada
        /// </summary>
        /// <param name="clave"></param>
        /// <returns></returns>
        Task EliminaSiContiene(string clave);
    }
}
