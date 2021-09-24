using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Models
{
    public class Formula
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(40)]

        public string Model { get; set; }

        [Range(1900, 2050)]
        [Display(Name = "Model Year")]
        public int? ModelYear { get; set; }

        [StringLength(40)]
        [Display(Name = "Tyres")]
        public string? Tyres { get; set; }

        [Display(Name = "Picture")]
        public string? FormulaPicture { get; set; }

        [Display(Name = "Manufacturer")]
        public int ManufacturerId { get; set; }

        [Display(Name = "Manufacturer")]
        [ForeignKey("ManufacturerId")]
        public Manufacturer Manufacturer { get; set; }

        public ICollection<FormulaDriver> Drivers { get; set; }
    }
}
