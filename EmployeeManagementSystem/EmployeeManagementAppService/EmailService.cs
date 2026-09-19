using System.Net;
using System.Net.Mail;

namespace EmployeeManagementAppService;

/// <summary>
/// Sends emails through an SMTP server (configured for Mailtrap in appsettings.json).
/// </summary>
public class EmailService
{
    private readonly string _host;
    private readonly int _port;
    private readonly string _username;
    private readonly string _password;
    private readonly string _fromAddress;
    private readonly string _fromName;

    public EmailService(string host, int port, string username, string password, string fromAddress, string fromName)
    {
        _host = host;
        _port = port;
        _username = username;
        _password = password;
        _fromAddress = fromAddress;
        _fromName = fromName;
    }

    public void SendEmail(string toAddress, string subject, string body)
    {
        using var client = new SmtpClient(_host, _port)
        {
            Credentials = new NetworkCredential(_username, _password),
            EnableSsl = true
        };

        using var message = new MailMessage
        {
            From = new MailAddress(_fromAddress, _fromName),
            Subject = subject,
            Body = body,
            IsBodyHtml = false
        };
        message.To.Add(toAddress);

        client.Send(message);
    }
}
