using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Infraestructura.Comun.Seguridad
{


    public class MascaraPermisos
    {
        public const int PDenegarAcceso = 1;
        public const int PLeer = 2;
        public const int PEscribir = 4;
        public const int PEliminar = 8;
        public const int PAdministrar = 16;
        public const int PEjecutar = 32;


        public MascaraPermisos()
        {
            Leer = false;
            Escribir = false;
            Eliminar = false;
            Admin = false;
            Ejecutar = false;
            NegarAcceso = false;
        }


        public bool Leer { get; set; }

        public bool NegarAcceso { get; set; }

        public bool Escribir { get; set; }

        public bool Eliminar { get; set; }

        public bool Admin { get; set; }

        public bool Ejecutar { get; set; }



        /// <summary>
        /// Devuelve la lsita de permisos apra un módulo o aplicación administrable
        /// </summary>
        /// <returns></returns>
        public static int PermisosAdministrables(){
            return PLeer + PEscribir + PEliminar +  PAdministrar + PEjecutar; 
        }

        public int Mascara { get { return ObtenerMascara(); } }


        public void EstablacerMascara(int Mask)
        {
            NegarAcceso = (Mask & PDenegarAcceso) > 0 ? true : false;
            Leer = (Mask & PLeer) > 0 ? true : false;
            Escribir = (Mask & PEscribir) > 0 ? true : false;
            Eliminar = (Mask & PEliminar) > 0 ? true : false;
            Admin = (Mask & PAdministrar) > 0 ? true : false;
            Ejecutar = (Mask & PEjecutar) > 0 ? true : false;
        }

        public int ObtenerMascara()
        {
            int mask = 0;

            mask = NegarAcceso ? mask + PDenegarAcceso : mask;
            mask = Leer ? mask + PLeer : mask;
            mask = Escribir ? mask + PEscribir : mask;
            mask = Eliminar ? mask + PEliminar : mask;
            mask = Admin ? mask + PAdministrar : mask;
            mask = Ejecutar ? mask + PEjecutar : mask;

            return mask;
        }


    }
}
