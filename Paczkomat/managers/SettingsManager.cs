namespace Paczkomat.managers;

public class SettingsManager
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

    #region DBSettings
    /// <summary>
    /// DB host
    /// </summary>
    private static string DbHost => "localhost";

    /// <summary>
    /// DB Database
    /// </summary>
    private static string DbDatabase => "paczkomat";
    
    /// <summary>
    /// DB User
    /// </summary>
    private static string DbUser => "root";
    
    
    /// <summary>
    /// DB Password
    /// </summary>
    private static string DbPassword => "root";

    /// <summary>
    /// DB Port
    /// </summary>
    private static string DbPort => "3306";

    public static string ConnectionString => $"Server={DbHost};Database={DbDatabase};User ID={DbUser};Password={DbPassword};Port={DbPort}";

    #endregion

    #region EmailSettings
    public static string EmailHost => "localhost";
    public static int EmailPort => 1025;

    #endregion
}