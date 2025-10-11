using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace SmtpSample;

class EmailService : IDisposable
{
    private readonly SmtpClient _smtpClient;

    public EmailService(string host, int port, NetworkCredential? credentials = null, bool enableSsl = true)
    {
        _smtpClient = new SmtpClient
        {
            Host = host,
            Port = port,
            EnableSsl = enableSsl,
            Credentials = credentials
        };
    }

    public async Task SendMessage(string[] recipients, string subject, string text, string from = "", CancellationToken ct = default)
    {
        using var mailMessage = new MailMessage();
        var encoder = HtmlEncoder.Create();
        
        mailMessage.Subject = subject;
        mailMessage.Body = $"<h1>{subject}</h1><p>{encoder.Encode(text)}</p>";
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(from);

        foreach (var recipient in recipients)
        {
            mailMessage.To.Add(recipient);
        }
        
        await _smtpClient.SendMailAsync(mailMessage, ct);
    }
    
    public async Task SendMessage(string recipient, string subject, string text, string from = "", CancellationToken ct = default)
    {
        await SendMessage([recipient], subject, text, from, ct);
    }

    public void Dispose()
    {
        _smtpClient.Dispose();
    }
}