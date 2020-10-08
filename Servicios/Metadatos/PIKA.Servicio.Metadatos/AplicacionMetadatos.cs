using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using System;
using System.Collections.Generic;


namespace PIKA.Servicio.Metadatos
{
   public class AplicacionMetadatos : InformacionAplicacionBase, IInformacionAplicacion
    {
                  
 
        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.APP_ID);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS ) {
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
