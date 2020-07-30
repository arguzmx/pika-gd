using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static ValoracionEntradaClasificacion Copia(this ValoracionEntradaClasificacion a)
        {
            if (a == null) return null;
            return new ValoracionEntradaClasificacion()
            {
               
               EntradaClasificacionId = a.EntradaClasificacionId,
                TipoValoracionDocumentalId = a.TipoValoracionDocumentalId,
            };
        }
    }
}
