namespace CityInfo.API.Services;
public class MockMailService : IMailService
{
    protected string sender = string.Empty;
    protected string reciever = string.Empty;

    public MockMailService(IConfiguration configuration)
    {
        sender = configuration["MailService:SenderMailAddress"];
        reciever = configuration["MailService:RecieverMailAddress"];
    }

    public Task Send(string subject, string body)
    {
        Console.WriteLine($"Mail from {sender} to {reciever} on {DateTime.Now}");
        Console.WriteLine($"Mailing service: {nameof(MockMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {body}");
        return Task.CompletedTask;
    }
}
