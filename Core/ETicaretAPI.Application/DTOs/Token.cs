namespace ETicaretAPI.Application.DTOs;

public class Token
{
	public string AccessToken { get; set; }
	public DateTime Expiration { get; set; } // Token'ın süresi
	public string RefreshToken { get; set; }
}
