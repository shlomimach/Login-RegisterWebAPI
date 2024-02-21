using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LoginAndRegister.Models
{
    public class UserRegistrationDto
    {
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
    }
}
