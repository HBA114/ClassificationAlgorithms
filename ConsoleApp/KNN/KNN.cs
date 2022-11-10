using ConsoleApp.Entities;

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

    public double TestKNN(List<string> trainDataset, List<string> testDataset, int K = 1, bool useWeights = false)
    {
        double sumOfTrueDecisions = 0;
        double testDataCount = testDataset.Count() - 1;
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

            neighbourFreq = neighbourFreq.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string decision = neighbourFreq.First().Key;
            
            if (decision.Equals(testColumns[testColumns.Count() - 1]))
            {
                sumOfTrueDecisions++;
            }
            // System.Console.WriteLine();
        }
        System.Console.WriteLine("True Prediction Count : " + sumOfTrueDecisions);
        System.Console.WriteLine("Test Data Count : " + testDataCount);
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