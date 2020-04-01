﻿using PIKA.Modelo.Metadatos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Organizacion
{

    /// <summary>
    /// Las unidades organizacionales agrupan recursos para la organización del trabajo
    /// </summary>
    public class UnidadOrganizacional : Entidad<string>, IEntidadNombrada, IEntidadEliminada
    {

        [Prop(OrderIndex: 0, IsId: true, Required: true, Autogenerated: false,  
            HTMLControl: PropAttribute.HTML_HIDDEN, isHieId: true)]
        [Tbl(OrderIndex: 0, Visible: false)]
        public override string Id { get => base.Id; set => base.Id = value; }


        [Prop(OrderIndex: 1, Required: true, isHieText:true, Visible: true)]
        [Tbl(OrderIndex: 1, Visible: true)]
        [ValidString(minlen: 1, maxlen: 100)]
        /// <summary>
        /// NOmbre de la unodad organizacional
        /// </summary>
        public string Nombre {get; set;}


        [Prop(OrderIndex: 2 ,actions: PropAttribute.CRUDActions.delete, Visible: true, HTMLControl: PropAttribute.HTML_CHECKBOX, DefaultValue: false)]
        [Tbl(OrderIndex: 2)]
        /// <summary>
        /// Destermina si la unidad ha sido eliminada
        /// </summary>
        public bool Eliminada {get; set;}

        /// <summary>
        /// Identiicador único del cominio al que se asocia la UO
        /// </summary>
        public string DominioId { get; set; }


        public Dominio Dominio { get; set; }

      
    }
}
