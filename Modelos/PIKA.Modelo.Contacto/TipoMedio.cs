using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contacto
{
    [Entidad(EliminarLogico:false)]
    public class TipoMedio: EntidadCatalogo<string, TipoMedio>
    {
        public const string Telefono = "tel";
        public const string Email = "email";
        public const string Facebook = "fbook";
        public const string Twitter = "twitter";

        public override string Id { get; set; }

        public override string Nombre { get; set; }

        public override List<TipoMedio> Seed()
        {
            List<TipoMedio> l = new List<TipoMedio>();

            l.Add(new TipoMedio() { Id = Telefono, Nombre = "Teléfono" });
            l.Add(new TipoMedio() { Id = Email, Nombre = "Correo electrónico" });
            l.Add(new TipoMedio() { Id = Facebook, Nombre = "Facebook" });
            l.Add(new TipoMedio() { Id = Twitter, Nombre = "Twitter" });

            return l;
        }


        public virtual ICollection<MedioContacto> MediosContacto { get; set; }
    }
}
