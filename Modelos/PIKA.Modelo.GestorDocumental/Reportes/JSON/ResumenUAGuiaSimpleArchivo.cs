using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class UnidadAdministrativaGuiaSimpleArchivo : UnidadAdministrativaArchivo
    {
        public UnidadAdministrativaGuiaSimpleArchivo()
        {
            Secciones = new List<SeccionGuiaSimpleArchivo>();
        }
        public int Expedientes { get; set; }
        public List<SeccionGuiaSimpleArchivo> Secciones { get; set; }
    }
}
