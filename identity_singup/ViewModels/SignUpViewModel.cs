using System.ComponentModel.DataAnnotations;

namespace identity_singup.ViewModels 
{
    public class SignUpViewModel
    {
       
        [Required(ErrorMessage = "Kullanıcı Ad alanı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı :")]
        public string UserName { get; set; }

        
        [Required(ErrorMessage = "Email alanı boş bırakılamaz.")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz.")]
        [Display(Name = "Email :")] 
        public string Email { get; set; }

       
        [Required(ErrorMessage = "Telefon alanı boş bırakılamaz.")]
        [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz.")]
        [RegularExpression(@"^(05\d{9})$", ErrorMessage = "Geçerli bir telefon numarası giriniz. Örn: 05XX XXX XX XX")]
        [Display(Name = "Telefon :")]
        public string Phone { get; set;}

        [DataType(DataType.Password)]
        [Required(ErrorMessage = "şifre alanı boş bırakılamaz.")]
        [MinLength(6, ErrorMessage = "Şifreniz en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&]).{6,}$",
            ErrorMessage = "Şifreniz en az bir büyük harf, bir küçük harf, bir rakam ve bir özel karakter içermelidir.")]
        [Display(Name = "Şifre :")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Şifre aynı değildir.")]
        [Required(ErrorMessage = "şifre tekrar alanı boş bırakılamaz.")]
        [Display(Name = "Şifre Tekrarı:")]
        public string PasswordConfirm { get; set; }

    }
}
