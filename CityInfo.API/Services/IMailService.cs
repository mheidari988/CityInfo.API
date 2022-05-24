namespace CityInfo.API.Services;
public interface IMailService
{
    Task Send(string subject, string body);
}
