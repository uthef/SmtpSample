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

await emailService.SendMessage(recipients[0], 
    "Test message", "<b>Hello!</b>", credentials?.UserName ?? "");

