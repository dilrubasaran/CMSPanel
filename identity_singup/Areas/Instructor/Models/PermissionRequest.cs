
namespace identity_signup.Areas.Instructor.Models{
public class PermissionRequest
{
    public int Id { get; set; }
    public string RequestedBy { get; set; }
    public int EducationId { get; set; }
    public string Reason { get; set; }
    public DateTime RequestDate { get; set; }
    public bool IsApproved { get; set; }
    public DateTime? ApprovedDate { get; set; }  //Onaylama tarihi
    public string? ApprovedBy { get; set; }
} 
}