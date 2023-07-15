using System.ComponentModel.DataAnnotations;

namespace People.Models
{
    public class ContactInfo
    {
        [Key]
        public Guid Id { get; set; }
        public string InfoType { get; set; }
        public string InfoContent { get; set; }
    }
}
