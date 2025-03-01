using System.ComponentModel.DataAnnotations;

namespace identity_signup.Areas.Instructor.Models
{
    public class Education
    {
        [Key] public int Id { get; set; }

        [Required] public string EduName { get; set; }

        public string Description { get; set; }

        [Required] public EduType EduType { get; set; }

        [Required] public string EduDuration { get; set; }

        [Required] public decimal EduPrice { get; set; }

        [Required] public string CreatedBy { get; set; } // Kullanıcı Adı veya ID

        [Required] public DateTime CreatedAt { get; set; }
    }
}