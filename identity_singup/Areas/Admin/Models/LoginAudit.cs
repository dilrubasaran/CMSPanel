using System;
using System.ComponentModel.DataAnnotations;

namespace identity_singup.Models
{
    public class LoginAudit
    {
        public int Id { get; set; }
        
        [Required]
        public string UserId { get; set; }
        
        [Required]
        public string UserName { get; set; }
        
        [Required]
        public DateTime LoginTime { get; set; }
        
        [Required]
        public string IpAddress { get; set; }
        
        public string? UserAgent { get; set; }
        
        [Required]
        public bool IsSuccess { get; set; }
        
        public string? FailureMessage { get; set; } 
        
        public string? UserRole { get; set; }
    }
} 