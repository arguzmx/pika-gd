using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos.Atributos
{
    public class EntidadMiembroAttribute : Attribute
    {
        
        private string _ObjetoMiembro;
        private string _ObjetoMembresia;
        private string _ColumnaIdMiembro;
        private string _ColumnaIdMembresia;
        private string _PropiedadPadre;

        /// <summary>
        /// Establecer uan relación de membresia entre el objeto padre y sus entidades  miembro
        /// </summary>
        /// <param name="ObjetoMiembro">Nombre del objeto miembro del objeto padre</param>
        /// <param name="ObjetoMembresia">NOmbre del objeto que administra la membresía</param>
        /// <param name="ColumnaIdMiembro">Columna Identificador del objeto miembre</param>
        /// <param name="ColumnaIdMembresia">Columna o propiedad del padre de la membresía</param>
        public EntidadMiembroAttribute(string PropiedadPadre, string ObjetoMiembro, string ObjetoMembresia, 
            string ColumnaIdMiembro, string ColumnaIdMembresia)
        {
            _ObjetoMiembro = ObjetoMiembro;
            _ColumnaIdMiembro = ColumnaIdMiembro;
            _ObjetoMembresia = ObjetoMembresia;
            _ColumnaIdMembresia = ColumnaIdMembresia;
            _PropiedadPadre = PropiedadPadre;
        }

        public string ObjetoMiembro
        {
            get { return _ObjetoMiembro; }
        }

        public string ObjetoMembresia
        {
            get { return _ObjetoMembresia; }
        }

        public string ColumnaIdMiembro
        {
            get { return _ColumnaIdMiembro; }
        }


        public string ColumnaIdMembresia
        {
            get { return _ColumnaIdMembresia; }
        }

        public string PropiedadPadre
        {
            get { return _PropiedadPadre; }
        }
    }
}
