using System.Globalization;

namespace ConsoleApp.Helpers;

public static class Calculations
{
    // OS specific double calculation
    public static double ParseToDouble(string value)
    {
        //! Changed CultureInfo
        return double.Parse(value, CultureInfo.InvariantCulture);
        // if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        // {
        //     return double.Parse(value.Replace(".", ","));
        // }
        // else
        // {
        //     return double.Parse(value);
        // }
    }

    public static double Mean(double[] values)
    {
        double mean = 0;
        int i = 0;

        while (i < values.Count())
        {
            mean += values[i] / (double)values.Count();
            i++;
        }

        return mean;
    }

    public static double StandartDeviation(double[] values)
    {
        double sum = 0;
        double mean = Mean(values);
        int count = values.Count() - 1;

        foreach (double number in values)
        {
            sum += Math.Pow((number - mean), 2);
        }
        sum /= count;

        return Math.Sqrt(sum);
    }
}