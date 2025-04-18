using identity_singup.Models;

//data annation gerekli deðil tabloda göstermek için kullanýlýyor 
public class LoginAttemptsViewModel
{
    public List<LoginAudit> LoginAttempts { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
} 