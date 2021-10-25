using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Constantes.Aplicaciones.Aplicaciones;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Aplicacion.Plugins;

namespace PIKA.Servicio.AplicacionPlugin
{
    public class AplicacionAplicacionPlugin : InformacionAplicacionBase, IInformacionAplicacion
    {
        
     public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppAplicacionPlugin.APP_ID, ConstantesAppAplicacionPlugin.APP_ID);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {

                new ElementoAplicacion(ConstantesAppAplicacionPlugin.APP_ID, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES ) {
                    Titulo = "Configuración del sistema",
                    Descripcion = "Permite administrar la configuración del sistema",
                    Tipos = new List<Type> { typeof(Aplicacion) }
                },

                //new ElementoAplicacion(ConstantesAppAplicacionPlugin.APP_ID, ConstantesAppAplicacionPlugin.MODULO_APLICACIONES ) {
                //    Titulo = "Aplicaciones del sistema",
                //    Descripcion = "Permite administrar la configuración de las aplicaciones del sistema",
                //    Tipos = new List<Type> { typeof(Aplicacion) }
                //},
                //new ElementoAplicacion(ConstantesAppAplicacionPlugin.APP_ID, ConstantesAppAplicacionPlugin.MODULO_PLUGINS ) {
                //    Titulo = "Plugins del sistema",
                //    Descripcion = "Permite administrar la configuración de los pluigns del sistema",
                //    Tipos = new List<Type> { typeof(Plugin), typeof(VersionPlugin), typeof(PluginInstalado) }
                //}
            };
            return m;
        }

    }

}
