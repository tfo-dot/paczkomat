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

    private ServiceMode _serviceMode = ServiceMode.Courier;

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
        var paczkomatSize = SettingsManager.Size.Split("x");

        _column = Convert.ToInt32(paczkomatSize[0]);
        _row = Convert.ToInt32(paczkomatSize[1]);
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

    public Courier Courier { get; } = new Courier("1337", "111301337", "Radosław", "Partyka");

    public Servicer Servicer { get; } = new Servicer("0803", "111302002", "Radosław", "Partyka");

    public ServiceMode? OpenLock(string phone, string pin)
    {
        if (Servicer.Pin == pin && Servicer.Phone == phone) return ServiceMode.Service;
        if (Courier.Pin == pin && Courier.Phone == phone) return ServiceMode.Courier;

        var package = GetPackage(phone, pin);
        
        _attempts[phone] = _attempts.GetValueOrDefault(phone, 0) + 1;

        if (package == null)
        {
            PrettyPrint();

            if (_attempts.GetValueOrDefault(phone, 0) >= 3)
            {
                ResetFor(phone);
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

        var rv = AddPackage(pin, phone, email);
        
        PrettyPrint();
        
        return rv ;
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
        //todo write spaghetti here to save
    }

    public void ResetPin(int x, int y)
    {
        var pin = _pinManager.GetNext();

        UpdatePin(x, y, pin);
    }

    public void Reset()
    {
        _packages.Clear();
        _pinManager.Reset();
    }

    public void SendNewPinMessage(string email, string pin) =>
        _mailManager.NewPinEmail(email, pin);

    public void SendSupportMessage(string courierName, string courierLastname) =>
        _mailManager.SendSupportMessage(courierName, courierLastname);

    public bool HasPackageAt(int x, int y) => GetPackage(x, y) != null;

    public string GetNewPin() =>
        _pinManager.GetNext();
    
    private readonly Dictionary<(int, int), Package> _packages = [];

    private readonly int _row;
    private readonly int _column;

    private int _currentId;

    public (int, int) Size => (_row, _column);

    private void PrettyPrint()
    {
        for (var column = 0; column < _column; column++)
        {
            for (var row = 0; row < _row; row++)
            {
                Console.Write(_packages.ContainsKey((column, row)) ? "*" : " ");
            }

            Console.WriteLine("|");
        }
    }

    public bool HasEmptyLockers()
    {
        return _row * _column - _packages.Count > 0;
    }

    private (int, int) FindNextPlace()
    {
        for (var column = 0; column < _column; column++)
        {
            for (var row = 0; row < _row; row++)
            {
                if (!_packages.ContainsKey((column, row)))
                {
                    return (column, row);
                }
            }
        }

        throw new Exception("No place found");
    }

    private int GetNextId()
    {
        _currentId++;
        return _currentId;
    }

    private Package AddPackage(string pin, string phone, string email)
    {
        var (column, row) = FindNextPlace();

        Console.WriteLine($"{column}, {row}");

        _packages[(column, row)] = new Package(pin, phone, email, column, row, GetNextId());

        foreach (var variable in _packages)
        {
            Console.WriteLine($"{variable.Key} - {variable.Value}");
        }

        return _packages[(column, row)];
    }

    public Package? GetPackage(int id)
    {
        return (from packageEntry in _packages
            where packageEntry.Value.Id == id
            select packageEntry.Value).FirstOrDefault();
    }

    public Package? GetPackage(string phone, string pin)
    {
        return (from packageEntry in _packages
            where packageEntry.Value.Phone == phone && packageEntry.Value.Pin == pin
            select packageEntry.Value).FirstOrDefault();
    }

    private Package? GetPackage(int column, int row)
    {
        _packages.TryGetValue((column, row), out var value);
        return value;
    }

    public void RemovePackage(int packageId)
    {
        var package = GetPackage(packageId);

        if (package == null)
        {
            return;
        }

        _packages.Remove((package.Column, package.Row));
    }

    public void RemovePackage(int x, int y)
    {
        var package = GetPackage(x, y);

        if (package == null)
        {
            return;
        }

        _packages.Remove((x, y));
    }

    private void UpdatePin(int x, int y, string pin)
    {
        var package = GetPackage(x, y);

        if (package == null)
        {
            return;
        }

        _packages[(x, y)] = package with { Pin = pin };
    }

    private void ResetFor(string phone)
    {
        var ownedPackages = _packages.ToList().Where(elt => elt.Value.Phone == phone).ToList();

        foreach (var (key, value) in ownedPackages)
        {
            var newPin = GetNewPin();
            _packages[key] = value with { Pin = newPin };

            SendNewPinMessage(value.Email, newPin);
        }
    }
}