using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad.Servicios
{
    public static class ExtensionesSeguridad
    {

        public static PropiedadesUsuario CopiaPropieddesUsuario(this PropiedadesUsuario d)
        {
            var r = new PropiedadesUsuario()
            {
                UsuarioId = d.UsuarioId,
                email = d.email,
                password = "",
                username = d.username,
                email_verified = d.email_verified,
                estadoid = d.estadoid,
                family_name = d.family_name,
                generoid = d.generoid,
                given_name = d.given_name,
                gmt = d.gmt,
                gmt_offset = d.gmt_offset,
                middle_name = d.middle_name,
                name = d.name,
                nickname = d.nickname,
                paisid = d.paisid,
                picture = "",
                updated_at = d.updated_at,
            };

            return r;
        }

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
