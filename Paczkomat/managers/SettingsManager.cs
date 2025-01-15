namespace Paczkomat.managers;

public static class SettingsManager
{
    /// <summary>
    /// Paczkomat size, provided as COLxROW
    /// </summary>
    public static string Size => "3x4";

    /// <summary>
    /// Paczkomat code
    /// </summary>
    public static string Code => "LCH-01";

    /// <summary>
    /// Paczkomat address
    /// </summary>
    public static string Address => "Localhost";

    #region EmailSettings
    public static string EmailHost => "localhost";
    public static int EmailPort => 1025;

    #endregion
}