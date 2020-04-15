using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental.Topologia
{
    public class Estante : Entidad<string>, IEntidadNombrada, IEntidadIdElectronico
    {
        public Estante()
        {
            Espacios = new HashSet<EspacioEstante>();
        }

        public string AlmacenArchivoId { get; set; }
        public override string Id { get; set; }
        
        /// <summary>
        /// Nombre comun del rack para uso humano
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Codigo de barras para el rack
        /// </summary>
        public string CodigoOptico { get; set; }
        //# Logintud 2048, opcional

        /// <summary>
        /// Código electrónico para el rack por ejemplo de RFID
        /// </summary>
        public string CodigoElectronico { get; set; }
        //# Logintud 2048, opcional

        public virtual AlmacenArchivo Almacen { get; set; }

        public ICollection<EspacioEstante> Espacios { get; set; }
    }
}
