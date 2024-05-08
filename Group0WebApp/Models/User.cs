using System.Collections.Generic;

namespace Group_WebApp.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? ProfilePicture { get; set; }
        public virtual ICollection<File> Files { get; set; } = new List<File>();
    }
}
