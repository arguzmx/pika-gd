using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data
{
    public class Estructuraexcel
    {
        public void ExportarCuadroCalsificacionExcel(string id)
        {
        }
        /// <summary>
        /// Idenificador del nùmero de columna
        /// </summary>
        public int NumeroCulumna { get; set; }
        /// <summary>
        /// Identidicador de Nùmero de Fila o renglòn
        /// </summary>
        public int NumeroRenglon { get; set; }
        /// <summary>
        /// La posiciòn coresponde a la letra de la columna que se asigna por medio del nivel obtenido en la regiòn 1
        /// </summary>
        public string PosicionColumna { get; set; }
        /// <summary>
        /// Valor de Renglon o el contenido que tendra el texto.
        /// </summary>
        public string ValorCelda { get; set; }


    }

}
