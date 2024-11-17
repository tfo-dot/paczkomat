using MySql.Data.MySqlClient;
using Paczkomat.consts;

namespace Paczkomat.managers;

public class LockerManager
{
    private readonly Dictionary<(int, int), Package> _packages = [];

    private readonly int _row;
    private readonly int _column;

    private MySqlConnection? _connection;

    public (int, int) Size => (_row, _column);

    public LockerManager()
    {
        var paczkomatSize = SettingsManager.Size.Split("x");

        _column = Convert.ToInt32(paczkomatSize[0]);
        _row = Convert.ToInt32(paczkomatSize[1]);
    }

    public void SetConnection(ref MySqlConnection connection)
    {
        if (_connection != null)
        {
            throw new InvalidOperationException("Connection already set");
        }

        _connection = connection;
    }

    public void Sync()
    {
        using var packageCmd = new MySqlCommand("select * from packages", _connection);

        using var result = packageCmd.ExecuteReader();

        while (result.Read())
        {
            var pin = result["pin"].ToString();
            var phone = result["phone"].ToString();
            var email = result["email"].ToString();
            var column = Convert.ToInt32(result["column"]);
            var row = Convert.ToInt32(result["row"]);
            var id = Convert.ToInt32(result["id"]);

            _packages[(column, row)] = new Package(pin!, phone!, email!, column, row, id);
        }
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

    public Package AddPackage(string pin, string phone, string email)
    {
        var (column, row) = FindNextPlace();

        Console.WriteLine($"Adding package pin: `{pin.Select(_ => '*')}` to phone: `{phone}` with email: `{email}`");

        var insertCmd =
            new MySqlCommand(
                "insert into packages (pin, phone, email,`column`, `row`) values (@pin, @phone, @email, @column, @row)",
                _connection);

        insertCmd.Parameters.AddWithValue("@pin", pin);
        insertCmd.Parameters.AddWithValue("@phone", phone);
        insertCmd.Parameters.AddWithValue("@email", email);
        insertCmd.Parameters.AddWithValue("@column", column);
        insertCmd.Parameters.AddWithValue("@row", row);

        insertCmd.ExecuteNonQuery();

        using var fetchCmd = new MySqlCommand("select last_insert_id()", _connection);

        var id = Convert.ToInt32(fetchCmd.ExecuteScalar()!);

        _packages[(column, row)] = new Package(pin, phone, email, column, row, id);

        return _packages[(column, row)];
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

    public Package? GetPackage(int id)
    {
        return _packages.Values.FirstOrDefault(value => value.Id == id);
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

        using var cmd = new MySqlCommand("delete from packages where id = @id", _connection);

        cmd.Parameters.AddWithValue("@id", packageId);

        cmd.ExecuteNonQuery();

        _packages.Remove((package.Column, package.Row));
    }

    public void RemovePackage(int x, int y)
    {
        var package = GetPackage(x, y);

        if (package == null)
        {
            return;
        }

        using var cmd = new MySqlCommand("delete from packages where id = @id", _connection);

        cmd.Parameters.AddWithValue("@id", package.Id);

        _packages.Remove((x, y));

        cmd.ExecuteNonQuery();
    }

    public void UpdatePin(int x, int y, string pin)
    {
        var package = GetPackage(x, y);

        if (package == null)
        {
            return;
        }

        using var cmd = new MySqlCommand("UPDATE packages SET pin='@pin' WHERE id=@id", _connection);

        cmd.Parameters.AddWithValue("@id", package.Id);
        cmd.Parameters.AddWithValue("@pin", pin);

        _packages[(x, y)] = package with { Pin = pin };

        cmd.ExecuteNonQuery();
    }

    public void Reset()
    {
        _packages.Clear();

        using var cmd = new MySqlCommand("DELETE FROM packages", _connection);

        cmd.ExecuteNonQuery();
    }

    public void ResetFor(string phone)
    {
        Sync();

        var ownedPackages = _packages.ToList().Where(elt => elt.Value.Phone == phone).ToList();

        foreach (var (key, value) in ownedPackages)
        {
            var newPin = Paczkomat.Instance.GetNewPin();
            _packages[key] = value with { Pin = newPin };

            using var cmd = new MySqlCommand("UPDATE paczkomat.packages SET pin='@pin' WHERE id=@id;", _connection);

            cmd.Parameters.AddWithValue("@pin", newPin);
            cmd.Parameters.AddWithValue("@id", value.Id);

            cmd.ExecuteNonQuery();

            Paczkomat.Instance.SendNewPinMessage(value.Email, value.Pin);
        }
    }
}