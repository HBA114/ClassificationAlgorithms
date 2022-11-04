namespace ConsoleApp.NaiveBayes;
public class NaiveBayes
{
    private bool _saveModel;
    private string _modelPath;
    private List<string> modelData;
    public NaiveBayes(bool saveModel = false, string modelPath = "")
    {
        _saveModel = saveModel;
        _modelPath = modelPath;
        modelData = new List<string>();
    }

    public async Task TrainNaiveBayesModelAsync(List<string> trainData)
    {
        // her sınıf için ortalama ve standart sapma değerleri hesaplanır.
        // bir dosyaya burada karşılaşılan ger sınıf için gerekli veriler kaydedilir ve model kaydedilmiş olur.
        // modeli kaydetmek istenilmemesi durumunda String değişken içerisinde dosyada saklanacağı gibi saklanır.
        string className = "";
        List<List<double>> values = new List<List<double>>();
        for (int i = 1; i < trainData.Count(); i++)
        {
            List<string> columns = trainData[i].Split(",").ToList();
            if (columns[columns.Count() - 1] != className)
            {
                //! class changed make calculations and assign them to variable
                //! Barbunya gelince NullReference Döndürüyor!!
                await CalculateAndWriteFromValuesAsync(values, className);

                // clearing list and updating className variable
                className = columns[columns.Count() - 1];
                values.Clear();
            }
            else
            {
                List<double> columnValues = new List<double>();
                for (int j = 0; j < columns.Count() - 1; j++)
                {
                    columnValues.Add(double.Parse(columns[j]));
                }
                values.Add(columnValues);
            }
        }
    }

    public void TestNaiveBayes(List<string> testDataSet)
    {
        // her sınıf için hesaplanan veriler kullanılarak test verisinin sınıfı tahmin edilmeli.
        // her tahmin doğruluğu veya yanlışlığı belirlenerek listelenmeli ve sonuç olarak doğruluk ölçüsü Accuracy gösterilmeli.
        //TODO test datasetin her bir satırı için :
        //TODO      modelData verisindeki her sınıfın her sütunu için olan verilerinden 
        //TODO      en yakın olanları hangisi ise onu seç ve test verisi içindeki sınıf ismi
        //TODO      ile karşılaştırarak doğruluk oranı (accuracy) hesaplamasını yap!
    }

    private async Task CalculateAndWriteFromValuesAsync(List<List<double>> values, string className)
    {
        if (values.Count() == 0) return;
        List<double> data;
        int j = 0;
        for (int i = 0; i < values[0].Count(); i++)
        {
            data = new List<double>();
            for (j = 0; j < values.Count(); j++)
            {
                data.Add(values[j][i]);
            }
            string mean = Mean(data.ToArray()).ToString().Replace(",", ".");
            string standartDeviation = StandartDeviation(data.ToArray()).ToString().Replace(",", ".");

            string classData = "";
            classData += className + "," + mean + "," + standartDeviation;
            modelData.Add(classData);
        }

        if (_modelPath != "")
        {
            await File.WriteAllLinesAsync(_modelPath, modelData);
        }
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