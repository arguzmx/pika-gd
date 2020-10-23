using PIKA.Modelo.GestorDocumental;
using PIKA.Servicio.GestionDocumental.Servicios.Reporte;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PIKA.Servicio.GestionDocumental.Servicios
{
    public partial class ServicioActivo
    {
        public async Task<byte[]> ImportarActivos(byte[] file, string ArchivId, string TipoId, string OrigenId, string formatoFecha)
        {
            IoImportarActivos importador = new IoImportarActivos(this, this.logger,
                proveedorOpciones, Config);
            byte[] archivo = await importador.ImportandoDatos(file, ArchivId, TipoId, OrigenId, formatoFecha);
            return archivo;
        }


        public async Task<Activo> ValidadorImportador(Activo a)
        {

            await Task.Delay(1);
       //     if (!await ExisteElemento(x => x.Id.Equals(a.EntradaClasificacionId.Trim(), StringComparison.InvariantCultureIgnoreCase)
       //|| x.Eliminada != true)
       //|| !await ExisteArchivo(x => x.Id.Equals(a.ArchivoOrigenId.Trim(), StringComparison.InvariantCultureIgnoreCase)
       //           && x.Eliminada != true)
       //|| await Existe(x => x.IDunico.Equals(a.IDunico, StringComparison.InvariantCultureIgnoreCase)))
       //     {
       //         a = new Activo();
       //     }
       //     else
       //     {
       //         await CrearAsync(a);
       //     }

            return a;
        }


    }

}
