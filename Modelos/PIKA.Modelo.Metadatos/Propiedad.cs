﻿using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
  
    public class Propiedad: Entidad<string>, IEntidadNombrada
    {

        public Propiedad()
        {
            Atributos = new HashSet<AtributoMetadato>();
        }

        /// <summary>
        /// Identificador único de la propiedad
        /// </summary>
        public override string Id { get; set; }

        /// <summary>
        /// Nombre asociado a la propiedad
        /// </summary>
        public string Nombre { get; set; }

        /// <summary>
        /// Identificador del tipo de propiedad
        /// </summary>
        public string TipoDatoId { get; set; }

    [NotMapped]
        /// <summary>
        /// Valor por defecto de la propiedad
        /// </summary>
        public object ValorDefault { get; set; }

        /// <summary>
        /// Índice de despliegue para la propiedad
        /// </summary>
        public int IndiceOrdenamiento { get; set; }

        /// <summary>
        /// Determina si es posible realizar bpsuquedas sobre la propiedad
        /// </summary>
        public bool Buscable { get; set; }

        /// <summary>
        /// Determina si es posible realizar ordenamiento sobre la propiedad
        /// </summary>
        public bool Ordenable { get; set; }

        /// <summary>
        /// Determina si la propieda es vivisble
        /// </summary>
        public bool Visible { get; set; }

        /// <summary>
        /// DEtermina si es una clave primaria
        /// </summary>
        public bool EsIdClaveExterna { get; set; }
        
  

        /// <summary>
        /// Dettermina si es el identificador de un registro
        /// </summary>
        public bool EsIdRegistro { get; set; }



        /// <summary>
        /// Dettermina si es el identificador de una jeraquía
        /// </summary>
        public bool EsIdJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde al texto de un registro
        /// </summary>
        public bool EsTextoJerarquia { get; set; }

        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        public bool EsIdPadreJerarquia { get; set; }


        /// <summary>
        /// Dettermina si coreresponde a la propiedad padre para una relacion jerárquica
        /// </summary>
        public bool EsFiltroJerarquia { get; set; }


        /// <summary>
        /// DEtermina si es requerido
        /// </summary>
        public bool Requerido { get; set; }

        /// <summary>
        /// Determina si el valos es autogenerado
        /// </summary>
        public bool Autogenerado { get; set; }

        /// <summary>
        /// Determina si el valos es un campo de índice 
        /// </summary>
        public bool EsIndice { get; set; }


        /// <summary>
        /// Especifica el control de HTML preferido
        /// </summary>
        public string ControlHTML { get; set; }

        [NotMapped]
        public virtual TipoDato TipoDato { get; set; }
        [NotMapped]
        public virtual AtributoTabla AtributoTabla { get; set; }
        [NotMapped]
        public virtual ValidadorTexto ValidadorTexto { get; set; }
        [NotMapped]
        public virtual ValidadorNumero ValidadorNumero { get; set; }
        [NotMapped]
        public virtual ICollection<AtributoMetadato> Atributos { get; set; }

    }
}
