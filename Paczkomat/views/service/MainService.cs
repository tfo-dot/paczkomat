using Paczkomat.consts;

namespace Paczkomat.views.service;

public class MainService: BaseForm
{
    private readonly Label _welcomeLabel;
    private readonly Button _openLockerButton;
    private readonly Button _resetLockerButton;
    private readonly Button _resetPinButton;
    private readonly Button _resetWholeButton;
    private readonly Button _goBackButton;

    public MainService(int id) : base("Tryb serwisowy")
    {
        var servicer = Paczkomat.Instance.GetServicer(id);
        
        // Add a welcome label
        _welcomeLabel = new Label();
        _welcomeLabel.Text = $"Witaj {servicer.Name} {servicer.LastName}!";
        _welcomeLabel.Font = new Font("Arial", 20, FontStyle.Bold);
        _welcomeLabel.AutoSize = true;

        // Add a Track Package button
        _openLockerButton = new Button();
        _openLockerButton.Text = "Otwórz skrytkę";
        _openLockerButton.Size = new Size(150, 40);
        _openLockerButton.Click += (_, _) => LockerOperation(RedirectType.Open);
        
        _resetLockerButton = new Button();
        _resetLockerButton.Text = "Zresetuj skrytkę";
        _resetLockerButton.Size = new Size(150, 40);
        _resetLockerButton.Click += (_, _) => LockerOperation(RedirectType.Reset);
        
        _resetPinButton = new Button();
        _resetPinButton.Text = "Zresetuj pin skrytki";
        _resetPinButton.Size = new Size(150, 40);
        _resetPinButton.Click += (_, _) => LockerOperation(RedirectType.ResetPin);
        
        _resetWholeButton = new Button();
        _resetWholeButton.Text = "Zresetuj paczkomat";
        _resetWholeButton.Size = new Size(150, 40);
        _resetWholeButton.Click += (_, _) => ResetAll();
        
        // Add a Track Package button
        _goBackButton = new Button();
        _goBackButton.Text = "Opuść tryb";
        _goBackButton.Size = new Size(150, 40);
        _goBackButton.Click += (_, _) =>
        {
            Paczkomat.Instance.SwitchMode(new LockerMeta(ServiceMode.Normal, -1));
            SwitchTo(Paczkomat.Instance.GetMainView());
        };
        
        // Add the controls to the form
        Controls.Add(_welcomeLabel);
        Controls.Add(_openLockerButton);
        Controls.Add(_resetLockerButton);
        Controls.Add(_resetPinButton);
        Controls.Add(_resetWholeButton);
        Controls.Add(_goBackButton);

        CenterControls();
    }

    private void LockerOperation(RedirectType type) => SwitchTo(new LockerPicker(type));

    private static void ResetAll() => Paczkomat.Instance.Reset();

    private void CenterControls()
    {
        _welcomeLabel.Location = new Point((ClientSize.Width - _welcomeLabel.Width) / 2, ClientSize.Height / 6);
       CenterHorizontal(_openLockerButton, _welcomeLabel, 20);
       CenterHorizontal(_resetLockerButton, _openLockerButton, 20);
       CenterHorizontal(_resetPinButton, _resetLockerButton, 20);
       CenterHorizontal(_resetWholeButton, _resetPinButton, 20);
       CenterHorizontal(_goBackButton, _resetWholeButton, 20);
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}