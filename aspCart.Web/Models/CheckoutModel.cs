using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspCart.Web.Models
{
    public class CheckoutModel
    {
        [Required(ErrorMessage = "Lütfen adınızı girin.")]
        [StringLength(30, ErrorMessage = "Lütfen geçerli bir ad girin.", MinimumLength = 2)]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Lütfen soyadınızı girin.")]
        [StringLength(30, ErrorMessage = "Lütfen geçerli bir soyad girin.", MinimumLength = 2)]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Lütfen e-posta adresinizi girin.")]
        [EmailAddress(ErrorMessage = "Lütfen geçerli bir e-posta girin.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Lütfen adres girin.")]
        [StringLength(250, ErrorMessage = "Lütfen geçerli bir adres girin.", MinimumLength = 10)]
        public string Address { get; set; }

        [Required(ErrorMessage = "Lütfen şehir seçin.")]
        public string City { get; set; }

        [Required(ErrorMessage = "Lütfen ilçe girin.")]
        [StringLength(30, ErrorMessage = "Lütfen geçerli bir ilçe girin.", MinimumLength = 2)]
        public string StateProvince { get; set; }

        //[Required]
        //[Display(Name = "Zip/Postal Code")]
        public string ZipPostalCode { get; set; }

        //[Required]
        public string Country { get; set; }

        [Required]
        public string Telephone { get; set; }

        public List<CartItemModel> CartItemModel { get; set; }
    }
}
