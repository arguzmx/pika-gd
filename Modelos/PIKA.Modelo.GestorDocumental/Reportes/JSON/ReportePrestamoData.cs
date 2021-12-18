using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class ReportePrestamoData
    {
        public ReportePrestamoData()
        {
            Activos = new List<ActivoPrestado>();
        }

        public string Fecha { get; set; }
        public string FechaDevolucionPrevista { get; set; }
        public string FechaDevolucionReal { get; set; }
        public string Estado { get; set; }
        public Prestamo Prestamo { get; set; }
        
        /// <summary>
        /// Activos involucrados en el préstamo
        /// </summary>
        public List<ActivoPrestado> Activos { get; set; }
        
        /// <summary>
        /// Nombre del prestatario
        /// </summary>
        public UsuarioPrestamo Prestatario { get; set; }

        /// <summary>
        /// Nombre del prestadr
        /// </summary>
        public UsuarioPrestamo Prestador { get; set; }

    }

    public class ActivoPrestado: Activo
    {
        public string Indice { get; set; }
        public string Devuelto { get; set; }
    }


    public class UsuarioPrestamo
    {
        public string Nombre { get; set; }
        public string Puesto { get; set; }
        public string Email { get; set; }
        public string Telefono { get; set; }
    }

}
