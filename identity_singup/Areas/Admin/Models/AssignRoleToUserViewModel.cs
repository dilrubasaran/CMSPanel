namespace identity_signup.Areas.Admin.Models
{
    public class AssignRoleToUserViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool Exist { get; set; }
        public int PermissionLevel { get; set; }
    }
} 