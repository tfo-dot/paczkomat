namespace Paczkomat;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    private static void Main()
    {
        Application.Run( Paczkomat.Instance.GetMainView());
    }
}