namespace identity_singup.Areas.Admin.ViewModels
{
    public class PendingRequestViewModel
    {
        public int RequestId { get; set; }
        public string InstructorName { get; set; }
        public string EduName { get; set; }
        public DateTime RequestDate { get; set; }
        public string Reason { get; set; }
    }
} 