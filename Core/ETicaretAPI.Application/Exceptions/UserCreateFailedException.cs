namespace ETicaretAPI.Application.Exceptions;

public class UserCreateFailedException : Exception
{
    public UserCreateFailedException() : base("Kullanıcı oluşturulurken beklenmedik bir hata ile karşılaşıldı.") // default value oluşturma
    {
    }
    public UserCreateFailedException(string? message) : base(message) 
    {
    }
    public UserCreateFailedException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}
