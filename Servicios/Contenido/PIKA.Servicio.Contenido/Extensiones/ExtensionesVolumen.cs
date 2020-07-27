using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;

namespace PIKA.Servicio.Contenido
{
    public static partial class Extensiones
    {

        public static Volumen Copia(this Volumen d)
        {
            if (d == null) return null;
            return new Volumen()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Activo = d.Activo,
                CanidadPartes = d.CanidadPartes,
                ConsecutivoVolumen = d.ConsecutivoVolumen,
                Eliminada = d.Eliminada,
                EscrituraHabilitada = d.EscrituraHabilitada,
                OrigenId = d.OrigenId,
                Tamano = d.Tamano,
                TipoGestorESId = d.TipoGestorESId,
                TipoOrigenId = d.TipoOrigenId, 
                TamanoMaximo = d.TamanoMaximo
            };
        }
    }
}
