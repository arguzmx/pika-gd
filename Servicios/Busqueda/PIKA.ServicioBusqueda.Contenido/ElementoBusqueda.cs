using PIKA.Constantes.Aplicaciones.Contenido;
using PIKA.Infraestructura.Comun;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace PIKA.ServicioBusqueda.Contenido
{

    public class ElementoBusqueda : Entidad<string>, IEntidadRegistroCreacion,
        IEntidadEliminada, IEntidadNombrada
    {


        public override string Id { get => base.Id; set => base.Id = value; }
        public string Nombre { get; set; }
        public bool Eliminada { get; set; }
        public string PuntoMontajeId { get; set; }
        public string CreadorId { get; set; }
        public DateTime FechaCreacion { get; set; }
        public string VolumenId { get; set; }
        public string CarpetaId { get; set; }
        public string PermisoId { get; set; }
        public bool Versionado { get; set; }
        public string VersionId { get; set; }
    }

}
