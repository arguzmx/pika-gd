using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Constantes.Aplicaciones.GestorDocumental;
using PIKA.Constantes.Aplicaciones.Organizacion;
using PIKA.Constantes.Aplicaciones.Seguridad;
using PIKA.Infraestructura.Comun.Menus;
using PIKA.Servicio.Seguridad.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Seguridad.Modelos
{
    public class MenuAplicacionPIKAAngular : IServicioMenuAplicacion
    {

        public async Task<MenuAplicacion> ObtieneMenuApp(string Id)
        {

                await Task.Delay(1);
                MenuAplicacion m = new MenuAplicacion() { AppId = Id, AppNombre = "Pika Gestion Documetal" };
                m.Elementos = this.ObtieneElementosPIKA();
            return m;

        }

        private List<ElementoMenu> ObtieneElementosPIKA()
        {
            List<ElementoMenu> l = new List<ElementoMenu>();

            ElementoMenu el = CreaElemento(0, "home-outline", "Home", "/pages/iot-dashboard");
            l.Add(el);

            l.Add(CreaElemento(10, "", "OPCIONES","", true));

            l.Add(CreatElementoGestionDocumental(20));

            l.Add(CreatElementoContenido(30));

            l.Add(CreatElementoOrganizacion(40));

            l.Add(CreatElementoConfigSistema(50));

            return l;
        }
        private ElementoMenu CreatElementoConfigSistema(int index)
        {
            ElementoMenu gd = CreaElemento(index, "settings-2-outline", "Configuración sistema", "");

            ElementoMenu el = CreaElemento(5, "", "Permisos", "/pages/permisos");
            el.TokenSeguridad =  ModuloId(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_ACL);
            gd.Hijos.Add(el);

            return gd;
        }




        private ElementoMenu CreatElementoOrganizacion(int index)
        {
          

            ElementoMenu gd = CreaElemento(index, "map-outline", "Configuración organización", "");

            ElementoMenu el = CreaElemento(5, "", "Dominios", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "dominio" });
            el.TokenSeguridad = ModuloId(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_DOMINIO);
            gd.Hijos.Add(el);

            el = CreaElemento(10, "", "Unidades organizacionales", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "unidadorganizacional" });
            el.TokenSeguridad = ModuloId(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_UNIDADORGANIZACIONAL);
            gd.Hijos.Add(el);

            el = CreaElemento(15, "", "Roles", "/pages/tabular/");
            el.TokenSeguridad =  ModuloId(ConstantesAppOrganizacion.APP_ID, ConstantesAppOrganizacion.MODULO_ROL);
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "rol" });
            gd.Hijos.Add(el);



            el = CreaElemento(20, "", "Usuarios", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "propiedadesusuario" });
            el.TokenSeguridad = ModuloId(ConstantesAppSeguridad.APP_ID, ConstantesAppSeguridad.MODULO_USUARIOS);
            gd.Hijos.Add(el);

            el = CreaElemento(20, "", "Archivos", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "archivo" });
            el.TokenSeguridad = ModuloId(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ARCHIVOS);
            gd.Hijos.Add(el);
            

            el = CreaElemento(5, "", "Volumenes", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "volumen" });
            el.TokenSeguridad = ModuloId(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ADMIN_CONFIGURACION);
            gd.Hijos.Add(el);


            el = CreaElemento(5, "", "Catálogo tipo archivo", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "tipoarchivo" });
            el.TokenSeguridad =  ModuloId(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CAT_ARCHIVO);
            gd.Hijos.Add(el);

            return gd;
        }


        private ElementoMenu CreatElementoContenido(int index)
        {
            
            ElementoMenu gd = CreaElemento(index, "attach-outline", "Contenido", "");
            

            ElementoMenu el = CreaElemento(5, "", "Repositorios", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "puntomontaje" });
            el.TokenSeguridad =  ModuloId(ConstantesAppContenido.APP_ID, ConstantesAppContenido.MODULO_ESTRUCTURA_CONTENIDO);
            gd.Hijos.Add(el);


            return gd;
        }

        
        private ElementoMenu CreatElementoGestionDocumental(int index)
        {
         

            ElementoMenu gd = CreaElemento(index, "archive-outline", "Gestión documental", "");

            ElementoMenu el = CreaElemento(5, "", "Cuadros de clasificación", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "cuadroclasificacion" });
            el.TokenSeguridad =  ModuloId(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_CUADROCLASIF);
            gd.Hijos.Add(el);

            el = CreaElemento(10, "", "Inventario de expedientes", "/pages/tabular/");
            el.Parametros.Add(new ParametroMenu() { Id = "tipo", Valor = "activo" });
            el.TokenSeguridad =  ModuloId(ConstantesAppGestionDocumental.APP_ID, ConstantesAppGestionDocumental.MODULO_ACTIVOS);
            gd.Hijos.Add(el);

            return gd;
        }


        private string ModuloId(string modulobase, string modulo)
        {
            return $"{modulobase}-{modulo}";
        }

        private ElementoMenu CreaElemento(int Indice, string Icono, string Titulo, string URL, bool EsGrupo =false)
        {
            return new ElementoMenu()
            {
                Indice = Indice,
                EsGrupo = EsGrupo,
                Hijos = new List<ElementoMenu>(),
                Icono = Icono,
                Parametros = new List<ParametroMenu>(),
                TokenSeguridad = "",
                Titulo = Titulo,
                URL = URL
            };
        }
        

    }
}
