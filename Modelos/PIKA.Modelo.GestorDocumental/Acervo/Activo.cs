using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class Activo: Entidad<string>, IEntidadRelacionada
    {

        public Activo()
        {
            TipoOrigenId = TipoOrigenDefault;
        }

        public override string Id { get => base.Id; set => base.Id = value; }

        /// <summary>
        /// Los activos del inventario son propiedad de las unidades oragnizacionales
        /// y éstas a su vez pertenecen a un dominio lo que garantiza la cobertura de movivimentos
        /// </summary>
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL;

        /// <summary>
        /// En este ID 
        /// </summary>
        public string TipoOrigenId { get; set; }
        
        /// <summary>
        /// Alamcena el ID de la unidad organizaciónal creadora de la entrada
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Nombre de la entrada de inventario por ejemplo el número de expediente
        /// </summary>
        public string Nombre { get; set; }
        //# Longitud nombre, requeirdo

        /// <summary>
        /// Asunto de la entrada de inventario
        /// </summary>
        public string Asunto { get; set; }
        //# Longitus 2048, opcional

        /// <summary>
        /// Fecha de apertura UTC del activo
        /// </summary>
        public DateTime FechaApertura { get; set; }
        //#Requerido


        /// <summary>
        /// Fecha opcional de ciere del activo
        /// </summary>
        public DateTime? FechaCierre { get; set; }
        //#opcional

        /// <summary>
        /// Indica que el elemento se encuentra en formato electrónico desde su creación
        /// </summary>
        public bool EsElectronio { get; set; }

        /// <summary>
        /// Código de barras o QR de la entrada para ser leído por un scanner 
        /// </summary>
        public string CodigoOptico { get; set; }
        //# Longitus 1024, opcional


        /// <summary>
        /// Código electrónico de acceso al elemento por ejemplo RFID
        /// </summary>
        public string CodigoElectronico { get; set; }
        //# Longitus 1024, opcional

        /// <summary>
        /// Identificador único del ElementoClasificacion
        /// </summary>
        public string ElementoClasificacionId { get; set; }


        public ElementoClasificacion ElementoClasificacion { get; set; }
    }
}
