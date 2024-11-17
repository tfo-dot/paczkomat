using MySql.Data.MySqlClient;

namespace Paczkomat;

public class PinManager
{
    private MySqlConnection? _connection;

    private List<PinMeta> _pins = [];

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
        using var packageCmd = new MySqlCommand("select * from pins", _connection);

        var result = packageCmd.ExecuteReader();

        while (result.Read())
        {
            var val = result["value"].ToString();
            var id = Convert.ToInt32(result["id"]);

            _pins.Add(new PinMeta(val!, id));
        }

        result.Close();

        if (_pins.Count != 0)
        {
            var rnd = new Random();

            _pins = [.._pins.OrderBy(_ => rnd.Next())];

            return;
        }

        Console.WriteLine("Generating new pins...");
        GeneratePins();
    }

    private readonly char[] _validChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    private void GeneratePins()
    {
        List<string> pins = [.._validChars.Select(elt => elt.ToString())];

        for (var i = 1; i < 4; i++)
        {
            List<string> tempPins = [];
            foreach (var pin in pins)
            {
                tempPins.AddRange(_validChars.Select(chr => pin + chr));
            }

            pins = tempPins;
        }

        pins.RemoveAll(pin =>
        {
            //All digits are the same
            {
                var value = pin[1] == pin[0] && pin[2] == pin[0] && pin[3] == pin[0];

                if (value) return true;
            }

            //Is sequential aka 1234, 2345, 9876
            {
                var ascending = true;
                var descending = true;

                for (var i = 0; i < pin.Length - 1; i++)
                {
                    if (pin[i] + 1 != pin[i + 1])
                    {
                        ascending = false;
                    }

                    if (pin[i] - 1 != pin[i + 1])
                    {
                        descending = false;
                    }
                }

                if (ascending || descending) return true;
            }

            //Is palindrome
            {
                var value = pin[0] == pin[3] && pin[1] == pin[2];
                if (value) return true;
            }

            return false;
        });

        var temp = string.Join(",", pins.Select(elt => $"('{elt}')"));

        {
            //No error here, just Rider moment
            using var myCmd = new MySqlCommand("INSERT INTO pins (value) VALUES" + temp, _connection);

            myCmd.ExecuteNonQuery();
        }

        {
            using var packageCmd = new MySqlCommand("select * from pins", _connection);

            var result = packageCmd.ExecuteReader();

            while (result.Read())
            {
                var val = result["value"].ToString();
                var id = Convert.ToInt32(result["id"]);

                _pins.Add(new PinMeta(val!, id));
            }

            var rnd = new Random();

            _pins = [.._pins.OrderBy(_ => rnd.Next())];
        }
    }

    public PinMeta GetNext()
    {
        var value = _pins[0];

        _pins.RemoveAt(0);

        using var cmd = new MySqlCommand("delete from pins where id = @id", _connection);

        cmd.Parameters.AddWithValue("@id", value.Id);

        cmd.ExecuteNonQuery();

        return value;
    }

    public void Reset()
    {
        _pins.Clear();
        
        using var cmd = new MySqlCommand("DELETE FROM pins", _connection);

        cmd.ExecuteNonQuery();
        
        GeneratePins();
    }
}

public record PinMeta(string Value, int Id);