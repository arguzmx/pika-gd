using System;
using System.Collections.Generic;
using System.Text;

namespace RepositorioEntidades
{
    public class RespuestaComandoWeb
    {
        public const string ErrorProceso = "comandoweb-500";
        public const string NoEncontrado = "comandoweb-404";
        public const string Novalido = "comandoweb-400";

        public object Payload { get; set; }
        public bool Estatus { get; set; }
        public string MensajeId { get; set; }
    }
}
