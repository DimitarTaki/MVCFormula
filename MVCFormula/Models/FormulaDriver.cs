using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Models
{
    public class FormulaDriver
    {
        [Required]
        public int Id { get; set; }

        [Display(Name = "Formula")]
        public int FormulaId { get; set; }

        [Display(Name = "Formula")]
        [ForeignKey("FormulaId")]
        public Formula Formula { get; set; }

        [Display(Name = "Driver")]
        public int DriverId { get; set; }

        [Display(Name = "Driver")]
        [ForeignKey("DriverId")]
        public Driver Driver { get; set; }

        public string Fuel { get; set; }

        public string Chassis { get; set; }

        public string Pts { get; set; }
    }
}
