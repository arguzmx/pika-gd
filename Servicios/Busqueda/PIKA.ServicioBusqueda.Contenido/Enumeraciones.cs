using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.ServicioBusqueda.Contenido
{

   

    public enum EstadoBusqueda
    {
        Nueva = 0, EnEjecucion = 1, Finaliza = 3, FinalizadaError = 10
    }

    public class Constantes
    {
        public const string PROPIEDEDES = "propiedades";
        public const string ENFOLDER = "enfolder";
        public const string METADATOS = "metadatos";
    }

}
