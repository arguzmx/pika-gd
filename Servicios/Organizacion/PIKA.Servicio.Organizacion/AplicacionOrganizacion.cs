using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;

namespace PIKA.Servicio.Organizacion
{
    public class AplicacionOrganizacion : InformacionAplicacionBase, IInformacionAplicacion
    {
    

        public override Aplicacion Info()
        {
            Aplicacion a = AplicacionRaiz.ObtieneAplicacionRaiz();
            a.Modulos = this.ModulosAplicacion();
            return a;
        }

        public List<ModuloAplicacion> ModulosAplicacion()
        {
            return this.ModulosAplicacionLocales(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.APP_ID);
        }

        public override List<ElementoAplicacion> GetModulos()
        {
            List<ElementoAplicacion> m = new List<ElementoAplicacion>()
            {
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO ) {
                    Titulo = "Dominios",
                    Descripcion = "Permite administrar los dominios para la conformación de organizaciones",
                    Tipos = new List<Type> { typeof(Dominio) }
                },
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_UNIDADORGANIZACIONAL ) {
                    Titulo = "Unidades organizacionales",
                    Descripcion = "Permite administrar las unidades organizacionales de un dominio",
                    Tipos = new List<Type> { typeof(UnidadOrganizacional) }
                },
                new ElementoAplicacion(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL ) {
                    Titulo = "Roles",
                    Descripcion = "Permite administrar los roles de usuario de un dominio",
                    Tipos = new List<Type> { typeof(Rol), typeof(UsuariosRol) }
                }
            };
            return m;
        }

    }
}
