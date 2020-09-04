using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Contacto
{
    public class AplicacionContacto : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-CONTACTO";

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
                new ElementoAplicacion(MODULO_BASE, "PAISES" ) {
                    Titulo = "Catálogo de paises",
                    Descripcion = "Permite administrar el catálogo de paises del sistema",
                    Tipos = new List<Type> { typeof(Pais) }
                },
                new ElementoAplicacion(MODULO_BASE, "ESTADOS" ) {
                    Titulo = "Catálogo de estados",
                    Descripcion = "Permite administrar el catálogo de estados de los paises del sistema",
                    Tipos = new List<Type> { typeof(Estado) }
                },
                new ElementoAplicacion(MODULO_BASE, "TIPO-MEDIOS" ) {
                    Titulo = "Catálogo de medios de contacto",
                    Descripcion = "Permite administrar el catálogo de tipos de medios de contacto del sistema",
                    Tipos = new List<Type> { typeof(TipoMedio) }
                },
                new ElementoAplicacion(MODULO_BASE, "TIPO-FUENTE" ) {
                    Titulo = "Catálogo de fuentes de contacto",
                    Descripcion = "Permite administrar el catálogo de fuentes de contacto del sistema",
                    Tipos = new List<Type> { typeof(TipoFuenteContacto) }
                },
                new ElementoAplicacion(MODULO_BASE, "DIR-POSTAL" ) {
                    Titulo = "Direcciones postales",
                    Descripcion = "Permite la gestión de direcciones postales de las entidades del sistema",
                    Tipos = new List<Type> { typeof(DireccionPostal) }
                },
                new ElementoAplicacion(MODULO_BASE, "MEDIO-CONTACO" ) {
                    Titulo = "Medios de contatco",
                    Descripcion = "Permite la gestión de medios de contacto de las entidades del sistema",
                    Tipos = new List<Type> { typeof(MedioContacto) }
                },
                new ElementoAplicacion(MODULO_BASE, "MEDIO-CONTACO" ) {
                    Titulo = "Horarios sedios de contatco",
                    Descripcion = "Permite la gestión de los horarios de los medios de contacto de las entidades del sistema",
                    Tipos = new List<Type> { typeof(HorarioMedioContacto) }
                }
            };
            return m;
        }
 

    }
}
