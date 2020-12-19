using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace aspCart.Web.Models
{
    public class CreateReviewModel
    {
        [Required]
        public Guid ProductId { get; set; }

        [Required]
        public string ProductName { get; set; }

        [Required]
        public string ProductSeo { get; set; }

        [Required(ErrorMessage = "Lütfen başlık girin.")]
        [MinLength(10, ErrorMessage = "Lütfen daha uzun başlık girmeyi deneyin.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Lütfen değerlendirme girin.")]
        [MinLength(20, ErrorMessage = "Lütfen daha uzun değerlendirme girmeyi deneyin.")]
        public string Message { get; set; }

        [Required]
        public int Rating { get; set; }
    }
}
