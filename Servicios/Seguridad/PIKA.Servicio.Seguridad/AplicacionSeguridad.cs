using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Seguridad;

namespace PIKA.Servicio.Seguridad
{
    public class AplicacionSeguridad : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-SEG";
        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(AplicacionRaiz.APP_ID, MODULO_BASE);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(MODULO_BASE, "USUARIOS" ) {
                    Titulo = "Usuarios del sistema",
                    Descripcion = "Permite gestionar los usuarios del sistema",
                    Tipos = new List<Type> { typeof(PropiedadesUsuario) }
                },
                new ElementoAplicacion(MODULO_BASE, "CAT-USUARIOS" ) {
                    Titulo = "Catálogos usuario",
                    Descripcion = "Permite administrar los catálogos relacionados con los usuario del sistema",
                    Tipos = new List<Type> { typeof(Genero) }
                },
                new ElementoAplicacion(MODULO_BASE, "APLICACIONES" ) {
                    Titulo = "Gestor de aplciaciones",
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
