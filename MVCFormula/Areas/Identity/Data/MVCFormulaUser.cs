using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using MVCFormula.Models;

namespace MVCFormula.Areas.Identity.Data
{
    // Add profile data for application users by adding properties to the MVCFormulaUser class
    public class MVCFormulaUser : IdentityUser
    {
        public string Role { get; set; }

        public int? DriverId { get; set; }

        [Display(Name = "Driver")]
        [ForeignKey("DriverId")]
        public Driver Driver { get; set; }
    }
}
