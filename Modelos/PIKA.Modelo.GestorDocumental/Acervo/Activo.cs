using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.GestorDocumental
{
    public class Activo: Entidad<string>, IEntidadRelacionada, IEntidadIdElectronico
    {

        public Activo()
        {
            TipoOrigenId = TipoOrigenDefault;
            HistorialArchivosActivo = new HashSet<HistorialArchivoActivo>();
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
        public virtual Asunto oAsunto { get; set; }
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


        /// <summary>
        /// Identificador único del archivo actual del activo
        /// </summary>
        public string ArchivoId { get; set; }
        //# la relacion del actuivo con archovo es de 1 a 1, un activo puede estar en un sólo archivo al mismo tiempo

        /// <summary>
        /// Indica si el activo se encuentra en préstamo
        /// </summary>
        public bool EnPrestamo { get; set; }


        /// <summary>
        /// Especifica si el activo se encuentra marcado como en reserva
        /// </summary>
        public bool Reservado { get; set; }

        /// <summary>
        /// Especifica si el activo se encuentra marcado como confidenxial
        /// </summary>
        public bool Confidencial { get; set; }

        /// <summary>
        /// Especifica si el activo tiene ampliaciones vigentes
        /// </summary>
        public bool Ampliado { get; set; }


        public ElementoClasificacion ElementoClasificacion { get; set; }

        public Archivo ArchivoActual { get; set; }

        /// <summary>
        /// Historial de archivos por los que ha pasado el activo
        /// </summary>
        public virtual ICollection<HistorialArchivoActivo> HistorialArchivosActivo { get; set; }

        public virtual ICollection<Ampliacion> Ampliaciones { get; set; }

        public virtual ActivoPrestamo Prestamo { get; set; }
    }
}
