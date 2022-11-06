namespace ConsoleApp.KNN;

public class KNN
{
    List<string> trainedData;
    List<string> trainDataMean;
    public KNN()
    {
        trainDataMean = new List<string>();
        trainedData = new List<string>();
    }

    public async Task TrainKNN(List<string> trainDataset, string savePath = "")
    {
        // Sınıfların her sütununun ortalamasını alıp her sınıf için tek değer olacak şekilde veri hazırlanabilir.
        string className = trainDataset[1].Split(",").Last();
        string classData = "";
        string oldClassName = className;
        bool loop = true;
        int columnIndex = 0;
        int lastIndex = 1;
        while (loop)
        {
            List<double> values = new List<double>();
            for (int i = lastIndex; i < trainDataset.Count(); i++)
            {
                List<string> columns = trainDataset[i].Split(",").ToList();

                if (className.Equals(columns.Last()) && columnIndex < columns.Count() - 1)
                {
                    values.Add(double.Parse(columns[columnIndex].Replace(".", ",")));
                    if (i + 1 >= trainDataset.Count() - 1 && columnIndex >= columns.Count() - 1)
                    {
                        loop = false;
                        break;
                    }
                }
                if (columnIndex >= columns.Count() - 2 && i == trainDataset.Count() - 1)
                {
                    columnIndex = -1;
                    loop = false;
                    break;
                }
                if (columnIndex >= columns.Count() - 2 && !trainDataset[i + 1].Split(",").ToList().Last().Equals(className))
                {
                    columnIndex = -1;
                    className = trainDataset[i + 1].Split(",").ToList().Last();
                    lastIndex = i;
                    break;
                }
            }
            // calculate mean
            double mean = Mean(values.ToArray());
            string meanString = mean.ToString().Replace(",", ".");
            if (columnIndex != -1)
            {
                classData += meanString + ",";
            }
            else
            {
                classData += meanString + "," + oldClassName;
                oldClassName = className;
                trainedData.Add(classData);
                classData = "";
            }
            columnIndex++;
        }

        if (savePath != "")
        {
            await File.WriteAllLinesAsync(savePath, trainedData);
        }
    }

    public double TestKNN(List<string> trainDataset, List<string> testDataset, int K = 1, bool useWeights = false)
    {
        double sumOfTrueDecisions = 0;
        double testDataCount = 1000;
        for (int i = 1; i < testDataCount; i++)
        {
            List<Neighbour> neighbourList = new List<Neighbour>();
            List<string> testColumns = testDataset[i].Split(",").ToList();
            Dictionary<string, double> neighbourFreq = new Dictionary<string, double>();
            string neighborName = "";
            for (int j = 1; j < trainDataset.Count() - 1; j++)
            {
                List<string> trainColumns = trainDataset[j].Split(",").ToList();
                neighborName = trainColumns[trainColumns.Count() - 1];
                double sumOfSquares = 0;
                for (int k = 0; k < testColumns.Count() - 1; k++)
                {
                    double testValue = Double.Parse(testColumns[k].Replace(".", ","));
                    double trainValue = Double.Parse(trainColumns[k].Replace(".", ","));
                    sumOfSquares += Math.Pow((trainValue - testValue), 2);
                }
                double value = Math.Sqrt(sumOfSquares);
                if (useWeights)
                    value = (double)1 / (double)Math.Pow(value, 2);

                neighbourList.Add(new Neighbour(neighborName, value));
            }

            if (K > neighbourList.Count())
                K = neighbourList.Count() - 1;

            if (useWeights)
            {
                neighbourList = neighbourList.OrderByDescending(x => x.Value).ToList();
                for (int j = 0; j < K; j++)
                {
                    if (neighbourFreq.ContainsKey(neighbourList[j].Name))
                        neighbourFreq[neighbourList[j].Name] += neighbourList[j].Value;
                    else
                        neighbourFreq.Add(neighbourList[j].Name, neighbourList[j].Value);
                }
            }
            else
            {
                neighbourList = neighbourList.OrderBy(x => x.Value).ToList();
                for (int j = 0; j < K; j++)
                {
                    if (neighbourFreq.ContainsKey(neighbourList[j].Name))
                        neighbourFreq[neighbourList[j].Name] += 1;
                    else
                        neighbourFreq.Add(neighbourList[j].Name, 1);
                }
            }

            // Karar Verildi
            // for (int j = 0; j < K; j++)
            // {
            //     if (useWeights)
            //     {
            //         if (neighbourFreq.ContainsKey(neighbourList[j].Name))
            //         {
            //             neighbourFreq[neighbourList[j].Name] += neighbourList[j].Value;
            //         }
            //         else
            //         {
            //             neighbourFreq.Add(neighbourList[j].Name, neighbourList[j].Value);
            //         }
            //     }
            //     else
            //     {
            //         if (neighbourFreq.ContainsKey(neighbourList[j].Name))
            //         {
            //             neighbourFreq[neighbourList[j].Name] += 1;
            //         }
            //         else
            //         {
            //             neighbourFreq.Add(neighbourList[j].Name, 1);
            //         }
            //     }
            // }
            neighbourFreq = neighbourFreq.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string decision = neighbourFreq.First().Key;
            if (decision.Equals(testColumns[testColumns.Count() - 1]))
            {
                sumOfTrueDecisions++;
            }
            // System.Console.WriteLine();
        }
        System.Console.WriteLine("True Prediction Count : " + sumOfTrueDecisions);
        System.Console.WriteLine("Data Count : " + testDataCount);
        return (double)sumOfTrueDecisions / (double)testDataCount;
    }

