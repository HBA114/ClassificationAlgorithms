namespace ConsoleApp.NaiveBayes;
public class NaiveBayes
{
    public NaiveBayes()
    {
        
    }

    public void TrainNaiveBayesModel(List<string> trainData)
    {
        // her sınıf için ortalama ve standart sapma değerleri hesaplanır.
    }

    public void TestNaiveBayes(List<string> testDataSet)
    {
        // her sınıf için hesaplanan veriler kullanılarak test verisinin sınıfı tahmin edilmeli.
        // her tahmin doğruluğu veya yanlışlığı belirlenerek listelenmeli ve sonuç olarak doğruluk ölçüsü Accuracy gösterilmeli.
    }

    public double Mean(double[] values)
    {
        double sum = 0;
        foreach (double number in values)
        {
            sum += number;
        }

        return sum / values.Count();
    }

    public double StandartDeviation(double[] values)
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

    public void CalculateYesNoSample(double temp)
    {
        double yesMean = Mean(new double[] { 27, 16, 5 });
        double noMean = Mean(new double[] { 25, 30, 8 });
        double yesStandartDeviation = StandartDeviation(new double[] { 27, 16, 5 });
        double noStandartDeviation = StandartDeviation(new double[] { 25, 30, 8 });

        double yes = 1 / (yesStandartDeviation * Math.Sqrt(2 * Math.PI)) * Math.Pow(Math.E, -1 / 2 * Math.Pow((temp - yesMean / yesStandartDeviation), 2));
        double no = 1 / (noStandartDeviation * Math.Sqrt(2 * Math.PI)) * Math.Pow(Math.E, -1 / 2 * Math.Pow((temp - noMean / noStandartDeviation), 2));

        System.Console.WriteLine(yes / (yes + no));
        System.Console.WriteLine(no / (yes + no));
    }
}