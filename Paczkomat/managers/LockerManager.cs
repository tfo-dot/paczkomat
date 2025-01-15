using Paczkomat.consts;

namespace Paczkomat.managers;

public class LockerManager
{
    private readonly Dictionary<(int, int), Package> _packages = [];

    private readonly int _row;
    private readonly int _column;

    private int _currentId;

    public (int, int) Size => (_row, _column);

    public LockerManager()
    {
        var paczkomatSize = SettingsManager.Size.Split("x");

        _column = Convert.ToInt32(paczkomatSize[0]);
        _row = Convert.ToInt32(paczkomatSize[1]);
    }

    public void PrettyPrint()
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

    public Package AddPackage(string pin, string phone, string email)
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
        Console.WriteLine($"packages size {_packages.Count}");

        foreach (var variable in _packages)
        {
            Console.WriteLine($"{variable.Value}");
        }

        Console.WriteLine($"Phone Number: {phone}, Pin: {pin}");

        return (from packageEntry in _packages
            where packageEntry.Value.Phone == phone && packageEntry.Value.Pin == pin
            select packageEntry.Value).FirstOrDefault();
    }

    public Package? GetPackage(int column, int row)
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

    public void UpdatePin(int x, int y, string pin)
    {
        var package = GetPackage(x, y);

        if (package == null)
        {
            return;
        }

        _packages[(x, y)] = package with { Pin = pin };
    }

    public void Reset()
    {
        _packages.Clear();
    }

    public void ResetFor(string phone)
    {
        var ownedPackages = _packages.ToList().Where(elt => elt.Value.Phone == phone).ToList();

        foreach (var (key, value) in ownedPackages)
        {
            var newPin = Paczkomat.Instance.GetNewPin();
            _packages[key] = value with { Pin = newPin };

            Paczkomat.Instance.SendNewPinMessage(value.Email, value.Pin);
        }
    }

    public void Load(List<Package> records)
    {
        foreach (var record in records)
        {
            _packages[(record.Row, record.Column)] = record;
        }
    }

    public List<Package> GetPackages()
    {
        return _packages.Values.ToList();
    }
}