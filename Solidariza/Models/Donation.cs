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
        [JsonPropertyName("qrcode_base64")]
        public string Qrcode_base64 { get; set; }

        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("key_type")]
        public string Key_type { get; set; }

        [JsonPropertyName("key")]
        public string Key { get; set; }

        [JsonPropertyName("amount")]
        public string Amount { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("city")]
        public string? City { get; set; }

        [JsonPropertyName("formated_amount")]
        public string? Formated_amount { get; set; }

    }
}
