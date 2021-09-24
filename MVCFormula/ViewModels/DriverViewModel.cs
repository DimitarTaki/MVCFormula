using Microsoft.AspNetCore.Http;
using MVCFormula.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MVCFormula.ViewModels
{
    public class DriverViewModel
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(30)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        public int Podiums { get; set; }

        public string Country { get; set; }

        [Display(Name = "Driver Picture")]
        public IFormFile? DriverPicture { get; set; }

        [Display(Name = "Full Name")]
        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        public string Team { get; set; }

        public double Points { get; set; }


        public ICollection<FormulaDriver> Formulas { get; set; }
    }
}
