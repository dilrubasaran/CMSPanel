using System.ComponentModel.DataAnnotations;
using identity_signup.Areas.Instructor.Models;

namespace identity_signup.Areas.Instructor.ViewModels
{
    public class EduCreateViewModel
    {
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Eğitim adı zorunludur.")]
        [Display(Name = "Eğitim Adı")]
        public string EduName { get; set; }

        [Display(Name = "Açıklama")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Eğitim tipi zorunludur.")]
        [Display(Name = "Eğitim Tipi")]
        public EduType EduType { get; set; }

        [Required(ErrorMessage = "Eğitim süresi zorunludur.")]
        [Display(Name = "Eğitim Süresi")]
        public string EduDuration { get; set; }

        [Required(ErrorMessage = "Fiyat zorunludur.")]
        [Display(Name = "Fiyat")]
        public decimal EduPrice { get; set; }

        public string? CreatedBy { get; set; }
    }
}
