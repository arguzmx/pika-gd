using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Contenido;
using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioVisor: IServicioAutenticado<Elemento>
    {

        Task<Documento> ObtieneDocumento(string IdElemento);

    }
}
