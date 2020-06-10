using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{
    public class TipoFuenteContacto: EntidadCatalogo<string, TipoFuenteContacto>
    {
        public const string OTRO = "otro";
        public const string CASA = "casa";
        public const string TRABAJO = "trabajo";
        public const string PERSONAL = "personal";
        public const string FISCAL = "fiscal";
        public const string ENTREGAS = "Entregas";
        public const string ENTIDAD = "entidad";

        public override List<TipoFuenteContacto> Seed()
        {
            List<TipoFuenteContacto> lista = new List<TipoFuenteContacto>();
            lista.Add(new TipoFuenteContacto() { Id = OTRO, Nombre = "Otro" });
            lista.Add(new TipoFuenteContacto() { Id = CASA, Nombre = "Casa" });
            lista.Add(new TipoFuenteContacto() { Id = TRABAJO, Nombre = "Trabajo" });
            lista.Add(new TipoFuenteContacto() { Id = PERSONAL, Nombre = "Personal" });
            lista.Add(new TipoFuenteContacto() { Id = FISCAL, Nombre = "Fiscal" });
            lista.Add(new TipoFuenteContacto() { Id = ENTREGAS, Nombre = "Entregas" });
            lista.Add(new TipoFuenteContacto() { Id = ENTIDAD, Nombre = "Entidad" });
            return lista;
        }

    }
}
