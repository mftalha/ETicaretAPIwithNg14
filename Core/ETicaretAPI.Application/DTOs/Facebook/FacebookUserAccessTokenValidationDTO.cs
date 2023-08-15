using System.Text.Json.Serialization;

namespace ETicaretAPI.Application.DTOs.Facebook;

public class FacebookUserAccessTokenValidationDTO
{
    [JsonPropertyName("data")]
    public FacebookUserAccessTokenValidationDataDTO Data { get; set; }
}

public class FacebookUserAccessTokenValidationDataDTO
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }
    [JsonPropertyName("user_id")]
    public string UserId { get; set; }
}


/*
 * 
 * 
 
{"data":{"app_id":"1038169094012077","type":"USER","application":"Small E-Commerce","data_access_expires_at":1699879220,"expires_at":1692108000,"is_valid":true,"scopes":["email","public_profile"],"user_id":"820954199632249"}}

*/