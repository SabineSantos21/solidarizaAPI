using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Solidariza.Models
{
    public class Profile
    {
        public int ProfileId { get; set; }

        public int UserId { get; set; }

        public virtual User User { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }
    }
    
    public class NewProfile
    {

        public int UserId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }
    }
    
    public class UpdateProfile
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }
    }
}
