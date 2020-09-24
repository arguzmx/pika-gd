using System;
using System.Collections.Generic;
using System.Text;
using PIKA.Infraestructura.Comun;
using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using PIKA.Modelo.Contenido;
using Version = PIKA.Modelo.Contenido.Version;

namespace PIKA.Servicio.Contenido
{
    public class AplicacionContenido : InformacionAplicacionBase, IInformacionAplicacion
    {
        public const string MODULO_BASE = "PIKA-GD-COTENIDO-";

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
                new ElementoAplicacion(MODULO_BASE, "ADMIN-CONFIGURACION" ) {
                    Titulo = "Configuración de almacenamiento de los repositorios",
                    Descripcion = "Permite administrar la configuración de los almacenamientos del repositorio",
                    Tipos = new List<Type> {
                        typeof(GestorAzureConfig),
                        typeof(GestorLocalConfig),
                        typeof(VolumenPuntoMontaje),
                        typeof(PuntoMontaje),
                        typeof(TipoGestorES),
                        typeof(Volumen),
                        typeof(GestorLocalConfig)
                    }
                },
                 new ElementoAplicacion(MODULO_BASE, "ESTRUCTURA-CONTENIDO" ) {
                    Titulo = "Gestión de elementos de contenido",
                    Descripcion = "Permite realizar la gestión de contenido del repositorio, por ejemplo carpetas y documentos",
                    Tipos = new List<Type> {
                        typeof(Carpeta),
                        typeof(Parte),
                        typeof(Version),
                        typeof(Permiso),
                        typeof(DestinatarioPermiso),
                        typeof(Elemento)
                    }
                },
                     new ElementoAplicacion(MODULO_BASE, "VISOR-CONTENIDO" ) {
                    Titulo = "Acceso al visor de contenido",
                    Descripcion = "Permite acceder al visor de cotenids para su visualización y edición",
                    Tipos = new List<Type> {}
                }

            };



            return m;
        }


    }

}
