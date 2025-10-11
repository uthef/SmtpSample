using System.Net;
using SmtpSample;
using System.Text.Json;
using System.Text.Json.Nodes;

NetworkCredential? credentials;
JsonObject? serverConfig;

await using (var fs = new FileStream("PrivateCredentials.json", FileMode.Open))
{
    credentials = JsonSerializer.Deserialize<NetworkCredential>(fs);
}

await using (var fs = new FileStream("PrivateServerConfig.json", FileMode.Open))
{
    serverConfig = JsonSerializer.Deserialize<JsonObject>(fs);
}

if (serverConfig is null)
{
    throw new Exception("Server configuration missing");
}

var host = serverConfig["Host"]?.AsValue().ToString() ?? "";
var port = serverConfig["Port"]?.AsValue().Deserialize<int>() ?? 0;
var enableSsl = serverConfig["EnableSsl"]?.AsValue().Deserialize<bool>() ?? true;
var recipients = serverConfig["Recipients"]?.AsArray().Deserialize<string[]>() ?? [];

using var emailService = new EmailService(host, port, credentials, enableSsl);

if (recipients.Length == 0)
{
    throw new Exception("No recipients specified");
}

for (var i = 0; i < 5; i++)
{
    if (i > 0) await Task.Delay(TimeSpan.FromSeconds(1));
    
    var subject = $"Test message #{i + 1}";
    await emailService.SendMessageAsync(recipients[0], subject, "<b>Hello!</b>", 
        credentials?.UserName ?? "");
    
    Console.WriteLine($"{subject} has been sent");
}