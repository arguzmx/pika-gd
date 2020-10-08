using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contacto;
using System;
using System.Collections.Generic;
using PIKA.Constantes.Aplicaciones.Contacto;

namespace PIKA.Servicio.Contacto
{
    public class AplicacionContacto : InformacionAplicacionBase, IInformacionAplicacion
    {
      

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }


        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_BASE);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_PAISES ) {
                    Titulo = "Catálogo de paises",
                    Descripcion = "Permite administrar el catálogo de paises del sistema",
                    Tipos = new List<Type> { typeof(Pais) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_ESTADOS ) {
                    Titulo = "Catálogo de estados",
                    Descripcion = "Permite administrar el catálogo de estados de los paises del sistema",
                    Tipos = new List<Type> { typeof(Estado) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_TIPO_MEDIOS ) {
                    Titulo = "Catálogo de medios de contacto",
                    Descripcion = "Permite administrar el catálogo de tipos de medios de contacto del sistema",
                    Tipos = new List<Type> { typeof(TipoMedio) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_TIPO_FUENTE ) {
                    Titulo = "Catálogo de fuentes de contacto",
                    Descripcion = "Permite administrar el catálogo de fuentes de contacto del sistema",
                    Tipos = new List<Type> { typeof(TipoFuenteContacto) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_DIR_POSTAL ) {
                    Titulo = "Direcciones postales",
                    Descripcion = "Permite la gestión de direcciones postales de las entidades del sistema",
                    Tipos = new List<Type> { typeof(DireccionPostal) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_MEDIO_CONTACTO ) {
                    Titulo = "Medios de contatco",
                    Descripcion = "Permite la gestión de medios de contacto de las entidades del sistema",
                    Tipos = new List<Type> { typeof(MedioContacto) }
                },
                new ElementoAplicacion(ConstantesAppContacto.MODULO_BASE, ConstantesAppContacto.MODULO_HOR_MEDIO_CONTACTO ) {
                    Titulo = "Horarios medios de contatco",
                    Descripcion = "Permite la gestión de los horarios de los medios de contacto de las entidades del sistema",
                    Tipos = new List<Type> { typeof(HorarioMedioContacto) }
                }
            };
            return m;
        }
 

    }
}
