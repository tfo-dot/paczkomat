using Paczkomat.consts;

namespace Paczkomat.views.courier;

public class MainCourier : BaseForm
{
    private readonly Label _welcomeLabel;
    private readonly Button _newPackageButton;
    private readonly Button _reportProblemButton;
    private readonly Button _goBackButton;

    public MainCourier() : base("Tryb kuriera")
    {
        var courier = Paczkomat.Instance.Courier;

        // Add a welcome label
        _welcomeLabel = new Label();
        _welcomeLabel.Text = $"Witaj {courier.Name} {courier.LastName}!";
        _welcomeLabel.Font = new Font("Arial", 20, FontStyle.Bold);
        _welcomeLabel.AutoSize = true;
        _welcomeLabel.Location = new Point(180, 90);

        // Add a Track Package button
        _newPackageButton = new Button();
        _newPackageButton.Text = "Dodaj paczkę";
        _newPackageButton.Size = new Size(150, 40);
        _newPackageButton.Location = new Point(170, 230);
        _newPackageButton.Click += (_, _) => AddPackage();

        // Add a Track Package button
        _reportProblemButton = new Button();
        _reportProblemButton.Text = "Zgłoś problem!";
        _reportProblemButton.Size = new Size(150, 40);
        _reportProblemButton.Location = new Point(170, 230);
        _reportProblemButton.Click += (_, _) =>
        {
            Paczkomat.Instance.SendSupportMessage(courier.Name, courier.LastName);
            MessageBox.Show("Email do pomocy technicznej został wysłany!", "Uwaga!");
        };

        // Add a Track Package button
        _goBackButton = new Button();
        _goBackButton.Text = "Opuść tryb";
        _goBackButton.Size = new Size(150, 40);
        _goBackButton.Location = new Point(170, 300);
        _goBackButton.Click += (_, _) =>
        {
            Paczkomat.Instance.SwitchMode(ServiceMode.Normal);
            SwitchTo(Paczkomat.Instance.GetMainView());
        };

        // Add the controls to the form
        Controls.Add(_welcomeLabel);
        Controls.Add(_newPackageButton);
        Controls.Add(_reportProblemButton);
        Controls.Add(_goBackButton);

        CenterControls();
    }

    private void AddPackage()
    {
        if (!Paczkomat.Instance.HasEmptyLockers)
        {
            MessageBox.Show("Nie możesz dodać paczki\nPaczkomat jest pełny!", "Uwaga!");
            return;
        }

        SwitchTo(new PackageInput());
    }

    private void CenterControls()
    {
        _welcomeLabel.Location = new Point((ClientSize.Width - _welcomeLabel.Width) / 2, ClientSize.Height / 6);
        CenterHorizontal(_newPackageButton, _welcomeLabel, 40);
        CenterHorizontal(_reportProblemButton, _newPackageButton, 40);
        CenterHorizontal(_goBackButton, _reportProblemButton, 40);
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}