namespace PruebaDsesempeño.Services;

using MailKit.Net.Smtp;
using PruebaDsesempeño.Models;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

public class EmailService
{
    private readonly SmtpSettings _settings;

    public EmailService(IOptions<SmtpSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendAsync(string to, string subject, string body, bool isHtml = false)
    {
        // MimeKit construye el mensaje MIME
        var message = new MimeMessage();

        message.From.Add(new MailboxAddress(_settings.FromName, _settings.Username));
        message.To.Add(MailboxAddress.Parse(to));
        message.Subject = subject;

        // BodyBuilder permite combinar parte texto plano + HTML + adjuntos
        var bodyBuilder = new BodyBuilder();

        if (isHtml)
            bodyBuilder.HtmlBody = body;
        else
            bodyBuilder.TextBody = body;

        message.Body = bodyBuilder.ToMessageBody();

        // MailKit se conecta y envía
        using var client = new SmtpClient();

        // SecureSocketOptions.StartTls es el equivalente correcto para puerto 587
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_settings.Username, _settings.Password);
        await client.SendAsync(message);
        await client.DisconnectAsync(quit: true);
    }
}