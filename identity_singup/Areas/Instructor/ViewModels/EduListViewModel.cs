using System;
using identity_signup.Areas.Instructor.Models;

namespace identity_signup.Areas.Instructor.ViewModels
{
    public class EduListViewModel
    {
        public int Id { get; set; }
        public string EduName { get; set; }
        public string Description { get; set; }
        public EduType  EduType { get; set; }
        public string EduDuration { get; set; }
        public decimal EduPrice { get; set; }
        public string CreatedBy { get; set; }
        public string CreatorName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
} 