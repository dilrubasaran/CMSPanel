using Microsoft.AspNetCore.Identity;

namespace identity_singup.Models
{
    public class AppUser:IdentityUser
    {
        public string? City { get; set; }
        public string? Picture { get; set; }
        public bool IsActive { get; set; } = true; // Varsayılan olarak aktif
    
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
    }
}
