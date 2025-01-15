using Paczkomat.consts;

namespace Paczkomat.views.customer;

public class PinInput : BaseForm
{
    private readonly TextBox _pinTextBox;
    private readonly TextBox _phoneTextBox;
    private readonly Button _submitButton;
    private readonly Label _pinLabel;
    private readonly Label _phoneLabel;
    private readonly Button _goBackButton;

    public PinInput():base("Wprowadzanie danych")
    {
        // Phone Label
        _phoneLabel = new Label();
        _phoneLabel.Text = "Wpisz numer telefonu:";
        _phoneLabel.Location = new Point(50, 50);
        _phoneLabel.AutoSize = true;

        // Phone TextBox
        _phoneTextBox = new TextBox();
        _phoneTextBox.Location = new Point(200, 50);
        
        // Pin Label
        _pinLabel = new Label();
        _pinLabel.Text = "Wpisz PIN (4 znaki):";
        _pinLabel.Location = new Point(50, 100);
        _pinLabel.AutoSize = true;

        // Pin TextBox
        _pinTextBox = new TextBox();
        _pinTextBox.Location = new Point(200, 100);
        _pinTextBox.MaxLength = 4; // Restrict input to 4 digits
        
        // Submit Button
        _submitButton = new Button();
        _submitButton.Text = "Otwórz";
        _submitButton.Location = new Point(150, 150);
        _submitButton.Click += SubmitButton_Click!;
        
        // Goback Button
        _goBackButton = new Button();
        _goBackButton.Text = "Wróć";
        _goBackButton.Location = new Point(150, 180);
        _goBackButton.Click += (_, _) => SwitchTo(new MainCustomer());

        // Add the controls to the form
        Controls.Add(_pinLabel);
        Controls.Add(_pinTextBox);
        Controls.Add(_phoneLabel);
        Controls.Add(_phoneTextBox);
        Controls.Add(_submitButton);
        Controls.Add(_goBackButton);
        
        CenterControls();
    }
    
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        var pin = _pinTextBox.Text;
        var phone = _phoneTextBox.Text;

        var result = Paczkomat.Instance.OpenLock(phone, pin);

        if (result == null)
        {
            MessageBox.Show("Nie znaleziono takiej paczki!", "Uwaga!");
            return;
        }

        if (result != ServiceMode.Normal)
        {
            Paczkomat.Instance.SwitchMode((ServiceMode)result);
            SwitchTo(Paczkomat.Instance.GetMainView());
        }
        else
        {
            SwitchTo(new BoxReceive());
        }
    }
    
    private void CenterControls()
    {
        {
            var sectionLeft = (ClientSize.Width - (_phoneLabel.Width + 30 + _phoneTextBox.Width)) / 2;
            _phoneLabel.Location = new Point(sectionLeft, ClientSize.Height / 6);
            _phoneTextBox.Location = new Point(sectionLeft + 30 + _phoneLabel.Width, ClientSize.Height / 6);
        }
        {
            var sectionLeft = (ClientSize.Width - (_phoneLabel.Width + 30 + _phoneTextBox.Width)) / 2;
            _pinLabel.Location = new Point(sectionLeft, _phoneLabel.Bottom + 30);
            _pinTextBox.Location = new Point(sectionLeft + 30 + _pinLabel.Width, _pinLabel.Top);
        }
        
        {
            var sectionLeft = (ClientSize.Width - (_goBackButton.Width + 30 + _submitButton.Width)) / 2;
            _goBackButton.Location = new Point(sectionLeft, _pinLabel.Bottom + 30);
            _submitButton.Location = new Point(sectionLeft + 30 + _goBackButton.Width, _goBackButton.Top);
        }
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}