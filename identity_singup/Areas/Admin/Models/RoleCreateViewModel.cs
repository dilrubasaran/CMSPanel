using System.ComponentModel.DataAnnotations;

namespace identity_signup.Areas.Admin.Models
{
    public class RoleCreateViewModel
    {
        [Required(ErrorMessage = "Rol adı gereklidir")]
        [Display(Name = "Rol Adı:")]
        public string Name { get; set; } = null!;
        public int PermissionLevel { get; set; }
    }
} 