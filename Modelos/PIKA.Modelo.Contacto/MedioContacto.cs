using PIKA.Infraestructura.Comun;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contacto
{
    public class MedioContacto: Entidad<string>, IEntidadRelacionada
    {
        public MedioContacto()
        {
            this.Horarios = new HashSet<HorarioMedioContacto>();
        }


        [XmlIgnore]
        [JsonIgnore]
        public string TipoOrigenDefault => ConstantesModelo.IDORIGEN_NULO;

        /// <summary>
        ///  Tipo de origen del medio de contato por ejemplo una oragnización persona
        /// </summary>
        public string TipoOrigenId { get; set; }

        /// <summary>
        /// Identificador de origen del contato por ejemplo el Id de la organización o la persona
        /// </summary>
        public string OrigenId { get; set; }

        /// <summary>
        /// Tipo de medio de contacto
        /// </summary>
        public string TipoMedioId { get; set; }

        /// ^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^^ 
        /// Los tres campos anteriores deben utilizarse para construir un índice 
        /// en la base de datos


        /// <summary>
        /// Id del de la fuente para contacto por ejempolo casa, trabajo, personal
        /// </summary>
        public string TipoFuenteContactoId { get; set; }

        /// <summary>
        /// Determina si el medio de conatcto es el principal para un tipo de medio y fuente específico
        /// Por ejemplo si es el teléfon o principal del trabajo
        /// </summary>
        public bool Principal { get; set; }

        /// <summary>
        /// Identifica si el medio de contacto se encuentra activo
        /// </summary>
        public bool Activo { get; set; }
        // Default = true

        /// <summary>
        /// Identificador del medio por ejemplo la dirección de correo o el 
        /// número telefónico
        /// </summary>
        public string Medio { get; set; }
        // Requerido longitud 500

        /// <summary>
        /// Prefijo para el medio si es  requerido como el código de país en
        /// el caso de un teléfono
        /// </summary>
        public string Prefijo { get; set; }
        // opcional longitud 100

        /// <summary>
        /// Sufijo para el medio por ejemeplo la extensión en el caso
        /// de un número telefónico
        /// </summary>
        public string Sufijo { get; set; }
        // opcional longitud 100

        /// <summary>
        /// Npotas acoasidas al medio de contacto
        /// </summary>
        public string Notas { get; set; }
        // opcional longitud =500

        /// <summary>
        /// Lista de horarios asociados al medio de conatcto
        /// </summary>
        public ICollection<HorarioMedioContacto> Horarios { get; set; }

        /// <summary>
        /// Tipo de la fuente para contacto por ejempolo casa, trabajo, personal
        /// </summary>
        public TipoFuenteContacto TipoFuenteContacto { get; set; }

        /// <summary>
        /// Tipo medio contacto
        /// </summary>
        public TipoMedio TipoMedio { get; set; }

    }
}
