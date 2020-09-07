using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.Organizacion
{
    public class AplicacionOrganizacion : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const  string MODULO_BASE = "PIKA-GD-ORG";



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
                new ElementoAplicacion(MODULO_BASE, "DOMINIO" ) {
                    Titulo = "Dominios",
                    Descripcion = "Permite administrar los dominios para la conformación de organizaciones",
                    Tipos = new List<Type> { typeof(Dominio) }
                },
                new ElementoAplicacion(MODULO_BASE, "UNIDADORGANIZACIONAL" ) {
                    Titulo = "Unidades organizacionales",
                    Descripcion = "Permite administrar las unidades organizacionales de un dominio",
                    Tipos = new List<Type> { typeof(UnidadOrganizacional) }
                },
                new ElementoAplicacion(MODULO_BASE, "ROL" ) {
                    Titulo = "Roles",
                    Descripcion = "Permite administrar los roles de usuario de un dominio",
                    Tipos = new List<Type> { typeof(Rol), typeof(UsuariosRol) }
                }
            };
            return m;
        }

    }
}
