namespace CityInfo.API.Services;
public class MockMailService : IMailService
{
    protected string sender = "info@myhost.com";
    protected string reciever = "admin@myhost.com";

    public Task Send(string subject, string body)
    {
        Console.WriteLine($"Mail from {sender} to {reciever} on {DateTime.Now}");
        Console.WriteLine($"Mailing service: {nameof(MockMailService)}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine($"Message: {body}");
        return Task.CompletedTask;
    }
}
