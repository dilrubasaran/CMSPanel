using identity_singup.Models;

//data annation gerekli de�il tabloda g�stermek i�in kullan�l�yor 
public class LoginAttemptsViewModel
{
    public List<LoginAudit> LoginAttempts { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
} 