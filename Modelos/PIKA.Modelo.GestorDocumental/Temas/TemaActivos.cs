using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Temas
{
    public class TemaActivos
    {
        public TemaActivos() {
            ActivosSeleccionados = new HashSet<ActivoSeleccionado>();
        }

        public string Id { get; set; }
        public string UsuarioId { get; set; }
        public string Nombre { get; set; }
        public virtual IEnumerable<ActivoSeleccionado> ActivosSeleccionados { get; set; }
    }
}
