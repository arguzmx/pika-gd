using System;
using System.Collections.Generic;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;

namespace PIKA.Servicio.AplicacionPlugin
{

    public class AplicacionAplicaciones : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-APLICACIONES-";
        public const string MODULO_APPS = "PIKA-GD-APLICACIONES-APLICACIONES";

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
                new ElementoAplicacion(MODULO_BASE, "APLICACIONES" ) {
                    Titulo = "Aplicaciones del sistema",
                    Descripcion = "Permite administrar la configuración de las aplicaciones del sistema",
                    Tipos = new List<Type> { typeof(Aplicacion) }
                }
            };
            return m;
        }

       
    }


}
