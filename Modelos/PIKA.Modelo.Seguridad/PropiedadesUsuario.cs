using PIKA.Modelo.Contacto;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using static PIKA.Modelo.Metadatos.PropAttribute;

namespace PIKA.Modelo.Seguridad
{
    // [EntidadVinculada(Entidad: "UnidadOrganizacional", Cardinalidad: TipoCardinalidad.UnoVarios, Padre: "Id", Hijo: "DominioId")]
    [Entidad(EliminarLogico: true)]
    public class PropiedadesUsuario
    {

        /// <summary>
        /// Identificador único del usuario
        /// </summary>
        public string UsuarioId { get; set; }

        /// <summary>
        /// Esta propeidad viene de la tabla aspnetuser en oeraciones GET, debe incluirse en POST
        /// </summary>
        public string username{ get; set; }

        /// <summary>
        ///  OIDC Claims se alamcenana en la tabla aspnetuserclaims
        /// </summary>
        public string email { get; set; }

        public string name { get; set; }

        public string family_name { get; set; }

        public string given_name { get; set; }

  
        public string middle_name { get; set; }


        public string nickname { get; set; }

        public DateTime? updated_at { get; set; }

        public bool? email_verified { get; set; }


        /// <summary>
        ///  Propeidades de la aplciación
        /// </summary>

        public string generoid { get; set; }
        public string paisid { get; set; }


        public string estadoid { get; set; }

        public string gmt { get; set; }


       public float? gmt_offset { get; set; }


        /// <summary>
        /// Especifica si la cuenta se encuentra ainactiva
        /// </summary>
         public bool Inactiva { get; set; }

        /// <summary>
        /// Especifica si la cuenta ha sido marcada para eliminar
        /// </summary>
        public bool Eliminada { get; set; }

        /// <summary>
        /// Esta propiedad se alamcena y debe obtenerse de la tabla de los claims del usuario 
        /// No debe incluirse en consultas debido a que puede degradar el performance
        /// </summary>
        [NotMapped]

        public string picture { get; set; }



        /// <summary>
        /// Esta propiedad solo es util al creal el suaurio y no debe persisttirse
        /// </summary>
        [NotMapped]
        public string password { get; set; }

        /// <summary>
        ///  Esta propeida se actualzai vía el login del usuario
        /// </summary>
        public DateTime? Ultimoacceso { get; set; }

        //public Pais pais { get; set; }
        //public Estado estado { get; set; }

        public Genero genero { get; set; }

        public ApplicationUser Usuario { get; set; }


    }
}
