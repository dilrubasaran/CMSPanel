
//data annation gerekli değil tabloda göstermek için kullanılıyor 

// Tekil istatistik verilerini temsil eder
public class LoginStatViewModel
{
    public string UserRole { get; set; }
    public bool IsSuccess { get; set; }
    public int Count { get; set; }
}

//Tüm istatistikleri bir arada tutan container modeldir genel özet gibi 
public class LoginStatsViewModel
{
    public string TimeRange { get; set; }
    public List<LoginStatViewModel> Stats { get; set; }
    public int TotalLogins { get; set; }
    public int SuccessfulLogins { get; set; }
    public int FailedLogins { get; set; }
} 