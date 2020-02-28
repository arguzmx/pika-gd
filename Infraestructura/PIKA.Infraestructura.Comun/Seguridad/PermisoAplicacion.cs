using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Infraestructura.Comun.Seguridad
{


    public class PermisoAplicacion
    {
        private const int GDenyAccess = 1;
        private const int GRead = 2;
        private const int GWrite = 4;
        private const int GDelete = 8;
        private const int GAdmin = 16;
        private const int GExecute = 32;


        public PermisoAplicacion()
        {
            Leer = false;
            Escribir = false;
            Eliminar = false;
            Admin = false;
            Ejecutar = false;
            NegarAcceso = false;
        }

        public bool NegarAcceso { get; set; }
        public bool Leer { get; set; }
        public bool Escribir { get; set; }
        public bool Eliminar { get; set; }
        public bool Admin { get; set; }
        public bool Ejecutar { get; set; }

        /// <summary>
        /// Devuelve la lsita de permisos apra un módulo o aplicación administrable
        /// </summary>
        /// <returns></returns>
        public static ulong PermisosAdministrables(){
            return GRead + GWrite + GDelete +  GAdmin + GExecute; 
        }

        public ulong Mascara { get { return ObtenerMascara(); } set { EstablacerMascara(value); } }


        private void EstablacerMascara(ulong Mask)
        {
            NegarAcceso = (Mask & GDenyAccess) > 0 ? true : false;
            Leer = (Mask & GRead) > 0 ? true : false;
            Escribir = (Mask & GWrite) > 0 ? true : false;
            Eliminar = (Mask & GDelete) > 0 ? true : false;
            Admin = (Mask & GAdmin) > 0 ? true : false;
            Ejecutar = (Mask & GExecute) > 0 ? true : false;
        }

        private ulong ObtenerMascara()
        {
            ulong mask = 0;

            mask = NegarAcceso ? mask + GDenyAccess : mask;
            mask = Leer ? mask + GRead : mask;
            mask = Escribir ? mask + GWrite : mask;
            mask = Eliminar ? mask + GDelete : mask;
            mask = Admin ? mask + GAdmin : mask;
            mask = Ejecutar ? mask + GExecute : mask;

            return mask;
        }


    }
}
