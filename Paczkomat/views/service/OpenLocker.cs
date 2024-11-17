namespace Paczkomat.views.service;

public class OpenLocker : BaseForm
{
    private readonly Label _mainText;
    private readonly Button _receiveButton;
    
    public OpenLocker(int x, int y) : base("Skrytka otwarta")
    {
        // Add a welcome label
        _mainText = new Label();
        _mainText.Text = $"Otworzona skrytka jest {x+1}x{y+1}!";
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

    private void CloseLocker() => SwitchTo(Paczkomat.Instance.GetMainView());

    private void CenterControls()
    {
        _mainText.Location = new Point(( ClientSize.Width - _mainText.Width) / 2, ClientSize.Height / 6);
        _receiveButton.Location = new Point(( ClientSize.Width - _receiveButton.Width) / 2, _mainText.Bottom + 30);
    }

    protected override void OnResize() => CenterControls();
}