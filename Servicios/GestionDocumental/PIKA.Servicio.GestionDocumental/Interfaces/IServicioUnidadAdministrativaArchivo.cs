using PIKA.Infraestructura.Comun;
using PIKA.Modelo.GestorDocumental;
using PIKA.Modelo.GestorDocumental.Seguridad;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Servicio.GestionDocumental
{
    public interface IServicioUnidadAdministrativaArchivo: IServicioRepositorioAsync<UnidadAdministrativaArchivo, string>,
        IServicioValorTextoAsync<UnidadAdministrativaArchivo>, IServicioAutenticado<PermisoUnidadAdministrativaArchivo>
    {

    }
}
