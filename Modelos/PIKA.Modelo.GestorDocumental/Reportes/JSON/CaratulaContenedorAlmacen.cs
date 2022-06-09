using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Reportes.JSON
{
    public class CaratulaContenedorAlmacen
    {
        public string Dominio { get; set; }
        public string UnidadOrganizacional { get; set; }
        public ContenedorAlmacen Contenedor { get; set; }
        public ZonaAlmacen ZonaAlmacen { get; set; }
        public PosicionAlmacen PosicionAlmacen { get; set; }
        public AlmacenArchivo AlmacenArchivo { get; set; }
        public Archivo Archivo { get; set; }

    }
}
