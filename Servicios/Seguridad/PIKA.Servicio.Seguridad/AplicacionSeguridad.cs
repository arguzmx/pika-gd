using System;
using System.Collections.Generic;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public class AplicacionSeguridad : InformacionAplicacionBase, IInformacionAplicacion
    {
        
        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.APP_ID);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS ) {
                    Titulo = "Usuarios del sistema",
                    Descripcion = "Permite gestionar los usuarios del sistema",
                    Tipos = new List<Type> { typeof(PropiedadesUsuario) }
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_CAT_USUARIOS ) {
                    Titulo = "Catálogos usuario",
                    Descripcion = "Permite administrar los catálogos relacionados con los usuario del sistema",
                    Tipos = new List<Type> { typeof(Genero) }
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL ) {
                    Titulo = "Control de acceso",
                    Descripcion = "Permite gestionar el control de acceso a las aplicaciones del sistema",
                    Tipos = new List<Type> { typeof(Aplicacion), typeof(PermisoAplicacion)}
                },
                new ElementoAplicacion(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_APLICACIONES ) {
                    Titulo = "Gestor de aplicaciones",
                    Descripcion = "Permite gestionar las aplicaciones del sistema",
                    Tipos = new List<Type> { typeof(Aplicacion), 
                        typeof(ModuloAplicacion),
                        typeof(TipoAdministradorModulo),
                        typeof(TraduccionAplicacionModulo)
                    }
                }
            };
            return m;
        }
       
    }

}
