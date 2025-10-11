using System.Net;
using System.Net.Mail;
using System.Text.Encodings.Web;

namespace SmtpSample;

class EmailService : IDisposable
{
    private readonly SmtpClient _smtpClient;
    private static readonly HtmlEncoder DefaultHtmlEncoder = HtmlEncoder.Default;
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

    public async Task SendMessageAsync(string[] recipients, string subject, string text, string from = "", 
        bool allowHtmlTags = false, CancellationToken ct = default)
    {
        using var mailMessage = new MailMessage();

        if (!allowHtmlTags)
        {
            subject = DefaultHtmlEncoder.Encode(subject);
            text = DefaultHtmlEncoder.Encode(text);
        }
        
        mailMessage.Subject = subject;
        mailMessage.Body = MessageTemplate.Replace("{subject}", subject).Replace("{content}", text);
        mailMessage.IsBodyHtml = true;
        mailMessage.From = new MailAddress(from);

        foreach (var recipient in recipients)
        {
            mailMessage.To.Add(recipient);
        }
        
        await _smtpClient.SendMailAsync(mailMessage, ct);
    }
    
    public async Task SendMessageAsync(string recipient, string subject, string text, string from = "", 
        bool allowHtmlTags = false, CancellationToken ct = default)
    {
        await SendMessageAsync([recipient], subject, text, from, allowHtmlTags, ct);
    }

    public void Dispose()
    {
        _smtpClient.Dispose();
    }
}