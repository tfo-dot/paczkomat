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

    private ServiceMode _serviceMode = ServiceMode.Normal;

    private readonly LockerManager _lockerManager = new();

    private readonly PinManager _pinManager = new();
    private readonly MailManager _mailManager = new();

    private readonly Dictionary<string, int> _attempts = new();

    public int PackageId = -1;

    public Form GetMainView()
    {
        return _serviceMode switch
        {
            ServiceMode.Normal => new MainCustomer(),
            ServiceMode.Courier => new MainCourier(),
            ServiceMode.Service => new MainService(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private Paczkomat()
    {
        if (!File.Exists("data.xml"))
        {
            return;
        }

        var data = new DataManager("data.xml").Deserialize();

        _pinManager.Pins = data.Pins;
        _lockerManager.Load(data.Packages.Select(elt => elt.ToNormal()).ToList());
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

    public Courier Courier { get; } = new("1337", "111301337", "Radosław", "Partyka");

    public Servicer Servicer { get; } = new("0803", "111302002", "Radosław", "Partyka");

    public ServiceMode? OpenLock(string phone, string pin)
    {
        if (Servicer.Pin == pin && Servicer.Phone == phone) return ServiceMode.Service;
        if (Courier.Pin == pin && Courier.Phone == phone) return ServiceMode.Courier;

        var package = _lockerManager.GetPackage(phone, pin);

        _attempts[phone] = _attempts.GetValueOrDefault(phone, 0) + 1;

        if (package == null)
        {
            if (_attempts.GetValueOrDefault(phone, 0) >= 3)
            {
                _lockerManager.ResetFor(phone);
            }

            return null;
        }

        _attempts.Remove(phone);

        PackageId = package.Id;

        return ServiceMode.Normal;
    }

    public Package AddPackage(string email, string phone)
    {
        var pin = _pinManager.GetNext();

        _mailManager.SendNewPackageMessage(email, pin);

        return _lockerManager.AddPackage(pin, phone, email);
    }

    public void SwitchMode(ServiceMode mode)
    {
        _serviceMode = mode;

        if (mode == ServiceMode.Normal)
        {
            PackageId = -1;
        }
    }

    public void Shutdown()
    {
        _mailManager.Shutdown();

        var tempData = new DataContainer
        {
            Pins = _pinManager.Pins
        }.AddPackages(_lockerManager.GetPackages());

        new DataManager("data.xml").Serialize(tempData);
    }

    public void ResetPin(int x, int y)
    {
        var pin = _pinManager.GetNext();

        _lockerManager.UpdatePin(x, y, pin);
    }

    public void Reset()
    {
        _lockerManager.Reset();
        _pinManager.Reset();
    }

    public void SendNewPinMessage(string email, string pin) => _mailManager.NewPinEmail(email, pin);

    public void SendSupportMessage(string courierName, string courierLastname) =>
        _mailManager.SendSupportMessage(courierName, courierLastname);

    public bool HasPackageAt(int x, int y) => _lockerManager.GetPackage(x, y) != null;

    public string GetNewPin() => _pinManager.GetNext();

    public void RemovePackage(int x, int y) => _lockerManager.RemovePackage(x, y);
    public void RemovePackage(int id) => _lockerManager.RemovePackage(id);

    public Package? GetPackage(int id) => _lockerManager.GetPackage(id);

    public bool HasEmptyLockers => _lockerManager.HasEmptyLockers();

    public (int, int) Size => _lockerManager.Size;
}