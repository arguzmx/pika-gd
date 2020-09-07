using Microsoft.EntityFrameworkCore.Migrations;
using PIKA.Infraestructura.Comun;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Seguridad
{
    public static class ExtensionesAplicacion
    {
    

        public static Aplicacion Copia(this Aplicacion app)
        {
            Aplicacion a = new Aplicacion()
            {
                Descripcion = app.Descripcion,
                Id = app.Id,
                Modulos = new List<ModuloAplicacion>(),
                Nombre = app.Nombre,
                ReleaseIndex = app.ReleaseIndex,
                UICulture = app.UICulture,
                Version = app.Version,
                Traducciones = new List<TraduccionAplicacionModulo>()
            };

            if (app.Modulos != null)
            {
                foreach (var m in app.Modulos)
                {
                    a.Modulos.Add(m.Copia());
                }
            }

            if (app.Traducciones != null)
            {
                foreach (var t in app.Traducciones)
                {
                    a.Traducciones.Add(t.Copia());
                }
            }

            return a;
        }

        public static ModuloAplicacion Copia(this ModuloAplicacion mod)
        {
            ModuloAplicacion m = new ModuloAplicacion()
            {
                AplicacionId = mod.AplicacionId,
                AplicacionPadreId = mod.AplicacionPadreId,
                Asegurable = mod.Asegurable,
                Descripcion = mod.Descripcion,
                Icono = mod.Icono,
                Id = mod.Id,
                ModuloPadreId = mod.ModuloPadreId,
                Nombre = mod.Nombre,
                PermisosDisponibles = mod.PermisosDisponibles,
                Tipo = mod.Tipo,
                UICulture = mod.UICulture,
                TiposAdministrados = new List<TipoAdministradorModulo>(),
                Traducciones = new List<TraduccionAplicacionModulo>()
            };

            if (m.TiposAdministrados != null)
            {
                foreach (var t in m.TiposAdministrados)
                {
                    m.TiposAdministrados.Add(t.Copia());
                }
            }

            if (m.Traducciones != null)
            {
                foreach (var t in m.Traducciones)
                {
                    m.Traducciones.Add(t.Copia());
                }
            }

            return m;
        }

        public static TipoAdministradorModulo Copia(this TipoAdministradorModulo ta)
        {
            TipoAdministradorModulo t = new TipoAdministradorModulo()
            {
                AplicacionId = ta.AplicacionId,
                Id = ta.Id,
                ModuloId = ta.ModuloId
                // TiposAdministrados = ta.TiposAdministrados
            };
            return t;
        }

        public static TraduccionAplicacionModulo Copia(this TraduccionAplicacionModulo trad)
        {
            TraduccionAplicacionModulo t = new TraduccionAplicacionModulo()
            {
                AplicacionId = trad.AplicacionId,
                Descripcion = trad.Descripcion,
                Id = trad.Id,
                ModuloId = trad.ModuloId,
                Nombre = trad.Nombre,
                UICulture = trad.UICulture
            };
            return t;
        }
    }
}
