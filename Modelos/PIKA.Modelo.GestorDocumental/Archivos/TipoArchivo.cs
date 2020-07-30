using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
   


    public class TipoArchivo : EntidadCatalogo<string, TipoArchivo>
    {

        public TipoArchivo()
        {
            Archivos = new HashSet<Archivo>();
        }
        public override List<TipoArchivo> Seed()
        {
            List<TipoArchivo> l = new List<TipoArchivo>();
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_CORRESPONDENCIA, Nombre = "Correspondencoia", FaseCicloVitalId = ConstantesArchivo.IDFASE_ACTIVA  });
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_TRAMITE  , Nombre = "Trámite", FaseCicloVitalId = ConstantesArchivo.IDFASE_ACTIVA });
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_HISTORICO  , Nombre = "Histórico", FaseCicloVitalId = ConstantesArchivo.IDFASE_HISTORICA });
            l.Add(new TipoArchivo() { Id = ConstantesArchivo.IDARCHIVO_CONSERVACION , Nombre = "Conservación", FaseCicloVitalId = ConstantesArchivo.IDFASE_SEMIACTIVA });
            return l;
        }

        /// <summary>
        /// IDentificador de la fase del ciclo de vida al que pertenece el archivo
        /// </summary>
        public string FaseCicloVitalId { get; set; }


        public IEnumerable<Archivo> Archivos { get; set; }

    }
}
