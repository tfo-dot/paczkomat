namespace Paczkomat.views;

public class BaseForm : Form
{
    protected BaseForm(string title)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Text = title;
        Size = new Size(600, 480);
        Closed += (_, _) =>
        {
            Paczkomat.Instance.Shutdown();
            Application.Exit();
        };
        Resize += (_, _) => OnResize();
    }

    protected void SwitchTo(Form form)
    {
        Hide();

        form.Size = Size;
        form.StartPosition = FormStartPosition.Manual;
        form.Location = Location;
        
        form.Show();
    }

    protected virtual void OnResize()
    {
        throw new NotImplementedException();
    }

    protected void CenterHorizontal(Control control, Control? basedOn = null, int yOffset = 0)
    {
        control.Location = new Point((ClientSize.Width - control.Width)/ 2, basedOn?.Bottom + yOffset ?? 0);
    }
}