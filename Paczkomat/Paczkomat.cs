using MySql.Data.MySqlClient;
using Paczkomat.consts;
using Paczkomat.managers;
using Paczkomat.views.courier;
using Paczkomat.views.customer;
using Paczkomat.views.service;

namespace Paczkomat;

public class Paczkomat
{
    private static Paczkomat _instance = null!;
    private static readonly object Padlock = new();

    private ServiceMode _serviceMode = ServiceMode.Service;
    private int _viewMeta = 2;

    private readonly LockerManager _lockerManager = new();
    private readonly PinManager _pinManager = new();
    private readonly MailManager _mailManager = new();

    private readonly Dictionary<string, int> _attempts = new();

    public Form GetMainView()
    {
        return _serviceMode switch
        {
            ServiceMode.Normal => new MainCustomer(),
            ServiceMode.Courier => new MainCourier(_viewMeta),
            ServiceMode.Service => new MainService(_viewMeta),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private readonly MySqlConnection _connection = new(SettingsManager.ConnectionString);

    private Paczkomat()
    {
        _connection.Open();

        _lockerManager.SetConnection(ref _connection);
        _pinManager.SetConnection(ref _connection);

        _lockerManager.Sync();
        _pinManager.Sync();

        _lockerManager.PrettyPrint();
    }

    public static Paczkomat Instance
    {
        get
        {
            lock (Padlock)
            {
                // ReSharper disable once NullCoalescingConditionIsAlwaysNotNullAccordingToAPIContract
                return _instance ??= new Paczkomat();
            }
        }
    }

    public (int, int) Size => _lockerManager.Size;

    public LockerMeta OpenLock(string phone, string pin)
    {
        using var serviceCmd =
            new MySqlCommand("select id from service where phone = @phone and pin = @pin", _connection);

        serviceCmd.Parameters.AddWithValue("@phone", phone);
        serviceCmd.Parameters.AddWithValue("@pin", pin);

        var serviceResult = serviceCmd.ExecuteScalar();

        if (serviceResult is not null) return new LockerMeta(ServiceMode.Service, Convert.ToInt32(serviceResult));

        using var courierCmd =
            new MySqlCommand("select id from courier where phone = @phone and pin = @pin", _connection);

        courierCmd.Parameters.AddWithValue("@phone", phone);
        courierCmd.Parameters.AddWithValue("@pin", pin);

        var courierResult = courierCmd.ExecuteScalar();

        if (courierResult is not null) return new LockerMeta(ServiceMode.Courier, Convert.ToInt32(courierResult));

        using var packageCmd =
            new MySqlCommand("select id from packages where phone = @phone and pin = @pin", _connection);


        packageCmd.Parameters.AddWithValue("@phone", phone);
        packageCmd.Parameters.AddWithValue("@pin", pin);

        var result = packageCmd.ExecuteScalar();

        _attempts[phone] = _attempts.GetValueOrDefault(phone, 0) + 1;

        if (result == null)
        {
            if (_attempts.GetValueOrDefault(phone, 0) >= 3)
            {
                _lockerManager.ResetFor(phone);
            }

            throw new Exception("Failed to open lock");
        }

        _attempts.Remove(phone);

        return new LockerMeta(ServiceMode.Normal, Convert.ToInt32(result));
    }

    public Courier GetCourier(int id)
    {
        Console.WriteLine(id);
        using var cmd = new MySqlCommand("select * from courier where id = @id", _connection);

        cmd.Parameters.AddWithValue("@id", id);

        using var result = cmd.ExecuteReader();

        result.Read();

        var pin = result["pin"].ToString();
        var phone = result["phone"].ToString();
        var name = result["name"].ToString();
        var lastname = result["lastname"].ToString();

        return new Courier(pin!, phone!, name!, lastname!, id);
    }

    public Servicer GetServicer(int id)
    {
        using var cmd = new MySqlCommand("select * from service where id = @id", _connection);

        cmd.Parameters.AddWithValue("@id", id);

        using var result = cmd.ExecuteReader();

        result.Read();

        var pin = result["pin"].ToString();
        var phone = result["phone"].ToString();
        var name = result["name"].ToString();
        var lastname = result["lastname"].ToString();

        return new Servicer(pin!, phone!, name!, lastname!, id);
    }

    public Package AddPackage(string email, string phone)
    {
        var pin = _pinManager.GetNext();

        _mailManager.SendNewPackageMessage(email, pin.Value);

        return _lockerManager.AddPackage(pin.Value, phone, email);
    }

    public void SwitchMode(LockerMeta data)
    {
        _serviceMode = data.ServiceMode;
        _viewMeta = data.Id;
    }

    public void Shutdown()
    {
        _connection.Close();
    }

    public void ResetPin(int x, int y)
    {
        var pin = _pinManager.GetNext();

        _lockerManager.UpdatePin(x, y, pin.Value);
    }

    public void Reset()
    {
        _lockerManager.Reset();
        _pinManager.Reset();
    }

    public void SendNewPinMessage(string email, string pin) =>
        _mailManager.NewPinEmail(email, pin);

    public void SendSupportMessage(string courierName, string courierLastname) =>
        _mailManager.SendSupportMessage(courierName, courierLastname);

    public void RemovePackage(int x, int y) =>
        _lockerManager.RemovePackage(x, y);

    public bool HasPackageAt(int x, int y) =>
        _lockerManager.GetPackage(x, y) != null;

    public Package GetPackage(int id) =>
        _lockerManager.GetPackage(id)!;

    public void RemovePackage(int packageId) =>
        _lockerManager.RemovePackage(packageId);

    public string GetNewPin() =>
        _pinManager.GetNext().Value;

    public bool HasEmptyLockers() =>
        _lockerManager.HasEmptyLockers();
}

public record LockerMeta(ServiceMode ServiceMode, int Id);