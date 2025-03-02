using System.ComponentModel.DataAnnotations;

namespace identity_singup.ViewModels
{
    public class SignInViewModel
    {
        [Required(ErrorMessage = "Kullanıcı adı alanı boş bırakılamaz.")]
        [Display(Name = "Kullanıcı Adı")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Şifre alanı boş bırakılamaz.")]
        [Display(Name = "Şifre")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Beni Hatırla")]
        public bool RememberMe { get; set; }
    }
}
