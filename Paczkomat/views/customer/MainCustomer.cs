namespace Paczkomat.views.customer;

public class MainCustomer : BaseForm
{
    private readonly Label _welcomeLabel;
    private readonly Button _receiveButton;
    
    public MainCustomer(): base("Paczkomat")
    {
        // Add a welcome label
        _welcomeLabel = new Label();
        _welcomeLabel.Text = "Witaj!";
        _welcomeLabel.Font = new Font("Arial", 20, FontStyle.Bold);
        _welcomeLabel.AutoSize = true;
        _welcomeLabel.Location = new Point(180, 90);

        // Add a Track Package button
        _receiveButton = new Button();
        _receiveButton.Text = "Odbierz paczkę";
        _receiveButton.Size = new Size(150, 40);
        _receiveButton.Location = new Point(170, 230);
        _receiveButton.Click += (_, _) => SwitchTo(new PinInput());
        
        // Add the controls to the form
        Controls.Add(_welcomeLabel);
        Controls.Add(_receiveButton);
        
        CenterControls();
    }

    private void CenterControls()
    {
        _welcomeLabel.Location = new Point(( ClientSize.Width - _welcomeLabel.Width) / 2, ClientSize.Height / 6);
        _receiveButton.Location = new Point(( ClientSize.Width - _receiveButton.Width) / 2, _welcomeLabel.Bottom + 30);
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}