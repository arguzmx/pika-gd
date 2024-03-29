﻿using PIKA.Modelo.Organizacion;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using PIKA.Infraestructura.Comun;
using Bogus;
using PIKA.Servicio.Organizacion.Data;
using Bogus.DataSets;
using Microsoft.EntityFrameworkCore;

namespace PIKA.Servicio.Organizacion
{
    public class InicializarDatos
    {
        public static void Inicializar(DbContextOrganizacion dbContext, string contentPath, bool datosDemo) {
            DatosIniciales(dbContext);
            if (datosDemo)
            {
                InicializarDatosDemo(dbContext);
            }
        }

        public static void DatosIniciales(DbContextOrganizacion dbContext)
        {
            Dominio d = new Dominio()
            {
                Eliminada = false,
                Nombre = $"Mi Organización",
                Id = $"principal",
                OrigenId = ConstantesModelo.IDORIGEN_GLOBAL,
                TipoOrigenId = ConstantesModelo.IDORIGEN_GLOBAL
            };
            if (dbContext.Dominios.Find(d.Id) == null)
            {
                dbContext.Dominios.Add(d);
                dbContext.SaveChanges();
            }
                

            UnidadOrganizacional uo = new UnidadOrganizacional()
            {
                DominioId = d.Id,
                Eliminada = false,
                Id = "principal",
                Nombre = "Mi oficina"
            };

            if (dbContext.UnidadesOrganizacionales.Find(uo.Id) == null)
            {
                dbContext.UnidadesOrganizacionales.Add(uo);
                dbContext.SaveChanges();
            }

            Rol r = new Rol()
            {
                Nombre = $"Usuarios",
                Id = $"usuarios",
                OrigenId = d.Id,
                TipoOrigenId = ConstantesModelo.IDORIGEN_DOMINIO,
                Descripcion = "Usuarios del sistema"
            };
            
            if (dbContext.Roles.Find(r.Id) == null)
            {
                dbContext.Roles.Add(r);
                dbContext.SaveChanges();
            }


            r = new Rol()
            {
                Nombre = $"Administradores",
                Id = $"administradores",
                OrigenId = d.Id,
                TipoOrigenId = ConstantesModelo.IDORIGEN_DOMINIO,
                Descripcion = "Administradores del sistema"
            };

            if (dbContext.Roles.Find(r.Id) == null)
            {
                dbContext.Roles.Add(r);
                dbContext.SaveChanges();
            }

        }

        public static void InicializarDatosDemo(DbContextOrganizacion dbContext)
        {
            //dbContext.Database.ExecuteSqlRaw($"delete from {DbContextOrganizacion.TablaRoles}");
            //dbContext.Database.ExecuteSqlRaw($"delete from {DbContextOrganizacion.TablaDireccionesPortales}");
            //dbContext.Database.ExecuteSqlRaw($"delete from {DbContextOrganizacion.TablaOU}");
            //dbContext.Database.ExecuteSqlRaw($"delete from {DbContextOrganizacion.TablaDominio}");

            List<Dominio> dominios = new List<Dominio>();
            List<UnidadOrganizacional> uos = new List<UnidadOrganizacional>();
            List<Rol> roles = new List<Rol>();
            //AÑADE DOMINIOS
            for (int i = 0; i < 15; i++)
            {
                Dominio d = new Dominio()
                {
                    Eliminada = false,
                    Nombre = $"dominio {i}",
                    Id = $"dominio{i}",
                    OrigenId = ConstantesModelo.IDORIGEN_GLOBAL,
                    TipoOrigenId = ConstantesModelo.IDORIGEN_GLOBAL
                };
                dominios.Add(d);
                if (dbContext.Dominios.Find(d.Id) == null)
                {
                    
                    dbContext.Dominios.Add(d);
                }
            }
            dbContext.SaveChanges();


            //AÑDE Uniodades organziacionales para los primeros 2 dominios
            for (int k = 0; k < 2; k++) { 
                for(int i = 0; i <= 15; i++)
                {
                    UnidadOrganizacional u = new UnidadOrganizacional()
                    {
                        DominioId = dominios[k].Id,
                        Eliminada = false,
                        Id = $"uo{i}{dominios[k].Id}",
                        Nombre = $"OU {i} - {dominios[k].Id}"
                    };
                    uos.Add(u);
                    if (dbContext.UnidadesOrganizacionales.Find(u.Id) == null)
                    {
                        dbContext.UnidadesOrganizacionales.Add(u);
                    }
                }
            }
            dbContext.SaveChanges();

            //Randomizer.Seed = new Random(DateTime.Now.Second);

            //var nombres_direcciones = new[] { "casa", "trabajo", "mensajes" };
            //var ids_estados = new[] { "CMX", "JAL", "MEX" };

            ////AÑDE direcciiones poastales a las 10 primeras
            //Console.WriteLine($"Añadiendo direcciones postales");
            //for (int k = 0; k < 10; k++)
            //{
            //    DireccionPostal d = new Faker<DireccionPostal>()
            //        .RuleFor(x => x.Id, f => $"dp{uos[k].Id}")
            //        .RuleFor(x => x.Calle, f => f.Address.StreetName())
            //        .RuleFor(x => x.Colonia, f => f.Address.City())
            //        .RuleFor(x => x.CP, f => f.Address.ZipCode())
            //        .RuleFor(x => x.EstadoId, f => f.Address.StreetName())
            //        .RuleFor(x => x.Municipio, f => f.Address.State())
            //        .RuleFor(x => x.NoExterno, f => f.Address.BuildingNumber())
            //        .RuleFor(x => x.NoInterno, f => f.Address.BuildingNumber())
            //        .RuleFor(x => x.Nombre, f => f.PickRandom(nombres_direcciones))
            //        .RuleFor(x => x.OrigenId, f => uos[k].Id)
            //        .RuleFor(x => x.TipoOrigenId, f => ConstantesModelo.IDORIGEN_UNIDAD_ORGANIZACIONAL)
            //        .RuleFor(x => x.PaisId, f => "MEX")
            //        .RuleFor(x => x.EstadoId, f => f.PickRandom(ids_estados));

            //    if (dbContext.DireccionesPostales.Find(d.Id) == null)
            //    {
            //        dbContext.DireccionesPostales.Add(d);
            //    }
            //}
            //dbContext.SaveChanges();


            //Añade roles
            for (int i = 0; i < 15; i++)
            {
                Rol d = new Rol()
                {
                    Nombre = $"Rol {i}",
                    Id = $"rol{i}",
                    OrigenId = ConstantesModelo.IDORIGEN_DOMINIO,
                    TipoOrigenId = ConstantesModelo.IDORIGEN_DOMINIO, 
                    Descripcion = "Descripción del rol"
                };
                roles.Add(d);
                if (dbContext.Roles.Find(d.Id) == null)
                {
                    dbContext.Roles.Add(d);
                }
            }
            dbContext.SaveChanges();


        }

    }
}
