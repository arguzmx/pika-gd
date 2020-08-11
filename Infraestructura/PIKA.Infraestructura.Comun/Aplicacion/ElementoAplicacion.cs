using PIKA.Infraestructura.Comun.Constantes;
using PIKA.Infraestructura.Comun.Seguridad;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public class ElementoAplicacion
    {

        private string _IdPadre;
        private string _IdModulo;
        public  ElementoAplicacion(string IdPadre, string IdModulo)
        {
            _IdPadre = IdPadre;
            _IdModulo = IdModulo;
        }

        public string IdModulo { get { return $"{_IdPadre}{_IdModulo}"; }  }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public List<Type> Tipos { get; set; }

        public static ModuloAplicacion CreaModuloTipico(string AppId, string IdModuloRaiz, string IdModulo,
                 string Titulo, string Descripcion, List<Type> Tipos)
        {
            ModuloAplicacion m = new ModuloAplicacion(AppId, IdModulo, true,
              Titulo,
              Descripcion,
              "",
              "es-MX",
              PermisoAplicacion.PermisosAdministrables(), IdModuloRaiz,
              ConstantesAplicacion.Id);

            m.TiposAdministrados.Add(new TipoAdministradorModulo()
            {
                AplicacionId = ConstantesAplicacion.Id,
                ModuloId = m.Id,
                TiposAdministrados = Tipos
            });

            return m;
        }


    }
}
