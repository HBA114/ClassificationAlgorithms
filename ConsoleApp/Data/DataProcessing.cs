using ConsoleApp.Exceptions;

namespace ConsoleApp.Data;
public class DataProcessing
{
    private string _dataFilePath;

    public DataProcessing(string dataFilePath)
    {
        _dataFilePath = dataFilePath;
    }

    public async Task<Tuple<List<string>, List<string>>> SeperateTrainAndTest(float trainDataPercentile, string trainDataFilePath = "", string testDataFilePath = "")
    {

        if (trainDataPercentile < 0 || trainDataPercentile >= 1)
            throw new DataProcessingException("Train Data Percentile should be between 0 and 1. Example : 0.7");

        List<string> trainData = new List<string>();
        List<string> testData = new List<string>();

        List<string> data = await ReadData();
        List<string> classes = SeperateClasses(data);

        System.Console.WriteLine("TrainPercentile = " + trainDataPercentile.ToString());
        // TODO: Her bir sınıf için test ve eğitim verisinde çeşitliliğin sağlanması gerekir.

        int trainDataCounter = Int32.Parse((trainDataPercentile * 10).ToString());
        int testDataCounter = 10 - trainDataCounter;

        System.Console.WriteLine("TrainDataCounter : " + trainDataCounter);
        System.Console.WriteLine("TestDataCounter : " + testDataCounter);

        // Add Column Headers each data
        trainData.Add(data[0]);
        testData.Add(data[0]);
        for (int i = 1; i < data.Count();)
        {
            if (i + 10 < data.Count())
            {
                for (int j = i; j < i + trainDataCounter; j++)
                {
                    trainData.Add(data[j]);
                }
                i += trainDataCounter;
                for (int j = i; j < i + testDataCounter; j++)
                {
                    testData.Add(data[j]);
                }
                i += testDataCounter;
            }
            else
            {
                trainData.Add(data[i]);
                i++;
            }
        }

        if (trainDataFilePath != "")
        {
            await File.WriteAllLinesAsync(trainDataFilePath, trainData);
        }

        if (testDataFilePath != "")
        {
            await File.WriteAllLinesAsync(testDataFilePath, testData);
        }

        return new(trainData, testData);
    }

    public async Task<List<string>> ReadData()
    {
        string[] data = await File.ReadAllLinesAsync(_dataFilePath);
        List<string> rows = data.ToList();
        return rows;
    }

    public List<string> SeperateClasses(List<string> data)
    {
        List<string> classes = new List<string>();

        for (int i = 1; i < data.Count(); i++)
        {
            List<string> columns = data[i].Split(",").ToList();
            if (!classes.Contains(columns[columns.Count() - 1]))
                classes.Add(columns[columns.Count() - 1]);
        }

        return classes;
    }
}