using System.Runtime.InteropServices;
using ConsoleApp.Helpers;

namespace ConsoleApp.NaiveBayes;
public class NaiveBayes
{
    private string _modelPath = "";
    private int classCount = -1;
    private List<string> modelData;

    public NaiveBayes() => modelData = new List<string>();

    // ! Finish This
    public async Task ReadModel(string modelPath)
    {
        string[] modelDataRaw = await File.ReadAllLinesAsync(modelPath);
        this.modelData = modelDataRaw.ToList();
        this.classCount = 1;
        string className = modelData[0].Split(",")[0];
        for (int i = 0; i < modelData.Count(); i++)
        {
            if (modelData[i].Split(",")[0] != className)
            {
                className = modelData[i].Split(",")[0];
                this.classCount++;
            }
        }
    }

    public async Task TrainNaiveBayesModel(List<string> trainDataset, bool saveModel = false, string modelPath = "")
    {
        _modelPath = modelPath;
        // her sınıf için ortalama ve standart sapma değerleri hesaplanır.
        // bir dosyaya burada karşılaşılan ger sınıf için gerekli veriler kaydedilir ve model kaydedilmiş olur.
        // modeli kaydetmek istenilmemesi durumunda String değişken içerisinde dosyada saklanacağı gibi saklanır.
        string className = "";
        List<List<double>> values = new List<List<double>>();
        for (int i = 1; i < trainDataset.Count(); i++)
        {
            List<string> columns = trainDataset[i].Split(",").ToList();
            if (columns[columns.Count() - 1] != className || i == trainDataset.Count() - 1)
            {
                classCount++;
                //! class changed make calculations and assign them to variable
                await CalculateAndWriteFromValuesAsync(values, className);

                // clearing list and updating className variable
                className = columns[columns.Count() - 1];
                values.Clear();
            }
            List<double> columnValues = new List<double>();
            for (int j = 0; j < columns.Count() - 1; j++)
            {
                columnValues.Add(Calculations.ParseToDouble(columns[j]));
            }
            values.Add(columnValues);
        }
    }

    public double TestNaiveBayesModel(List<string> testDataset)
    {
        // her sınıf için hesaplanan veriler kullanılarak test verisinin sınıfı tahmin edilmeli.
        // her tahmin doğruluğu veya yanlışlığı belirlenerek listelenmeli ve sonuç olarak doğruluk ölçüsü Accuracy gösterilmeli.
        //TODO test datasetin her bir satırı için :
        //TODO      modelData verisindeki her sınıfın her sütunu için olan verilerinden 
        //TODO      en yakın olanları hangisi ise onu seç ve test verisi içindeki sınıf ismi
        //TODO      ile karşılaştırarak doğruluk oranı (accuracy) hesaplamasını yap!

        int sumTrue = 0;
        for (int i = 1; i < testDataset.Count(); i++)
        {
            if (PredictBeanClass(testDataset[i]))
                sumTrue++;
        }

        System.Console.WriteLine("Naive Bayes True Prediction Count = " + sumTrue);
        System.Console.WriteLine("Naive Bayes Test Data Count = " + (testDataset.Count() - 1));
        // testDataSet.Count() - 1 because first line is contains column names
        double accuracy = (double)sumTrue / (double)(testDataset.Count() - 1);
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

            #region Test in Linux
            //! class data should be like "BARBUNYA,69846.580543933,10174.440752337463"
            string mean = Mean(data.ToArray()).ToString().Replace(",", ".");
            string standartDeviation = StandartDeviation(data.ToArray()).ToString().Replace(",", ".");
            #endregion
            
            string classData = "";
            classData += className + "," + mean + "," + standartDeviation;
            modelData.Add(classData);
        }

        if (_modelPath != "" || _modelPath != null)
        {
            await File.WriteAllLinesAsync(_modelPath!, modelData);
        }
    }

    private double Mean(double[] values)
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

    private double StandartDeviation(double[] values)
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

    private bool PredictBeanClass(string beanData)
    {
        int dataCountForClass = modelData.Count() / classCount;
        List<string> beanDataColumns = beanData.Split(",").ToList();
        List<Dictionary<string, double>> possibilities = new List<Dictionary<string, double>>();
        for (int i = 0; i < dataCountForClass; i++)
        {
            Dictionary<string, double> pos = new Dictionary<string, double>();
            for (int j = i; j < modelData.Count(); j += dataCountForClass)
            {
                List<string> data = modelData[j].Split(",").ToList();

                double mean, standartDeviation, beanVal;

                mean = Calculations.ParseToDouble(data[1]);
                standartDeviation = Calculations.ParseToDouble(data[2]);

                beanVal = Calculations.ParseToDouble(beanDataColumns[i]);


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

        finalPercentile = finalPercentile.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

        string predictedClass = finalPercentile.First().Key;
        return predictedClass.Equals(beanDataColumns[beanDataColumns.Count() - 1]);
    }
}