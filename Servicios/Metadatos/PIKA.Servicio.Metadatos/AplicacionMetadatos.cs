using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Constantes.Aplicaciones.Metadatos;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Seguridad.Auditoria;
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

        public enum EventosAdicionales
        {

        }

        private List<TipoEventoAuditoria> EventosPlantillasConfig()
        {
            List<TipoEventoAuditoria> l = new List<TipoEventoAuditoria>();
            l.AddRange("EV-METADATA-PLANTILLA".EventoComunes<Plantilla>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            l.AddRange("EV-METADATA-PROPIEDADPLANTILLA".EventoComunes<PropiedadPlantilla>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            //l.AddRange("EV-PM".EventoComunes<AtributoTabla>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            //l.AddRange("EV-PERMISOS-PM".EventoComunes<TipoDato>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            l.AddRange("METADATA-PLANTILLA-NUMVAL".EventoComunes<ValidadorNumero>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            l.AddRange("METADATA-PLANTILLA-TEXTVAL".EventoComunes<ValidadorTexto>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            //l.AddRange("EV-VOLUMEN".EventoComunes<ValorListaPlantilla>(ConstantesAppMetadatos.APP_ID, ConstantesAppMetadatos.MODULO_PLANTILLAS));
            return l;
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
                        typeof(ValidadorNumero),
                        typeof(ValidadorTexto),
                        typeof(ValorListaPlantilla),
                        typeof(TipoDato),
                        //typeof(AtributoTabla),
                        

                    },
                     EventosAuditables = EventosPlantillasConfig()
                }

            };
            return m;
        }
    }
}
