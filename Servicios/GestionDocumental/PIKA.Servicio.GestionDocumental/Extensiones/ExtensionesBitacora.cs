using PIKA.Modelo.GestorDocumental;
namespace PIKA.Servicio.GestionDocumental
{
    public static class ExtensionesBitacora
    {
        public static Archivo Bitacora(this Archivo a)
        {
            Archivo archivo = new Archivo()
            {
                Id = a.Id,
                Nombre = a.Nombre,
                OrigenId = a.OrigenId,
                TipoOrigenId = a.TipoOrigenId,
                TipoArchivoId = a.TipoArchivoId,
                Eliminada = a.Eliminada,
                VolumenDefaultId = a.VolumenDefaultId,
                PuntoMontajeId = a.PuntoMontajeId,
                Almacenes = null,
                Activos = null,
                HistorialArchivosActivo = null,
                Prestamos = null,
                TransferenciasOrigen = null,
                TransferenciasDestino = null,
                ZonasAlmacen = null,
                PosicionesAlmacen = null,
                Contenedores = null,
                Reportes = null,
            };

            return archivo;
        }

    }
}
