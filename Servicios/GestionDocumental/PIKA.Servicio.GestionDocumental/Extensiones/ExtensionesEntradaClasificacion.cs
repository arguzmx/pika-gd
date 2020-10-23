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
                VigenciaConcentracion = d.VigenciaConcentracion,
                VigenciaTramite = d.VigenciaTramite,
                DisposicionEntrada = d.DisposicionEntrada,
                ElementoClasificacionId = d.ElementoClasificacionId,
                Eliminada = d.Eliminada,
                Posicion = d.Posicion,
                TipoDisposicionDocumentalId = d.TipoDisposicionDocumentalId,
                TipoValoracionDocumentalId = d.TipoValoracionDocumentalId,
                CuadroClasifiacionId = d.CuadroClasifiacionId,
                Descripcion = d.Descripcion
            };

          

            return c;
        }
    }
   
}
