using PIKA.Modelo.Reportes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PIKA.Servicio.Reportes.Data
{
   public class InicializarDatos
    {

        private static ReporteEntidad GuiaSimpleArchivo()
        {
            return new ReporteEntidad()
            {
                Descripcion = "Reporte guía simple de archivo",
                Entidad = "Archivo",
                Id = "guiasimplearchivo",
                Nombre = "Guía simple de archivo",
                OrigenId = "*",
                TipoOrigenId = "*",
                Plantilla = ""
            };
        }

        private static ReporteEntidad CaratulaActivoAcervo()
        {
            return new ReporteEntidad()
            {
                Descripcion = "Reporte caratula acervo",
                Entidad = "Activo",
                Id = "caratulaactivo",
                Nombre = "Reporte caratula acervo",
                OrigenId = "*",
                TipoOrigenId = "*",
                Plantilla = ""
            };
        }

        private static void InsertaRreporte(DbContextReportes dbContext, ReporteEntidad r, FileInfo fi)
        {
            if (!dbContext.ReporteEntidades.Where(x => x.Id == r.Id && x.OrigenId == r.OrigenId
            && x.TipoOrigenId == r.TipoOrigenId).Any())
            {
                r.Plantilla = Convert.ToBase64String(File.ReadAllBytes(fi.FullName));
                dbContext.ReporteEntidades.Add(r);
                dbContext.SaveChanges();
            }

        }


        public static void Inicializar(DbContextReportes dbContext, string contentPath)
        {

            string Ruta = Path.Combine(contentPath, "Contenido", "Reportes");
            if (Directory.Exists(Ruta))
            {
                foreach (var item in Directory.GetFiles(Ruta, "*.docx"))
                {
                    FileInfo fi = new FileInfo(item);
                    switch (fi.Name.Replace(fi.Extension, ""))
                    {
                        case "guiasimplearchivo":
                            InsertaRreporte(dbContext, GuiaSimpleArchivo(), fi);
                            break;

                        case "caratulaactivo":
                            InsertaRreporte(dbContext, CaratulaActivoAcervo(), fi);
                            break;
                    }
                }

                
            }
            
        }

       
    }
}
