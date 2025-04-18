namespace identity_signup.Areas.Admin.Models
{

    //data annation gerekli deðil tabloda göstermek için kullanýlýyor 
    public class RoleViewModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int PermissionLevel { get; set; }
    }
} 