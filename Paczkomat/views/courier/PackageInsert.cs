using Paczkomat.consts;

namespace Paczkomat.views.courier;

public class PackageInsert : BaseForm
{
    private readonly Label _mainText;
    private readonly Button _receiveButton;

    private readonly Package _package;

    public PackageInsert(int id) : base("Wsadź paczkę do skrytki")
    {
        _package = Paczkomat.Instance.GetPackage(id);

        // Add a welcome label
        _mainText = new Label();
        _mainText.Text = $"Skrytka otwarta! Kolumna {_package.Column+1}, rząd {_package.Row+1}!";
        _mainText.Font = new Font("Arial", 20, FontStyle.Bold);
        _mainText.AutoSize = true;
        _mainText.Location = new Point(180, 90);

        // Add a Track Package button
        _receiveButton = new Button();
        _receiveButton.Text = "Skrytka zamknięta!";
        _receiveButton.Size = new Size(150, 40);
        _receiveButton.Location = new Point(170, 230);
        _receiveButton.Click += (_, _) => CloseLocker();

        Controls.Add(_mainText);
        Controls.Add(_receiveButton);

        CenterControls();
    }

    private void CenterControls()
    {
        _mainText.Location = new Point((ClientSize.Width - _mainText.Width) / 2, ClientSize.Height / 6);
        _receiveButton.Location = new Point((ClientSize.Width - _receiveButton.Width) / 2, _mainText.Bottom + 30);
    }

    protected override void OnResize()
    {
        CenterControls();
    }

    private void CloseLocker()
    {
        Paczkomat.Instance.RemovePackage(_package.Id);

        SwitchTo(Paczkomat.Instance.GetMainView());
    }
}