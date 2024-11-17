using Paczkomat.views.customer;

namespace Paczkomat.views.courier;

public class PackageInput : BaseForm
{
    private readonly TextBox _phoneTextBox;
    private readonly TextBox _emailTextBox;
    private readonly Button _submitButton;
    private readonly Label _phoneLabel;
    private readonly Label _emailLabel;
    private readonly Button _goBackButton;

    public PackageInput():base("Wprowadzanie danych")
    {
        // Phone Label
        _phoneLabel = new Label();
        _phoneLabel.Text = "Wpisz numer telefonu:";
        _phoneLabel.Location = new Point(50, 50);
        _phoneLabel.AutoSize = true;

        // Phone TextBox
        _phoneTextBox = new TextBox();
        _phoneTextBox.Location = new Point(200, 50);
        
        _emailLabel = new Label();
        _emailLabel.Text = "Wpisz email:";
        _emailLabel.Location = new Point(50, 50);
        _emailLabel.AutoSize = true;

        // Phone TextBox
        _emailTextBox = new TextBox();
        _emailTextBox.Location = new Point(200, 150);
        
        // Submit Button
        _submitButton = new Button();
        _submitButton.Text = "Otwórz";
        _submitButton.Location = new Point(150, 200);
        _submitButton.Click += SubmitButton_Click!;
        
        // Goback Button
        _goBackButton = new Button();
        _goBackButton.Text = "Wróć";
        _goBackButton.Location = new Point(150, 230);
        _goBackButton.Click += (_, _) => SwitchTo(new MainCustomer());

        // Add the controls to the form
        Controls.Add(_phoneLabel);
        Controls.Add(_phoneTextBox);
        Controls.Add(_emailLabel);
        Controls.Add(_emailTextBox);
        Controls.Add(_submitButton);
        Controls.Add(_goBackButton);
        
        CenterControls();
    }
    
    private void SubmitButton_Click(object sender, EventArgs e)
    {
        var phone = _phoneTextBox.Text;
        var email = _emailTextBox.Text;

        var newPackage = Paczkomat.Instance.AddPackage(email, phone);
        
        SwitchTo(new PackageInsert(newPackage.Id));
    }
    
    private void CenterControls()
    {
        {
            var sectionLeft = (ClientSize.Width - (_phoneLabel.Width + 30 + _phoneTextBox.Width)) / 2;
            _phoneLabel.Location = new Point(sectionLeft, ClientSize.Height / 6);
            _phoneTextBox.Location = new Point(sectionLeft + 30 + _phoneLabel.Width, ClientSize.Height / 6);
        }
        
        {
            var sectionLeft = (ClientSize.Width - (_emailLabel.Width + 30 + _emailTextBox.Width)) / 2;
            _emailLabel.Location = new Point(sectionLeft, _phoneLabel.Bottom + 30);
            _emailTextBox.Location = new Point(sectionLeft + 30 + _emailLabel.Width, _emailLabel.Top);
        }
        
        {
            var sectionLeft = (ClientSize.Width - (_goBackButton.Width + 30 + _submitButton.Width)) / 2;
            _goBackButton.Location = new Point(sectionLeft, _emailLabel.Bottom + 30);
            _submitButton.Location = new Point(sectionLeft + 30 + _goBackButton.Width, _goBackButton.Top);
        }
    }

    protected override void OnResize()
    {
        CenterControls();
    }
}