using ConsoleApp.Data;
using ConsoleApp.NaiveBayes;

string datasetCSVFilePath = "Data/dry_bean_dataset.csv";
string trainDataSavePath = "Data/dry_bean_dataset_train.csv";
string testDataSavePath = "Data/dry_bean_dataset_test.csv";

string naiveBayesModelSavePath = "Data/Models/NaiveBayesModel.csv";

//! Paths are changing on debuging and running from console
System.Console.WriteLine(Environment.CurrentDirectory);
string pathVariable = "";
if (!Environment.CurrentDirectory.Contains("ConsoleApp"))
{
    pathVariable = "ConsoleApp";
}
string filePath = Path.Combine(Environment.CurrentDirectory, pathVariable, datasetCSVFilePath);
string trainDataPath = Path.Combine(Environment.CurrentDirectory, pathVariable, trainDataSavePath);
string testDataPath = Path.Combine(Environment.CurrentDirectory, pathVariable, testDataSavePath);
string naiveBayesModelPath = Path.Combine(Environment.CurrentDirectory, pathVariable, naiveBayesModelSavePath);


DataProcessing dataProcessing = new DataProcessing(filePath);

// if you do not want to save train and test data:
// Use SeperateTrainAndTest method with only 1 argument trainDataPercentile

Tuple<List<string>, List<string>> seperatedData = await dataProcessing.SeperateTrainAndTest(0.7f, trainDataPath, testDataPath);
List<string> trainData = seperatedData.Item1;
List<string> testData = seperatedData.Item2;

// System.Console.WriteLine("Train Data Count : " + trainData.Count());
// System.Console.WriteLine("Test Data Count : " + testData.Count());

// TODO: Create NaiveBayes and KNN Algorithms And Test Them

// if you want to save the NaiveBayes model give true as Constructor Parameter as "saveModel: true"
// if you already saved a model give saved model's path as path

NaiveBayes naiveBayes = new NaiveBayes(saveModel: true, modelPath: naiveBayesModelPath);

// var allData = await File.ReadAllLinesAsync(filePath);
// List<string> allDataList = allData.ToList();

await naiveBayes.TrainNaiveBayesModelAsync(trainData: trainData);

double naiveBayesTestResult = naiveBayes.TestNaiveBayesModel(testDataSet: testData);
System.Console.WriteLine("Accuracy : " + Math.Round(naiveBayesTestResult, 2));
System.Console.WriteLine("Accuracy : " + Math.Round(naiveBayesTestResult, 2) * 100 + "%");