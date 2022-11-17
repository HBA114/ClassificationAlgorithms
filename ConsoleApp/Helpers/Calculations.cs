using System.Runtime.InteropServices;

namespace ConsoleApp.Helpers;

public static class Calculations
{
    // OS specific double calculation
    public static double ParseToDouble(string value)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return double.Parse(value.Replace(".", ","));
        }
        else
        {
            return double.Parse(value);
        }
    }
}