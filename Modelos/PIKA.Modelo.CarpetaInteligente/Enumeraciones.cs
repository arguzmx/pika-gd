using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.CarpetaInteligente
{
    public enum TipoContenido
    {
        Ninguno = 0, Documento = 1, Carpeta = 2, VinculoExpediente = 3
    }

    public enum TipoCaducidad
    {
        Ninguna = 0, FechaFija= 1, Intervalo = 2
    }


    public enum UnidadesCaducidad
    {
        Ninguna = 0, Segundos = 1, Minutos= 2, Horas=3, Dias=4, Meses=5
    }
}
