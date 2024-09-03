namespace MinAPIMusicProject.DTOs
{
    public class UserRegisterRequest
    {
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
    }

    public class UserLoginRequest
    {
        public string Login { get; set; }
        public string Password { get; set; }
    }

}
