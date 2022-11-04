namespace ConsoleApp.NaiveBayes;
public class NaiveBayes
{
    private bool _saveModel;
    private string _modelPath;
    private int classCount = -1;
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
            if (columns[columns.Count() - 1] != className || i == trainData.Count() - 1)
            {
                classCount++;
                //! class changed make calculations and assign them to variable
                //! Barbunya gelince NullReference Döndürüyor!!
                await CalculateAndWriteFromValuesAsync(values, className);

                // clearing list and updating className variable
                className = columns[columns.Count() - 1];
                values.Clear();
            }
            List<double> columnValues = new List<double>();
            for (int j = 0; j < columns.Count() - 1; j++)
            {
                columnValues.Add(double.Parse(columns[j].Replace(".", ",")));
            }
            values.Add(columnValues);
        }
    }

    public double TestNaiveBayesModel(List<string> testDataSet)
    {
        // her sınıf için hesaplanan veriler kullanılarak test verisinin sınıfı tahmin edilmeli.
        // her tahmin doğruluğu veya yanlışlığı belirlenerek listelenmeli ve sonuç olarak doğruluk ölçüsü Accuracy gösterilmeli.
        //TODO test datasetin her bir satırı için :
        //TODO      modelData verisindeki her sınıfın her sütunu için olan verilerinden 
        //TODO      en yakın olanları hangisi ise onu seç ve test verisi içindeki sınıf ismi
        //TODO      ile karşılaştırarak doğruluk oranı (accuracy) hesaplamasını yap!

        int sumTrue = 0;
        for (int i = 1; i < testDataSet.Count(); i++)
        {
            if (CalculateBeanClass(testDataSet[i]))
                sumTrue++;
        }

        System.Console.WriteLine("True Prediction Count = " + sumTrue);
        System.Console.WriteLine("Data Count = " + (testDataSet.Count() - 1));
        // testDataSet.Count() - 1 because first line is contains column names
        double accuracy = (double)sumTrue / (double)(testDataSet.Count() - 1);
        return accuracy;
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
        double mean = 0;
        int i = 0;

        while (i < values.Count())
        {
            mean += values[i] / (double)values.Count();
            // if (i > 0)
            i++;
        }

        return mean;
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

    private bool CalculateBeanClass(string beanData)
    {
        int dataCountForClass = modelData.Count() / classCount;
        List<string> beanDataColumns = beanData.Split(",").ToList();
        List<Dictionary<string, double>> possibilities = new List<Dictionary<string, double>>();
        for (int i = 0; i < dataCountForClass; i++)
        {
            //! Her Sınıf için aynı veri ve olasılık çıkıyor!
            Dictionary<string, double> pos = new Dictionary<string, double>();
            for (int j = i; j < modelData.Count(); j += dataCountForClass)
            {
                List<string> data = modelData[j].Split(",").ToList();
                double mean = double.Parse(data[1].Replace(".", ","));
                double standartDeviation = double.Parse(data[2].Replace(".", ","));

                //! Value her seferinde aynı çıkıyor
                double beanVal = double.Parse(beanDataColumns[i].Replace(".", ","));

                double firstArea = standartDeviation * Math.Sqrt(2 * Math.PI);
                double secondArea = -Math.Pow((beanVal - mean) / standartDeviation, 2) / 2;

                double value = 1 / firstArea *
                            Math.Pow(Math.E, secondArea);
                pos.Add(data[0], value);
            }
            possibilities.Add(pos);
        }

        // System.Console.WriteLine(possibilities);
        List<Dictionary<string, double>> percentiles = new List<Dictionary<string, double>>();
        foreach (var possibility in possibilities)
        {
            Dictionary<string, double> percentilePerPos = new Dictionary<string, double>();
            int count = 0;
            double sum = 0;
            double classPossibility = 0;
            foreach (var pos in possibility)
            {
                count++;
                sum += pos.Value;
            }
            foreach (var pos in possibility)
            {
                classPossibility = (pos.Value / sum);
                percentilePerPos.Add(pos.Key, classPossibility);
            }
            percentiles.Add(percentilePerPos);
        }

        Dictionary<string, double> finalPercentile = new Dictionary<string, double>();
        foreach (var percentile in percentiles)
        {
            foreach (var per in percentile)
            {
                if (!finalPercentile.ContainsKey(per.Key))
                    finalPercentile.Add(per.Key, per.Value / 16);
                else
                    finalPercentile[per.Key] += per.Value / 16;
            }
        }

        // System.Console.WriteLine(finalPercentile);
        double max = 0;
        string predictedClass = "";
        foreach (var percentile in finalPercentile)
        {
            if (percentile.Value > max)
            {
                max = percentile.Value;
                predictedClass = percentile.Key;
            }
        }

        // System.Console.WriteLine(predictedClass);

        return predictedClass.Equals(beanDataColumns[beanDataColumns.Count() - 1]);
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