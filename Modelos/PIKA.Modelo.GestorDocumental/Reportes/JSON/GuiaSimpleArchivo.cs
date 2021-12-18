using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class GuiaSimpleArchivo
    {
        public GuiaSimpleArchivo()
        {
            UnidadesAdministrativas = new List<UnidadAdministrativaGuiaSimpleArchivo>();
        }

        public Archivo Archivo { get; set; }
        public List<UnidadAdministrativaGuiaSimpleArchivo> UnidadesAdministrativas { get; set; }

    }
}
