using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
    public class AplicacionAplicacionPlugin : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD_PLUGIN-";


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
                new ElementoAplicacion(MODULO_BASE, "PLUGINS" ) {
                    Titulo = "Plugins del sistema",
                    Descripcion = "Permite administrar la configuración de los pluigns del sistema",
                    Tipos = new List<Type> { typeof(Plugin) }
                },
                  new ElementoAplicacion(MODULO_BASE, "VERSIONES" ) {
                    Titulo = "Versions de Plugins del sistema",
                    Descripcion = "Permite administrar las versiones de los plugins del sistema",
                    Tipos = new List<Type> { typeof(VersionPlugin) }
                },
                  new ElementoAplicacion(MODULO_BASE, "VERSIONES" ) {
                    Titulo = "Plugins instalados",
                    Descripcion = "Permite administrar los plugins instalados del sistema",
                    Tipos = new List<Type> { typeof(PluginInstalado) }
                }
            };
            return m;
        }

    }

}
