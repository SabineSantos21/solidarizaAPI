using Solidariza.Models.Enum;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Solidariza.Models
{
    public class DonationQRCodeRequest
    {
        [JsonPropertyName("key_type")]
        public string Key_type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("amount")]
        public string? Amount { get; set; }

    }

    public class DonationQRCodeResponse
    {
        public string Qrcode_base64 { get; set; }

        public string Code { get; set; }

        public string Key_type { get; set; }

        public string Key { get; set; }

        public string Amount { get; set; }

        public string Name { get; set; }

        public string? City { get; set; }

        public string? Formated_amount { get; set; }

    }
}
