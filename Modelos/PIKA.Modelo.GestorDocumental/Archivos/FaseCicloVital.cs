using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
   
    public class FaseCicloVital : EntidadCatalogo<string, FaseCicloVital>
    {
        public override List<FaseCicloVital> Seed()
        {
            List<FaseCicloVital> l = new List<FaseCicloVital>();
            l.Add(new FaseCicloVital() { Id = ConstantesArchivo.IDFASE_ACTIVA, Nombre = "Actima" });
            l.Add(new FaseCicloVital() { Id = ConstantesArchivo.IDFASE_SEMIACTIVA , Nombre = "Semiactiva" });
            l.Add(new FaseCicloVital() { Id = ConstantesArchivo.IDFASE_HISTORICA, Nombre = "Histórica" });
            return l;
        }

    }
}
