namespace Paczkomat.views.service;

public class LockerPicker : BaseForm
{
    private readonly RedirectType _redirectTo;

    public LockerPicker(RedirectType redirect) : base("Wybierz skrytkę")
    {
        _redirectTo = redirect;

        var (x, y) = Paczkomat.Instance.Size;

        for (var j = 0; j < y; j++)
        {
            for (var i = 0; i < x; i++)
            {
                var button = new Button();
                button.Size = new Size(50, 50);
                button.Location = new Point(50 + i * 60, 50 + j * 60);

                var localI = i;
                var localJ = j;

                button.Enabled = GetEnabled(localJ, localI);

                button.Click += (_, _) => ButtonClick(localJ, localI);

                Controls.Add(button);
            }
        }

        {
            var goBackButton = new Button();
            goBackButton.Size = new Size(100, 50);
            goBackButton.Location = new Point(100, 50 + (y + 1) * 60);
            goBackButton.Text = "Anuluj";

            goBackButton.Click += (_, _) => SwitchTo(Paczkomat.Instance.GetMainView());

            Controls.Add(goBackButton);
        }
    }

    private void ButtonClick(int x, int y)
    {
        switch (_redirectTo)
        {
            case RedirectType.Open:
                SwitchTo(new OpenLocker(x, y));
                break;
            case RedirectType.Reset:
                SwitchTo(new ResetLocker(x, y));
                break;
            case RedirectType.ResetPin:
                SwitchTo(new ResetPin(x, y));
                break;
            default:
                throw new Exception("No way to do execute this line");
        }
    }

    bool GetEnabled(int x, int y) => _redirectTo switch
    {
        RedirectType.Open => true,
        RedirectType.Reset => Paczkomat.Instance.HasPackageAt(x, y),
        RedirectType.ResetPin => Paczkomat.Instance.HasPackageAt(x, y),
        _ => false
    };


    protected override void OnResize()
    {
    }
}

public enum RedirectType
{
    Open,
    Reset,
    ResetPin
}