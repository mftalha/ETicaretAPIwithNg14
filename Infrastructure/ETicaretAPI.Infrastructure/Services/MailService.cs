using ETicaretAPI.Application.Abstractions.Services;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net;
using System.Text;

namespace ETicaretAPI.Infrastructure.Services;

public class MailService : IMailService
{
    readonly IConfiguration _configuration;

    public MailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task SendMailAsync(string to, string subject, string body, bool isBodyHtml = true)
    {
        await SendMailAsync(new[] {to},subject, body, isBodyHtml);// alttaki overload'ı kullanabiliriz.
    }

    public async Task SendMailAsync(string[] tos, string subject, string body, bool isBodyHtml = true)
    {
        /*
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
        */

        MimeMessage mimeMessage = new();
        mimeMessage.From.Add(new MailboxAddress(_configuration["Mail:Username"], _configuration["Mail:UserEmail"]));
        foreach (string to in tos)
            mimeMessage.To.Add(new MailboxAddress("User", to));

        mimeMessage.Subject = subject;

        BodyBuilder bodyBuilder = new();
        if (isBodyHtml == true)
            bodyBuilder.HtmlBody = body;
        else
            bodyBuilder.TextBody = body;
        mimeMessage.Body = bodyBuilder.ToMessageBody();

        SmtpClient client = new();
        client.Connect(_configuration["Mail:Host"], int.Parse(_configuration["Mail:Port"]), false);
        client.Authenticate(_configuration["Mail:UserEmail"], _configuration["Mail:Password"]);
        client.Send(mimeMessage);
        client.Disconnect(true);
    }

    public async Task SendPasswordResetMailAsync(string to, string userId, string resetToken)
    {
        /*
        StringBuilder mail = new();
        mail.AppendLine("Merhaba<br>Eğer yeni şifre talebinde bulunduysanız aşşağıdaki linkten şifrenizi yenileyebilirsiniz.<br><strong><a target=\"_blank\" href=\"");
        mail.AppendLine(_configuration["AngularClientUrl"]);
        mail.AppendLine("/update-password/");
        mail.AppendLine(userId);
        mail.AppendLine("/");
        mail.AppendLine(resetToken);
        mail.AppendLine("\">Yeni şifre talebi için tıklayınız.</a></strong><br><br><span style=\"font-size:12px;\">Not: Eğer ki bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız.</span><br>Saygılarımızla...<br><br><br>MTS - Small|E-Commerce");

        await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());
        */
        StringBuilder mail = new();
        mail.AppendLine("<p>Merhaba</p>");
        mail.AppendLine("<p>Eğer yeni şifre talebinde bulunduysanız aşağıdaki linkten şifrenizi yenileyebilirsiniz.</p>");
        mail.AppendLine("<p><strong><a target=\"_blank\" href=\"" + _configuration["AngularClientUrl"] + "/update-password/" + userId + "/" + resetToken + "\">Yeni şifre talebi için tıklayınız.</a></strong></p>");
        mail.AppendLine("<p><span style=\"font-size:12px;\">Not: Eğer ki bu talep tarafınızca gerçekleştirilmemişse lütfen bu maili ciddiye almayınız.</span></p>");
        mail.AppendLine("<p>Saygılarımızla...</p>");
        mail.AppendLine("<p><br><br><br>MTS - Small|E-Commerce</p>");

        await SendMailAsync(to, "Şifre Yenileme Talebi", mail.ToString());

    }
}

// https://www.youtube.com/watch?v=WXI_kS17rDs&ab_channel=MuratY%C3%BCceda%C4%9F => google mail gönderimi için takip edilen video


/*
 // target=\"_blank\" href=\"......./userId/resetToken.\" "
        // string içinde tırnak vermek sorun oldugunda => \" bu şekidle veriyoruz.
        // _blank => ilgili linki farklı bir sekmede açacak.
        // userId => hangi user'ı pass'ı değişecek
        // resetToken => burda bir token olacak ve .. kısmındaki linki ve userıd yi linkten değiştirip farklı bir userId li kişinin password'unu değiştirmelerini token ile engelliyeceğiz.
 */