using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Seguridad
{
    public class Genero: EntidadCatalogo<string, Genero>
    {

        public Genero() { }

        public const string MASCULINO = "m";
        public const string FEMENINO = "f";


        public override List<Genero> Seed()
        {
            List<Genero> l = new List<Genero>();

            l.Add(new Genero() { Id = FEMENINO, Nombre = "Femenino" });
            l.Add(new Genero() { Id = MASCULINO, Nombre = "Masculino" });

            return l;

        }

        public ICollection<PropiedadesUsuario> PropiedadesUsuario { get; set; }
    }
}
