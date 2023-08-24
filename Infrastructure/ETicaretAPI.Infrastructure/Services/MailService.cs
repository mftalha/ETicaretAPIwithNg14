using ETicaretAPI.Application.Abstractions.Services;
using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace ETicaretAPI.Infrastructure.Services;

public class MailService : IMailService
{
    readonly IConfiguration _configuration;
    public async Task SendMessageAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendMessageAsync(new[] {to},subject, body, isBodyHtml);// alttaki overload'ı kullanabiliriz.
    }

    public async Task SendMessageAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
    {
        MailMessage mail = new();
        mail.IsBodyHtml = isBodyHtml;
        foreach(string to in tos)
            mail.To.Add(to);
        mail.Subject = subject;
        mail.Body = body;
        mail.From = new(_configuration["Mail:Username"], "NG E-Ticaret", System.Text.Encoding.UTF8); //kim gönderecek.

        SmtpClient smtp = new();
        smtp.Credentials = new NetworkCredential(_configuration["Mail:Username"], _configuration["Mail:Password"]);
        smtp.Port = 587;
        smtp.EnableSsl= true; //smtp.EnableSsl durumunu aktifleştiriyoruz
        smtp.Host = _configuration["Mail:Host"];
        await smtp.SendMailAsync(mail);
    }
}
