using PIKA.Modelo.GestorDocumental.Temas;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class ActivoSeleccionado
    {
        public string  UsuarioId { get; set; }
        public string TemaId { get; set; }
        public string Id { get; set; }
        public Activo  Activo { get; set; }
        public TemaActivos TemaActivos { get; set; }

    }
}
