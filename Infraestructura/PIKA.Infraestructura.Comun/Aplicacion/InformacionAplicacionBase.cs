using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun
{
    public abstract class InformacionAplicacionBase
    {
        public virtual List<ElementoAplicacion> GetModulos()
        {
            throw new NotImplementedException();
        }

        public virtual Aplicacion Info()
        {
            throw new NotImplementedException();
        }


        public List<ModuloAplicacion> ModulosAplicacionLocales(string appId, string moduloBase)
        {
            List<ModuloAplicacion> l = new List<ModuloAplicacion>();

            foreach (var item in GetModulos())
            {
                ModuloAplicacion m = ElementoAplicacion.CreaModuloTipico(appId, moduloBase,
                    item.IdModulo, item.Titulo, item.Descripcion, item.Tipos);


                m.EventosAuditables = item.EventosAuditables;

                l.Add(m);
            }

            return l;
        }

        public List<TipoAdministradorModulo> TiposAdministrados()
        {

            List<TipoAdministradorModulo> tipos = new List<TipoAdministradorModulo>();
            Aplicacion a = this.Info();

            foreach (var m in a.Modulos)
            {

                foreach (var t in m.TiposAdministrados)
                {
                    tipos.Add(t);
                }
            }

            return tipos;
        }
    }
}
