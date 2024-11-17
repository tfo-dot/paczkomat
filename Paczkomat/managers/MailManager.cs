using System.Net.Mail;

namespace Paczkomat.managers;

public class MailManager
{

    private readonly SmtpClient _client = new(SettingsManager.EmailHost, SettingsManager.EmailPort);
    private readonly MailAddress _sender = new($"{SettingsManager.Code}@paczkomat.dev.pl");
    
    public void NewPinEmail(string email, string pin)
    {
        var receiver = new MailAddress(email);

        var msg = new MailMessage(_sender, receiver);
        
        msg.Subject = "Twój nowy pin do skrytki";
        msg.Body = $"Pin do twojej paczki w paczkomacie {SettingsManager.Code} został zresetowany! Nowy pin to: {pin}. Pamiętaj żeby odebrać paczkę na {SettingsManager.Address}!";
        
        _client.Send(msg);
    }

    public void SendSupportMessage(string courierName, string courierLastName)
    {
        var receiver = new MailAddress("support@paczkomat.dev.pl");

        var msg = new MailMessage(_sender, receiver);
        
        msg.Subject = "Paczkomat wymaga uwagi";
        msg.Body = $"Paczkomat {SettingsManager.Code} wymaga uwagi, kurier ({courierName} {courierLastName}) zgłosił problem!\nAdres paczkomatu: {SettingsManager.Address}";
        
        _client.Send(msg);
    }
    
    public void SendNewPackageMessage(string recipent, string pin)
    {
        var receiver = new MailAddress(recipent);

        var msg = new MailMessage(_sender, receiver);
        
        msg.Subject = "Nowa paczka w paczkomacie!";
        msg.Body = $"Pin do twojej paczki w paczkomacie {SettingsManager.Code} został zresetowany! Nowy pin to: {pin}. Pamiętaj żeby odebrać paczkę na {SettingsManager.Address}!";
        
        _client.Send(msg);
    }
}