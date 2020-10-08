using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Reportes;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Reportes
{
   public class AplicacionReporteEntidad : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-REPORTE";

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
                new ElementoAplicacion(MODULO_BASE, "REPORTE" ) {
                    Titulo = "Repoertes por entidad",
                    Descripcion = "Permite administrar los Reportes",
                    Tipos = new List<Type> { typeof(ReporteEntidad)
                        
                    }
                },
              };
            return m;
        }

    }
}
