using identity_singup.Models;
using System.ComponentModel.DataAnnotations;

namespace identity_signup.ViewModels
{
    public class UserEditViewModel
    {
      
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "E-Posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [Display(Name = "E-Posta Adresi")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Telefon Numarası Boş Bırakılamaz!")]
        [RegularExpression(@"^(\+90|0)?\d{10}$", ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [Display(Name = "Telefon Numarası")]
        public string Phone { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Doğum Günü")]
        public DateTime? BirthDate { get; set; }


        [Display(Name = "Şehir")]
        public string City { get; set; }


        //[Display(Name = "Profil resmi")]
        //public IFormFile Picture { get; set; }


        [Display(Name = "Cinsiyet")]
        public Gender? Gender { get; set; }

    }
}
