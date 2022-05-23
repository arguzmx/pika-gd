using PIKA.Modelo.GestorDocumental.eventos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;


namespace PIKA.Modelo.GestorDocumental
{
    public class EventoContenedorAlmacen: Entidad<long>
    {
        public override long Id { get => base.Id    ; set => base.Id = value; }
        public string UsuarioId { get; set; }
        public string ProcesoId { get; set; }
        public bool EsAccionUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public TipEventoContenedorAlmacen TipoEvento { get; set; }
        public string ContenedorAlmacenId { get; set; }
        public string Payload { get; set; }

        public ContenedorAlmacen ContenedorAlmacen { get; set; }

    }
}
