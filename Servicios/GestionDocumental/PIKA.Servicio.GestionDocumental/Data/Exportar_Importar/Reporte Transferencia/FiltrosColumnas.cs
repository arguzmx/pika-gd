using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental.Data.Exportar_Importar.Reporte_Transferencia
{
    public class Columnas 
    {
        public Columnas()
        {

        }

        public  string SEPARADOR_VALORES = ",";
        public  string Nombre = "Nombre";
        public  string Consecutio = "Consecutio";
        public  string EntradaClasificacionC = "EntradaClasificacion.Clave";
        public  string EntradaClasifiacionNombre = "EntradaClasificacion.Nombre";
        public  string Asunto = "Asunto";
        public  string FechaAperuta = "FechaAperuta";
        public  string FechaCierre = "FechaCierre";
        public  string CodigoOptico = "CodigoOptico";
        public  string CodigoElectronico = "CodigoElectronico";
        public  string Reservado = "Reservado";
        public  string Confidencial = "Confidencial";
        public  string Ampliado = "Ampliado";
        public string TransferenciaId { get; set; }

    }
    
}
