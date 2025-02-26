using System.ComponentModel.DataAnnotations;

namespace identity_singup.ViewModels 
{
    public class SignUpViewModel
    {
       
        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; }

        [DataType(DataType.EmailAddress)]
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [Display(Name = "Email :")] 
        public string Email { get; set; }

        [DataType(DataType.PhoneNumber)]
        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
        [RegularExpression(@"^(05\d{9})$", ErrorMessage = "Geçerli bir telefon numarası giriniz. Örnek: 05XXXXXXXXX")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "şifre alanı boş bırakılamaz.")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifre aynı değildir.")]
        [Required(ErrorMessage = "şifre tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Şifre Tekrarı:")]
        public string PasswordConfirm { get; set; }

    }
}
