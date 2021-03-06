﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspCart.Web.Models.AccountViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Lütfen e-posta adresinizi girin.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen şifrenizi girin.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} en az {2} ve en fazla {1} karakter uzunluğunda olmalıdır.", MinimumLength = 4)]
        public string Password { get; set; }

        public bool RememberMe { get; set; }
    }
}
