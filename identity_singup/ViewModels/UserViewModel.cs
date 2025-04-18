namespace identity_signup.ViewModels
{
    //data annation gerekli değil tabloda göstermek için kullanılıyor 
    public class UserViewModel
        {
            public string UserName { get; set; }
            public string Email { get; set; }
            public string Phone { get; set; }
            public string City { get; set; }
            public string Password { get; set; }
            public string PictureUrl { get; set; }
        }

    
}
