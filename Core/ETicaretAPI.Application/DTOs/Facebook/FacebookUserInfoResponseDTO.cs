using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace ETicaretAPI.Application.DTOs.Facebook;

public class FacebookUserInfoResponseDTO
{ 
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("email")]
    public string Email { get; set; }
    [JsonPropertyName("name")]
    public string Name { get; set; }
}
