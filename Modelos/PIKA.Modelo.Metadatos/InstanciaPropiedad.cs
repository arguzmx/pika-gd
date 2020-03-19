using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Metadatos
{
    public class InstanciaPropiedad
    {

        /// <summary>
        /// Identificador úico de la propiedad a la que pertenece la instancia
        /// </summary>
        public string PropiedadId { get; set; }


        /// <summary>
        /// Valores asignados a la propiedad
        /// </summary>
        public List<object> Valores { get; set; }

        /// <summary>
        /// Valor default de la propiedad
        /// </summary>
        public object Valor
        {
            get { return (Valores != null && Valores.Count > 0) ? Valores[0] : null; }
            set
            {
                if (Valores == null) Valores = new List<object>();
                if (Valores.Count == 0)
                {
                    Valores.Add(value);
                }
                else
                {
                    Valores[0] = value;
                }
            }
        }

        public void PushValue(object value)
        {
            if (Valores == null) Valores = new List<object>();
            Valores.Add(value);

        }

        /// <summary>
        /// Si el indice es cero lo agrega al inicio, si el indice es mayor que 
        /// la cuenta lo arega al final
        /// </summary>
        /// <param name="value"></param>
        /// <param name="index"></param>
        public void PushValueAt(object value, int index = 0)
        {
            if (Valores == null) Valores = new List<object>();
            if (Valores.Count < index)
            {
                Valores.Insert(index, value);
            }
            else
            {
                Valores.Add(value);
            }
        }

    }
}
