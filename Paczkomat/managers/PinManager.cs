namespace Paczkomat.managers;

public class PinManager
{
    private List<string> _pins = [];

    private readonly char[] _validChars = ['0', '1', '2', '3', '4', '5', '6', '7', '8', '9'];

    public PinManager()
    {
        if (_pins.Count == 0)
        {
            GeneratePins();
        }
    }

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

        var rnd = new Random();

        _pins = [..pins.OrderBy(_ => rnd.Next())];
    }

    public string GetNext()
    {
        var value = _pins[0];

        _pins.RemoveAt(0);

        return value;
    }

    public void Reset()
    {
        _pins.Clear();

        GeneratePins();
    }
}