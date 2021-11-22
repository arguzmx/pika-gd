using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public enum TipoContenido
    {
        Ninguno = 0, Documento = 1, Carpeta = 2, VinculoExpediente = 3, Secreto=4, FolderCFDI=5
    }

    public enum TipoPeriodicidad
    {
        Ninguna = 0, FechaFija= 1, Intervalo = 2
    }

    public enum UnidadesTiempo
    {
        Ninguna = 0, Segundos = 1, Minutos= 2, Horas=3, Dias=4, Meses=5, DiaSemana=6
    }

    public enum TipoEntidadAcceso
    {
        Usuario = 0, Grupo = 1
    }

    public enum TipoContenidoSecreto
    {
        Texto=0, Archivo=1
    }


    [Flags]
    public enum TipoCFDI { 
        Ninguno=0, Recibidos=1, Emitidos=2, Cancelados=4
    }

}
