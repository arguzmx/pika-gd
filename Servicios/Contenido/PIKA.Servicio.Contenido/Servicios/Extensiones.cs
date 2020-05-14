using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido.Servicios
{
    public static class ExtensionesContenido
    {
        public static Elemento CopiaElemento(this Elemento d)
        {
            return new Elemento()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                CreadorId=d.CreadorId,
                Eliminada=d.Eliminada,
                FechaCreacion=d.FechaCreacion,
                OrigenId=d.OrigenId,
                TipoOrigenId=d.TipoOrigenId
            };
        }
        public static Parte CopiaParte(this Parte d)
        {
            return new Parte()
            {
                 ElementoId=d.ElementoId,
                 VersionId=d.VersionId,
                 ConsecutivoVolumen=d.ConsecutivoVolumen,
                 Eliminada=d.Eliminada,
                 Indice=d.Indice,
                 LongitudBytes=d.LongitudBytes,
                 NombreOriginal=d.NombreOriginal,
                 TipoMime=d.TipoMime,
    
            };
        }
        public static TipoGestorES CopiaTipoGestorES(this TipoGestorES d)
        {
            return new TipoGestorES()
            {
                 Id=d.Id,
                 Nombre=d.Nombre,
                 Volumenid=d.Volumenid
            };
        }
        public static Version CopiaVersion(this Version d)
        {
            return new Version()
            {
                Id = d.Id,
                Activa=d.Activa,
                ElementoId=d.ElementoId,
                FechaCreacion=d.FechaCreacion,
                FechaActualizacion=d.FechaActualizacion
            };
        }

        public static Volumen CopiaVolumen(this Volumen d)
        {
            return new Volumen()
            {
                Id = d.Id,
                Nombre=d.Nombre,
                Activo=d.Activo,
                CadenaConexion=d.CadenaConexion,
                CanidadPartes=d.CanidadPartes,
                ConsecutivoVolumen=d.ConsecutivoVolumen,
                Eliminada=d.Eliminada,
                EscrituraHabilitada=d.EscrituraHabilitada,
                OrigenId=d.OrigenId,
                Tamano=d.Tamano,
                TipoGestorESId=d.TipoGestorESId,
                TipoOrigenId=d.TipoOrigenId
            };
        }
    }
}
