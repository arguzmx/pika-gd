using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.eventos
{
    public class EventoContenedorAlmacenExtraerActivo
    {
        public string ActivoId { get; set; }
        public decimal OcupacionInicial { get; set; }
        public decimal OcupacionFinal { get; set; }
    }
}
