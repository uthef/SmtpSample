using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace SmtpSample;

class EmailService : IDisposable
{
    private readonly SmtpClient _smtpClient;
    private static readonly HtmlEncoder HtmlEncoderInstance = HtmlEncoder.Default;
    private static readonly string MessageTemplate;

    static EmailService()
    {
        MessageTemplate = File.ReadAllText("MessageTemplate.html");
    }

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

    public async Task SendMessageAsync(string[] recipients, string subject, string text, string from = "", CancellationToken ct = default)
    {
        using var mailMessage = new MailMessage();
        
        mailMessage.Subject = subject;
        mailMessage.Body = string.Format(MessageTemplate, subject, HtmlEncoderInstance.Encode(text));
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(from);

        foreach (var recipient in recipients)
        {
            mailMessage.To.Add(recipient);
        }
        
        await _smtpClient.SendMailAsync(mailMessage, ct);
    }
    
    public async Task SendMessageAsync(string recipient, string subject, string text, string from = "", CancellationToken ct = default)
    {
        await SendMessageAsync([recipient], subject, text, from, ct);
    }

    public void Dispose()
    {
        _smtpClient.Dispose();
    }
}