using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.GestorDocumental;

namespace PIKA.Servicio.GestionDocumental
{
    public static partial class Extensiones
    {
        public static EntradaClasificacion Copia(this EntradaClasificacion d)
        {
            if (d == null) return null;
            var c = new EntradaClasificacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Clave = d.Clave,
                MesesVigenciConcentracion = d.MesesVigenciConcentracion,
                MesesVigenciTramite = d.MesesVigenciTramite,
                DisposicionEntrada = d.DisposicionEntrada,
                ElementoClasificacionId = d.ElementoClasificacionId,
                Eliminada = d.Eliminada,
                MesesVigenciHistorico = d.MesesVigenciHistorico,
                Posicion = d.Posicion,
                TipoDisposicionDocumentalId = d.TipoDisposicionDocumentalId,
                TipoValoracionDocumentalId = d.TipoValoracionDocumentalId
            };

          

            return c;
        }
    }
   
}