    public double TestKNN(List<string> testDataset, int K = 1, bool useWeights = false)
    {
        double sumOfTrueDecisions = 0;
        double testDataCount = 1000;
        for (int i = 1; i < testDataCount; i++)
        {
            List<Neighbour> neighbourList = new List<Neighbour>();
            List<string> testColumns = testDataset[i].Split(",").ToList();
            Dictionary<string, double> neighbourFreq = new Dictionary<string, double>();
            string neighborName = "";
            for (int j = 1; j < trainedData.Count() - 1; j++)
            {
                List<string> trainColumns = trainedData[j].Split(",").ToList();
                neighborName = trainColumns[trainColumns.Count() - 1];
                double sumOfSquares = 0;
                for (int k = 0; k < testColumns.Count() - 1; k++)
                {
                    double testValue = Double.Parse(testColumns[k].Replace(".", ","));
                    double trainValue = Double.Parse(trainColumns[k].Replace(".", ","));
                    sumOfSquares += Math.Pow((trainValue - testValue), 2);
                }
                double value = Math.Sqrt(sumOfSquares);
                if (useWeights)
                    value = (double)1 / (double)Math.Pow(value, 2);
                neighbourList.Add(new Neighbour(neighborName, value));
                // System.Console.WriteLine();
            }

            if (useWeights)
                neighbourList = neighbourList.OrderByDescending(x => x.Value).ToList();
            else
                neighbourList = neighbourList.OrderBy(x => x.Value).ToList();

            if (K > neighbourList.Count())
                K = neighbourList.Count() - 1;

            // Karar Verildi
            for (int j = 0; j < K; j++)
            {
                if (useWeights)
                {
                    if (neighbourFreq.ContainsKey(neighbourList[j].Name))
                    {
                        neighbourFreq[neighbourList[j].Name] += neighbourList[j].Value;
                    }
                    else
                    {
                        neighbourFreq.Add(neighbourList[j].Name, neighbourList[j].Value);
                    }
                }
                else
                {
                    if (neighbourFreq.ContainsKey(neighbourList[j].Name))
                    {
                        neighbourFreq[neighbourList[j].Name] += 1;
                    }
                    else
                    {
                        neighbourFreq.Add(neighbourList[j].Name, 1);
                    }
                }
            }
            neighbourFreq = neighbourFreq.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string decision = neighbourFreq.First().Key;
            if (decision.Equals(testColumns[testColumns.Count() - 1]))
            {
                sumOfTrueDecisions++;
            }
            // System.Console.WriteLine();
        }
        System.Console.WriteLine();
        return (double)sumOfTrueDecisions / (double)testDataCount;
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
}

public class Neighbour
{
    public string Name;
    public double Value;
    public Neighbour(string Name, double Value)
    {
        this.Name = Name;
        this.Value = Value;
    }
}