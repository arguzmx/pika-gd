using RepositorioEntidades;
using System;
using System.Collections.Generic;
using System.Text;

namespace PIKA.Modelo.Contenido
{
    public class TipoGestorES: EntidadCatalogo<string, TipoGestorES>
    {
        public const string SMB= "smb";
        public const string AzureBlob = "azure-blob";

        public TipoGestorES()
        {

            Volumenes = new HashSet<Volumen>();
        }


        public override List<TipoGestorES> Seed()
        {
            List<TipoGestorES> lista = new List<TipoGestorES>();
            lista.Add(new TipoGestorES() { Id = SMB, Nombre = "Sistema archivos SMB" });
            lista.Add(new TipoGestorES() { Id = AzureBlob, Nombre = "BLOB de Azure" });
            return lista;
        }

        public virtual ICollection<Volumen> Volumenes { get; set; }

    }
}
