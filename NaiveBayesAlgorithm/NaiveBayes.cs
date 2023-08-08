using Utils;

namespace NaiveBayesAlgorithm;

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
        int sumTrue = 0;
        for (int i = 1; i < testDataset.Count(); i++)
        {
            if (PredictBeanClass(testDataset[i]))
                sumTrue++;
        }

        System.Console.WriteLine("Naive Bayes True Prediction Count = " + sumTrue);
        System.Console.WriteLine("Naive Bayes Test Data Count = " + (testDataset.Count() - 1));
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

            string mean = Calculations.Mean(data.ToArray()).ToString().Replace(",", ".");
            string standardDeviation = Calculations.StandardDeviation(data.ToArray()).ToString().Replace(",", ".");
            
            string classData = "";
            classData += className + "," + mean + "," + standardDeviation;
            modelData.Add(classData);
        }

        if (_modelPath != "" || _modelPath != null)
        {
            await File.WriteAllLinesAsync(_modelPath!, modelData);
        }
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

                double mean, standardDeviation, beanVal;

                mean = Calculations.ParseToDouble(data[1]);
                standardDeviation = Calculations.ParseToDouble(data[2]);

                beanVal = Calculations.ParseToDouble(beanDataColumns[i]);


                double firstArea = standardDeviation * Math.Sqrt(2 * Math.PI);
                double secondArea = -Math.Pow((beanVal - mean) / standardDeviation, 2) / 2;

                double value = 1 / firstArea *
                            Math.Pow(Math.E, secondArea);
                pos.Add(data[0], value);
            }
            possibilities.Add(pos);
        }

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
