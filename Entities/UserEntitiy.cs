using System.ComponentModel.DataAnnotations;

namespace IdentityAPI.Entities
{
    public class UserEntitiy
    {
       
        public int Id { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
