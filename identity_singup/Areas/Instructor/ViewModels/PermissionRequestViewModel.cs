using System.ComponentModel.DataAnnotations;

public class PermissionRequestViewModel
{
    public int EducationId { get; set; }

    [Display(Name = "Eğitim Adı")]
    public string EduName { get; set; }

    [Required(ErrorMessage = "Lütfen güncelleme talebinizin nedenini açıklayın")]
    [Display(Name = "Talep Sebebi")]
    public string Reason { get; set; }
} 