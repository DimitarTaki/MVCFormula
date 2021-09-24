using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<MVCFormulaUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            roleCheck = await RoleManager.RoleExistsAsync("Driver");
            if (!roleCheck)
            {
                roleResult = await RoleManager.CreateAsync(new IdentityRole("Driver"));
            }
            MVCFormulaUser user = await UserManager.FindByEmailAsync("admin@mvcformula.com");
            if (user == null)
            {
                var User = new MVCFormulaUser();
                User.Email = "admin@mvcformula.com";
                User.UserName = "admin@mvcformula.com";
                string userPWD = "Admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }
        }

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MVCFormulaContext(
            serviceProvider.GetRequiredService<
            DbContextOptions<MVCFormulaContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();
                // Look for any movies.
                if (context.Formula.Any() || context.Driver.Any() || context.Manufacturer.Any())
                {
                    return;   // DB has been seeded
                }

                context.Manufacturer.AddRange(
                    new Manufacturer
                    { /*Id = 1, */
                        Name = "Alfa Romeo",
                        Country = "Italy",
                        Headquarters = "Turin",
                        Founded = DateTime.Parse("1910-6-24"),

                    },
                    new Manufacturer
                    { /*Id = 2, */
                        Name = "Honda",
                        Country = "Japan",
                        Headquarters = "Minato",
                        Founded = DateTime.Parse("1948-09-24"),

                    },
                    new Manufacturer
                    { /*Id = 3, */
                        Name = "Mercedes-Benz",
                        Country = "Germany",
                        Headquarters = "Stuttgart",
                        Founded = DateTime.Parse("1926-06-28"),

                    },
                    new Manufacturer
                    { /*Id = 4, */
                        Name = "Ferrari",
                        Country = "Italy",
                        Headquarters = "Maranello",
                        Founded = DateTime.Parse("1939-09-13"),

                    },
                    new Manufacturer
                    { /*Id = 5, */
                        Name = "McLarren",
                        Country = "England",
                        Headquarters = "McLarren Tehnology Centre",
                        Founded = DateTime.Parse("1985-12-02"),

                    }



                );
                context.SaveChanges();

                context.Formula.AddRange(
                    new Formula
                    {
                        /*Id = 1, */
                        Model = "SF21",
                        ModelYear = 2021,
                        Tyres = "Pirelli",
                        ManufacturerId = context.Manufacturer.Single(m => m.Name == "Ferrari").Id,

                    },
                    new Formula
                    {
                        /*Id = 2, */
                        Model = "W 12",
                        ModelYear = 2021,
                        Tyres = "Pirelli",
                        ManufacturerId = context.Manufacturer.Single(m => m.Name == "Mercedes-Benz").Id,

                    },
                    new Formula
                    {
                        /*Id = 3, */
                        Model = "MCL35M",
                        ModelYear = 2021,
                        Tyres = "Pirelli",
                        ManufacturerId = context.Manufacturer.Single(m => m.Name == "McLarren").Id,

                    },
                    new Formula
                    {
                        /*Id = 4, */
                        Model = "RA621H",
                        ModelYear = 2021,
                        Tyres = "Pirelli",
                        ManufacturerId = context.Manufacturer.Single(m => m.Name == "Honda").Id,

                    },
                    new Formula
                    {
                        /*Id = 5, */
                        Model = "Racing Orlen",
                        ModelYear = 2021,
                        Tyres = "Pirelli",
                        ManufacturerId = context.Manufacturer.Single(m => m.Name == "Alfa Romeo").Id,

                    }

                );
                context.SaveChanges();

                context.Driver.AddRange(
                    new Driver { /*Id = 1, */FirstName = "Carlos", LastName = "Sainz", BirthDate = DateTime.Parse("1994-01-09"), Podiums = 4,Country="Spain", Team = "Scuderia Ferrari Mission Winnow", Points = 201.5 },
                    new Driver
                    { /*Id = 2, */
                        FirstName = "Charles",
                        LastName = "Leclerc",
                        BirthDate = DateTime.Parse("1997-10-16"),
                        Podiums = 13,
                        Country="Monaco",
                        Team = "Scuderia Ferrari Mission Winnow",
                        Points = 201.5,
                    },
                    new Driver
                    { /*Id = 3, */
                        FirstName = "Lewis",
                        LastName = "Hamilton",
                        BirthDate = DateTime.Parse("1985-01-07"),
                        Podiums = 175,
                        Country = "United Kingdom",
                        Team = "Mercedes-AMG Petronas F1 Team",
                        Points = 362.5,

                    },
                    new Driver
                    { /*Id = 4, */
                        FirstName = "Lando",
                        LastName = "Norris",
                        BirthDate = DateTime.Parse("1999-11-13"),
                        Podiums = 5,
                        Country = "United Kingdom",
                        Team = "McLaren F1 Team",
                        Points = 215,

                    },
                     new Driver
                     { /*Id = 5, */
                         FirstName = "Antonio",
                         LastName = "Gionivazzi",
                         BirthDate = DateTime.Parse("1993-12-14"),
                         Podiums = 0,
                         Country = "Italy",
                         Team = "Alfa Romeo Racing ORLEN",
                         Points = 3

                     }
                );
                context.SaveChanges();

                context.FormulaDriver.AddRange(
                    new FormulaDriver
                    {
                        FormulaId = 1,
                        DriverId = 1,
                        Fuel = "Shell V-Power",
                        Chassis = "SF21",
                        Pts = "469.5"
                    },
                    new FormulaDriver
                    {
                        FormulaId = 1,
                        DriverId = 2,
                        Fuel = "Shell V-Power",
                        Chassis = "SF21",
                        Pts = "505"
                    },
                    new FormulaDriver
                    {
                        FormulaId = 2,
                        DriverId = 3,
                        Fuel = "Shell V-Power",
                        Chassis = "W12",
                        Pts = "3999.5"
                    },
                      new FormulaDriver
                      {
                          FormulaId = 3,
                          DriverId = 4,
                          Fuel = "Shell V-Power",
                          Chassis = "MCL35M",
                          Pts = "278"
                      },
                      new FormulaDriver
                      {
                          FormulaId = 5,
                          DriverId = 5,
                          Fuel = "Shell V-Power",
                          Chassis = "C41",
                          Pts = "19"
                      }


                );
                context.SaveChanges();

               
                    
                    
                    
                    
            }
        }

    }
}







    

