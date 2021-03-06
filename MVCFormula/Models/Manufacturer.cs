using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.Models
{
    public class Manufacturer
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Manufacturer")]
        public string Name { get; set; }

        [StringLength(40)]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [StringLength(100)]
        [Display(Name = "Headquarters")]
        public string? Headquarters { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Founded")]
        public DateTime? Founded { get; set; }

        [Display(Name = "Manufacturer Picture")]
        public string? ManufacturerPicture { get; set; }

        public ICollection<Formula> Formulas { get; set; }
    }
}
