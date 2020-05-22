using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Modelo.Metadatos
{
    public interface IRepositorioMetadatos
    {
        /// <summary>
        /// Obtiene un elemento único del repositorio basado en el Id
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="id">Identificador del registro a recuperar</param>
        /// <returns></returns>
        ValoresPlantilla Unico(Plantilla plantilla, string id);

        /// <summary>
        /// Elmina un elmento único del repositorio
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="id">Identificador del registro a eliminar</param>
        /// <returns></returns>
        bool Elimina(Plantilla plantilla, string id);


        /// <summary>
        /// Inserta un elmento al repositorio
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="valores">Datos de los valores en las propedades de la plantilla</param>
        /// <returns></returns>
        string Inserta(Plantilla plantilla, ValoresPlantilla valores);

        /// <summary>
        /// Actualiza un elmento al repositorio
        /// </summary>
        /// <param name="plantilla">Modelo de la plantilla</param>
        /// <param name="valores">Datos de los valores en las propedades de la plantilla</param>
        /// <returns></returns>
        bool Actualiza(Plantilla plantilla, ValoresPlantilla valores);

        /// <summary>
        /// Obtiene una lista de elementos de la plantilla
        /// </summary>
        /// <param name="plantilla"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        List<ValoresPlantilla> Consulta(Plantilla plantilla, Consulta query);
        
    }
}
