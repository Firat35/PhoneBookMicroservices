using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace People.Models
{
    public class ContactInfo
    {
        [Key]

        public Guid Id { get; set; }
        public string InfoType { get; set; }
        public string InfoContent { get; set; }
        [JsonIgnore]
        public Guid PersonId { get; set; }
        [JsonIgnore]
        public Person Person { get; set; }
    }
}
