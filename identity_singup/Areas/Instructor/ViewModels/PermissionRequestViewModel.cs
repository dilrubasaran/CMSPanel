using System.ComponentModel.DataAnnotations;

public class PermissionRequestViewModel
{
    public int EducationId { get; set; }
    public string EduName { get; set; }
    [Required(ErrorMessage = "Lütfen güncelleme talebinizin nedenini açıklayın")]
    public string Reason { get; set; }
} 