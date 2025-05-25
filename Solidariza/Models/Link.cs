using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Solidariza.Models.Enum;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class Link
    {
        public int LinkId { get; set; }

        public LinkType Type { get; set; }

        public string? Url { get; set; }

        public int ProfileId { get; set; }

        public virtual Profile? Profile { get; set; }
    }

    public class NewLink
    {
        [JsonRequired]
        [Required]
        public LinkType Type { get; set; }

        public string? Url { get; set; }

        [JsonRequired]
        [Required]
        public int ProfileId { get; set; }
    }
}
