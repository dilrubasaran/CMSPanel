using System.ComponentModel.DataAnnotations;

namespace identity_signup.Areas.Admin.Models
{
    public class RoleUpdateViewModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "Rol adı gereklidir")]
        [Display(Name = "Rol Adı:")]
        public string Name { get; set; }
    }
} 