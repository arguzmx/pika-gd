using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Metadatos
{
   public class AplicacionMetadatos : InformacionAplicacionBase, IInformacionAplicacion
    {
                  
        public const string MODULO_BASE = "PIKA-GD-MET";
 
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
                new ElementoAplicacion(MODULO_BASE, "PLANTILLAS" ) {
                    Titulo = "Plantillas de metadatos",
                    Descripcion = "Permite gestionar plantillas de metadatos para los objetos del sistema",
                    Tipos = new List<Type> { typeof(Plantilla), 
                        typeof(PropiedadPlantilla), 
                        typeof(AtributoTabla),
                        typeof(TipoDato), 
                        typeof(ValidadorNumero), 
                        typeof(ValidadorTexto)
                    }
                }
            };
            return m;
        }
    }
}
