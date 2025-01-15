namespace Paczkomat.views.service;

public class ResetPin: BaseForm
{
    private readonly Label _mainText;
    private readonly Button _receiveButton;
    private readonly Button _goBackButton;
    
    public ResetPin(int x, int y) : base("Zresetuj pin skrytki")
    {
        // Add a welcome label
        _mainText = new Label();
        _mainText.Text = $"Chcesz zresetować pin skrytki na {x+1} x {y+1}!";
        _mainText.Font = new Font("Arial", 20, FontStyle.Bold);
        _mainText.AutoSize = true;
        _mainText.Location = new Point(180, 90);
        
        // Add a Track Package button
        _receiveButton = new Button();
        _receiveButton.Text = "Potwierdzam!";
        _receiveButton.Size = new Size(150, 40);
        _receiveButton.Location = new Point(170, 230);
        _receiveButton.Click += (_, _) => CloseLocker(x, y);
        
        // Add a Track Package button
        _goBackButton = new Button();
        _goBackButton.Text = "Anuluj";
        _goBackButton.Size = new Size(150, 40);
        _goBackButton.Location = new Point(170, 230);
        _goBackButton.Click += (_, _) => GoBack();
        
        Controls.Add(_mainText);
        Controls.Add(_receiveButton);
        Controls.Add(_goBackButton);
        
        CenterControls();
    }

    private void CloseLocker(int x, int y)
    {
        Paczkomat.Instance.ResetPin(x, y);
        SwitchTo(Paczkomat.Instance.GetMainView());
    }

    private void GoBack()
    {
        SwitchTo(Paczkomat.Instance.GetMainView());
    }
    
    private void CenterControls()
    {
        _mainText.Location = new Point(( ClientSize.Width - _mainText.Width) / 2, ClientSize.Height / 6);
        _receiveButton.Location = new Point(( ClientSize.Width - _receiveButton.Width) / 2, _mainText.Bottom + 30);
        _goBackButton.Location = new Point((ClientSize.Width - _goBackButton.Width) / 2, _receiveButton.Bottom + 30);
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}