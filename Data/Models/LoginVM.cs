using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PhoneReclaim.Data.Models
{
    public class LoginVM
    {
        [Required(ErrorMessage ="Username is required.")]
        public string? UserName {get; set; }
        [DataType(DataType.Password)]
        [Required(ErrorMessage ="Password is required.")]
        public string? Password {get; set; }
        [Display(Name ="Remember Me. ")]
        public bool RememberMe {get; set; }
    }
}