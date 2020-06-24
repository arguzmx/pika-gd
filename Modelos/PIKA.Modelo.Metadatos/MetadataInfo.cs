using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class MetadataInfo
    {
        /// <summary>
        /// Tipo de elemento basado en el nombre del ensamblado
        /// </summary>
       public string Tipo { get; set; }

        /// <summary>
        /// Fullname del tipo desde el ensamblado
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Especifica si el elimnado de la entidad ocurre a nivel lógico
        /// </summary>
        public bool ElminarLogico { get; set; }

        public virtual List<Propiedad> Propiedades { get; set; }

        public virtual List<EntidadVinculada> EntidadesVinculadas { get; set; }


        public virtual bool PoseeJerarquia()
        {

            bool IdJ = Propiedades.Where(x => x.EsIdJerarquia == true).Any();
            bool TextoJ = Propiedades.Where(x => x.EsTextoJerarquia == true).Any();
            bool IdPadreJ = Propiedades.Where(x => x.EsIdPadreJerarquia == true).Any();
            return (IdJ && TextoJ && IdPadreJ);

        }

    }
}
