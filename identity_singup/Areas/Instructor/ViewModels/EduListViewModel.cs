using System;
using identity_signup.Areas.Instructor.Models;

namespace identity_signup.Areas.Instructor.ViewModels
{
    //data annation gerekli de�il card yap�s�nda  g�stermek i�in kullan�l�yor 
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