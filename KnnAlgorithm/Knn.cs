using Entities;

using Utils;

namespace KnnAlgorithm;

public class Knn
{
    public double TestKNN(List<string> trainDataset, List<string> testDataset, int K = 1, bool useWeights = false)
    {
        int sumOfTrueDecisions = 0;
        int testDataCount = testDataset.Count() - 1;

        int maxBarCount = 10;
        int barCount = (testDataCount / maxBarCount);

        Console.CursorVisible = false;
        var pos = Console.GetCursorPosition();

        try
        {
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top + 1);
        }
        catch (ArgumentOutOfRangeException)
        {
            System.Console.WriteLine();
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top - 1);
        }
        Console.ForegroundColor = ConsoleColor.Green;

        for (int i = 1; i < testDataCount; i++)
        {
            List<Neighbor> neighborList = new List<Neighbor>();
            List<string> testColumns = testDataset[i].Split(",").ToList();
            Dictionary<string, double> neighborFreq = new Dictionary<string, double>();
            string neighborName = "";
            for (int j = 1; j < trainDataset.Count() - 1; j++)
            {
                List<string> trainColumns = trainDataset[j].Split(",").ToList();
                neighborName = trainColumns[trainColumns.Count() - 1];
                double sumOfSquares = 0;
                for (int k = 0; k < testColumns.Count() - 1; k++)
                {
                    double testValue = Calculations.ParseToDouble(testColumns[k]);
                    double trainValue = Calculations.ParseToDouble(trainColumns[k]);
                    sumOfSquares += Math.Pow((trainValue - testValue), 2);
                }
                double value = Math.Sqrt(sumOfSquares);
                if (useWeights)
                    value = (double)1 / (double)Math.Pow(value, 2);
                neighborList.Add(new Neighbor(neighborName, value));
            }

            if (K > neighborList.Count())
                K = neighborList.Count() - 1;

            if (useWeights)
            {
                neighborList = neighborList.OrderByDescending(x => x.Value).ToList();
                for (int j = 0; j < K; j++)
                {
                    if (neighborFreq.ContainsKey(neighborList[j].Name))
                        neighborFreq[neighborList[j].Name] += neighborList[j].Value;
                    else
                        neighborFreq.Add(neighborList[j].Name, neighborList[j].Value);
                }
            }
            else
            {
                neighborList = neighborList.OrderBy(x => x.Value).ToList();
                for (int j = 0; j < K; j++)
                {
                    if (neighborFreq.ContainsKey(neighborList[j].Name))
                        neighborFreq[neighborList[j].Name] += 1;
                    else
                        neighborFreq.Add(neighborList[j].Name, 1);
                }
            }

            neighborFreq = neighborFreq.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string decision = neighborFreq.First().Key;

            if (decision.Equals(testColumns[testColumns.Count() - 1]))
            {
                sumOfTrueDecisions++;
            }

            // Progress Bar
            int spaceCount = -2;
            int currentBarCount = -1;
            for (int j = 1; j < i; j += barCount)
            {
                if (j == 1)
                    Console.Write("[");
                // Console.Write("\u2588");
                currentBarCount++;
                if (currentBarCount > 0)
                {
                    Console.Write("\u2588");
                }
                spaceCount++;
            }
            for (int j = 1; j < maxBarCount - spaceCount; j++)
            {
                Console.Write(" ");
            }
            if (i != 1)
            {
                Console.Write("] " + (i * 100) / (testDataCount - 1) + " / 100");
            }

            try
            {
                Console.SetCursorPosition(pos.Left, pos.Top);
            }
            catch (ArgumentOutOfRangeException)
            {
                System.Console.WriteLine();
                // pos = Console.GetCursorPosition();
                Console.SetCursorPosition(pos.Left, pos.Top - 1);
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.CursorVisible = true;

        System.Console.WriteLine("\nKNN True Prediction Count : " + sumOfTrueDecisions);
        System.Console.WriteLine("KNN Test Data Count : " + testDataCount);

        return (double)sumOfTrueDecisions / (double)testDataCount;
    }


    public double TestKNNArray(string[] trainDataset, string[] testDataset, int K = 1, bool useWeights = false)
    {
        int sumOfTrueDecisions = 0;
        int testDataCount = testDataset.Count() - 1;

        int maxBarCount = 10;
        int barCount = (testDataCount / maxBarCount);

        Console.CursorVisible = false;
        var pos = Console.GetCursorPosition();

        try
        {
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top + 1);
        }
        catch (ArgumentOutOfRangeException)
        {
            System.Console.WriteLine();
            Console.SetCursorPosition(Console.GetCursorPosition().Left + 1, Console.GetCursorPosition().Top - 1);
        }
        Console.ForegroundColor = ConsoleColor.Green;

        for (int i = 1; i < testDataCount; i++)
        {
            Neighbor[] neighborList = new Neighbor[100000];
            List<string> testColumns = testDataset[i].Split(",").ToList();
            Dictionary<string, double> neighborFreq = new Dictionary<string, double>();
            string neighborName = "";

            int neighborListIndex = 0;

            for (int j = 1; j < trainDataset.Count() - 1; j++)
            {
                List<string> trainColumns = trainDataset[j].Split(",").ToList();
                neighborName = trainColumns[trainColumns.Count() - 1];
                double sumOfSquares = 0;
                for (int k = 0; k < testColumns.Count() - 1; k++)
                {
                    double testValue = Calculations.ParseToDouble(testColumns[k]);
                    double trainValue = Calculations.ParseToDouble(trainColumns[k]);
                    sumOfSquares += Math.Pow((trainValue - testValue), 2);
                }
                double value = Math.Sqrt(sumOfSquares);
                if (useWeights)
                    value = (double)1 / (double)Math.Pow(value, 2);
                neighborList[neighborListIndex] = new Neighbor(neighborName, value);
                neighborListIndex++;
            }

            neighborList = neighborList.Where(x => x != null).ToArray();

            if (K > neighborList.Count())
                K = neighborList.Count() - 1;

            if (useWeights)
            {
                neighborList = neighborList.OrderByDescending(x => x.Value).ToArray();
                for (int j = 0; j < K; j++)
                {
                    if (neighborFreq.ContainsKey(neighborList[j].Name))
                        neighborFreq[neighborList[j].Name] += neighborList[j].Value;
                    else
                        neighborFreq.Add(neighborList[j].Name, neighborList[j].Value);
                }
            }
            else
            {
                neighborList = neighborList.OrderBy(x => x.Value).ToArray();
                for (int j = 0; j < K; j++)
                {
                    if (neighborFreq.ContainsKey(neighborList[j].Name))
                        neighborFreq[neighborList[j].Name] += 1;
                    else
                        neighborFreq.Add(neighborList[j].Name, 1);
                }
            }

            neighborFreq = neighborFreq.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);
            string decision = neighborFreq.First().Key;

            if (decision.Equals(testColumns[testColumns.Count() - 1]))
            {
                sumOfTrueDecisions++;
            }

            // Progress Bar
            int spaceCount = -2;
            int currentBarCount = -1;
            for (int j = 1; j < i; j += barCount)
            {
                if (j == 1)
                    Console.Write("[");
                // Console.Write("\u2588");
                currentBarCount++;
                if (currentBarCount > 0)
                {
                    Console.Write("\u2588");
                }
                spaceCount++;
            }
            for (int j = 1; j < maxBarCount - spaceCount; j++)
            {
                Console.Write(" ");
            }
            if (i != 1)
            {
                Console.Write("] " + (i * 100) / (testDataCount - 1) + " / 100");
            }

            try
            {
                Console.SetCursorPosition(pos.Left, pos.Top);
            }
            catch (ArgumentOutOfRangeException)
            {
                System.Console.WriteLine();
                // pos = Console.GetCursorPosition();
                Console.SetCursorPosition(pos.Left, pos.Top - 1);
            }
        }

        Console.ForegroundColor = ConsoleColor.White;
        Console.CursorVisible = true;

        System.Console.WriteLine("\nKNN True Prediction Count : " + sumOfTrueDecisions);
        System.Console.WriteLine("KNN Test Data Count : " + testDataCount);

        return (double)sumOfTrueDecisions / (double)testDataCount;
    }
}
