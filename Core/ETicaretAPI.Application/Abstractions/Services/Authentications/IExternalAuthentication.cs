﻿namespace ETicaretAPI.Application.Abstractions.Services.Authentications;

public interface IExternalAuthentication
{
    Task <DTOs.Token> FacebookLoginAsync(string authToken, int accessTokenLifeTime);
    Task <DTOs.Token> GoogleLoginAsync(string IdToken, int accessTokenLifeTime);
}
