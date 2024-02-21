using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace LoginAndRegister.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public bool Status { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; } // שימוש ב-JsonIgnore למניעת חשיפה ב-API
        public string ShortDescription { get; set; }
        public string LongDescription { get; set; }
    }
}
