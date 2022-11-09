using ConsoleApp.Exceptions;

namespace ConsoleApp.Data;
public class DataProcessing
{
    private string? _dataFilePath;

    public DataProcessing() { }

    public DataProcessing(string dataFilePath)
    {
        _dataFilePath = dataFilePath;
    }

    public async Task<Tuple<List<string>, List<string>>> ReadTrainAndTestData(string trainDataFilePath, string testDataFilePath)
    {
        List<string> trainData = await ReadData(trainDataFilePath);
        List<string> testData = await ReadData(testDataFilePath);

        return new(trainData, testData);
    }

    public async Task<Tuple<List<string>, List<string>>> SeperateTrainAndTest(float trainDataPercentile = 0.7f, string trainDataFilePath = "", string testDataFilePath = "")
    {
        // TODO: Test Random!
        if (trainDataPercentile <= 0 || trainDataPercentile >= 1)
            throw new DataProcessingException("Train Data Percentile should be between 0 and 1. Example : 0.7");

        List<string> trainData = new List<string>();
        List<string> testData = new List<string>();

        List<string> data = await ReadData(_dataFilePath!);

        // TODO: Her bir sınıf için test ve eğitim verisinde çeşitliliğin sağlanması gerekir.

        int trainDataCounter = Int32.Parse((trainDataPercentile * 10).ToString());
        int testDataCounter = 10 - trainDataCounter;

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

    private async Task<List<string>> ReadData(string filePath)
    {
        string[] data = await File.ReadAllLinesAsync(filePath);
        List<string> rows = data.ToList();
        return rows;
    }
}