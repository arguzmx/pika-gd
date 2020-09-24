using PIKA.Modelo.Contenido.ui;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.Contenido.Interfaces
{
    public interface IServicioVisor
    {

        Task<Documento> ObtieneDocumento(string IdElemento);

    }
}
