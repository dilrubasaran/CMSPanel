namespace identity_singup.Areas.Admin.ViewModels
{
    //E�itmen  e�tim onay viewmodel
    //data annation gerekli de�il tabloda g�stermek i�in kullan�l�yor 
    public class PendingRequestViewModel
    {
        public int RequestId { get; set; }
        public string InstructorName { get; set; }
        public string EduName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
    }
} 