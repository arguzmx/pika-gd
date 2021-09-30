using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public  class LinkVista
    {
        public virtual string Vista
        {
            get; set;
        }

        public virtual string Icono
        {
            get; set;
        }

        public virtual string Titulo
        {
            get; set;
        }

        public virtual string Condicion
        {
            get; set;
        }

        public virtual bool RequiereSeleccion
        {
            get; set;
        }

        public virtual TipoVista Tipo
        {
            get; set;
        }
    }
}
