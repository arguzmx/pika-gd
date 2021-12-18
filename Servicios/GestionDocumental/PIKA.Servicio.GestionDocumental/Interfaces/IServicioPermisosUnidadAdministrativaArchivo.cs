using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;


namespace PIKA.Servicio.GestionDocumental.Interfaces
{
    public interface IServicioPermisosUnidadAdministrativaArchivo: IServicioRepositorioAsync<PermisosUnidadAdministrativaArchivo, string>
    {
    }
}
