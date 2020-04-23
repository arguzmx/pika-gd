using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public static class ExtensionesSeguridad
    {
        public static Aplicacion CopiaAplicacion(this Aplicacion d)
        {
            return new Aplicacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                UICulture=d.UICulture,
                ReleaseIndex=d.ReleaseIndex,
                Version=d.Version
            };
        }
        public static ModuloAplicacion CopiaModuloAplicacion(this ModuloAplicacion d)
        {
            return new ModuloAplicacion()
            {
                Id = d.Id,
                Nombre = d.Nombre,
                Descripcion = d.Descripcion,
                UICulture = d.UICulture,
                Icono=d.Icono,
                AplicacionId=d.AplicacionId,
                Asegurable=d.Asegurable,
                PermisosDisponibles=d.PermisosDisponibles,
                ModuloPadreId=d.ModuloPadreId
            };
        }
        public static TipoAdministradorModulo CopiaTipoAdministradorModulo(this TipoAdministradorModulo d)
        {
            return new TipoAdministradorModulo()
            {
                Id = d.Id,
                AplicacionId=d.AplicacionId,
                ModuloId=d.ModuloId
            };
        }
        public static TraduccionAplicacionModulo CopiaTraduccionAplicacionModulo(this TraduccionAplicacionModulo d)
        {
            return new TraduccionAplicacionModulo()
            {
                Id = d.Id,
                Nombre=d.Nombre,
                Descripcion=d.Descripcion,
                AplicacionId = d.AplicacionId,
                ModuloId = d.ModuloId,
                UICulture=d.UICulture
            };
        }
    }
}
