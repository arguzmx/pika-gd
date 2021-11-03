using RepositorioEntidades;
using System;
using System.Collections.Generic;
using PIKA.Modelo.Metadatos;
using PIKA.Modelo.Metadatos.Atributos;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

namespace PIKA.Modelo.Contenido
{

    /// <summary>
    /// Administra los tipos de getsores paa el sistema de alamcenamiento y E/S de los repositorios
    /// </summary>
    [Entidad(PaginadoRelacional: false, EliminarLogico: false)]
    public class TipoGestorES: EntidadCatalogo<string, TipoGestorES>
    {
        public const string LOCAL_FOLDER = "folder";
        public const string SMB= "smb";
        public const string AzureBlob = "azure-blob";
        public const string LaserFiche = "laserfiche";

        public TipoGestorES()
        {

            Volumenes = new HashSet<Volumen>();
        }

        [Prop(Required: false, isId: true, Visible: false, OrderIndex: 0)]
        [VistaUI(ControlUI: ControlUI.HTML_HIDDEN, Accion: Acciones.update)]
        public override string Id { get => base.Id; set => base.Id = value; }


        [Prop(Required: true, OrderIndex: 5)]
        [VistaUI(ControlUI: ControlUI.HTML_TEXT, Accion: Acciones.addupdate)]
        [ValidString(minlen: 2, maxlen: 200)]
        public override string Nombre { get => base.Nombre; set => base.Nombre = value; }

        public override List<TipoGestorES> Seed()
        {
            List<TipoGestorES> lista = new List<TipoGestorES>();
            lista.Add(new TipoGestorES() { Id = LOCAL_FOLDER, Nombre = "Carpeta local" });
            lista.Add(new TipoGestorES() { Id = SMB, Nombre = "Sistema archivos SMB" });
            lista.Add(new TipoGestorES() { Id = LaserFiche, Nombre = "Volumen Laserfiche" });
            //lista.Add(new TipoGestorES() { Id = AzureBlob, Nombre = "BLOB de Azure" });
            return lista;
        }

        [JsonIgnore]
        [XmlIgnore]
        public virtual ICollection<Volumen> Volumenes { get; set; }
    }
}
