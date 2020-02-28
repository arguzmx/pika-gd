using Microsoft.AspNetCore.Mvc;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PIKA.GD.API
{
    public class ACLController : ControllerBase
    {
        public string TenatId{ get; set; }
        public string UsuarioId { get; set; }
        public string DominioId { get; set; }


        public List<FiltroConsulta> ObtieneFiltrosIdentidad() {
            List<FiltroConsulta> l = new List<FiltroConsulta>();
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "UsuarioId", Valor = UsuarioId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "TenatId", Valor = TenatId });
            l.Add(new FiltroConsulta() { Operador = FiltroConsulta.OP_EQ, Propiedad = "DominioId", Valor = DominioId });
            return l;
        }

    } 

}
