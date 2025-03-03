using identity_singup.Models;

public class LoginAttemptsViewModel
{
    public List<LoginAudit> LoginAttempts { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
    public string SearchTerm { get; set; }
} 