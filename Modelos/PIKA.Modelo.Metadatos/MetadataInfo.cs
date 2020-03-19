using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class MetadataInfo
    {
        public Type Tipo { get; set; }
        public virtual List<Propiedad> Propiedades { get; set; }


        public virtual bool PoseeJerarquia()
        {

            bool IdJ = Propiedades.Where(x => x.EsIdJerarquia == true).Any();
            bool TextoJ = Propiedades.Where(x => x.EsTextoJerarquia == true).Any();
            bool IdPadreJ = Propiedades.Where(x => x.EsIdPadreJerarquia == true).Any();
            return (IdJ && TextoJ && IdPadreJ);

        }

    }
}
