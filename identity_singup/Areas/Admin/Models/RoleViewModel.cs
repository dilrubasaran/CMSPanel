namespace identity_signup.Areas.Admin.Models
{

    //data annation gerekli de�il tabloda g�stermek i�in kullan�l�yor 
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int PermissionLevel { get; set; }
    }
} 