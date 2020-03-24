using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class EstadoCuadroClasificacion : EntidadCatalogo<string, EstadoCuadroClasificacion>
    {
        public EstadoCuadroClasificacion()
        {
            Cuadros = new HashSet<CuadroClasificacion>();
        }
        public override List<EstadoCuadroClasificacion> Seed() {
            List<EstadoCuadroClasificacion> l = new List<EstadoCuadroClasificacion>();
            l.Add(new EstadoCuadroClasificacion() { Id = ConstantesEstado.ESTADO_ACTIVO, Nombre = "Activo" });
            l.Add(new EstadoCuadroClasificacion() { Id = ConstantesEstado.ESTADO_INACTIVO, Nombre = "Inactivo" });
            return l;
        }

        public IEnumerable<CuadroClasificacion> Cuadros { get; set; }
    }
}
