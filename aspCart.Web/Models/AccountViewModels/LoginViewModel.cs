using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspCart.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Lütfen e-posta adresinizi girin")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta girin")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifrenizi girin")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
