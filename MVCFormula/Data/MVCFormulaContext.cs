using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MVCFormula.Areas.Identity.Data;
using MVCFormula.Models;

namespace MVCFormula.Data
{
    public class MVCFormulaContext : IdentityDbContext<MVCFormulaUser>
    {
        public MVCFormulaContext (DbContextOptions<MVCFormulaContext> options)
            : base(options)
        {
        }

        public DbSet<MVCFormula.Models.Driver> Driver { get; set; }

        public DbSet<MVCFormula.Models.Formula> Formula { get; set; }

        public DbSet<MVCFormula.Models.FormulaDriver> FormulaDriver { get; set; }

        public DbSet<MVCFormula.Models.Manufacturer> Manufacturer { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FormulaDriver>()
                .HasOne<Driver>(p => p.Driver)
                .WithMany(p => p.Formulas)
                .HasForeignKey(p => p.DriverId);

            builder.Entity<FormulaDriver>()
                .HasOne<Formula>(p => p.Formula)
                .WithMany(p => p.Drivers)
                .HasForeignKey(p => p.FormulaId);

            builder.Entity<Formula>()
                .HasOne<Manufacturer>(p => p.Manufacturer)
                .WithMany(p => p.Formulas)
                .HasForeignKey(p => p.ManufacturerId);

            base.OnModelCreating(builder);
        }
        }
}
