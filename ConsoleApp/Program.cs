using ConsoleApp.Data;
using ConsoleApp.KNN;
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


DataProcessing dataProcessing = new DataProcessing();

//! Warning: If you do not have train and test data files as csv comment next line which,
//! includes ReadTrainAndTestData function then uncomment the line includes SeperateTrainAndTest function
//! if you have train and data files:
Tuple<List<string>, List<string>> seperatedData = await dataProcessing.ReadTrainAndTestData(trainDataFilePath: trainDataPath, testDataFilePath: testDataPath);

//! if you do not want to save train and test data:
//! Use SeperateTrainAndTest method with only 1 argument trainDataPercentile
//! Warning: If you do not have train and test data files as csv uncomment next line
// Tuple<List<string>, List<string>> seperatedData = await dataProcessing.SeperateTrainAndTest(0.7f, trainDataPath, testDataPath);
List<string> trainData = seperatedData.Item1;
List<string> testData = seperatedData.Item2;

NaiveBayes naiveBayes = new NaiveBayes();

//! if you save the model file you can read model with:
//! if you do not have any model saved: comment next line and run train:
await naiveBayes.ReadModel(naiveBayesModelPath);

//! in case you have not saved Naive Bayes model then you should train algorithm with function below (uncomment next line for train)
// await naiveBayes.TrainNaiveBayesModelAsync(trainDataset: trainData, saveModel: true, modelPath: naiveBayesModelPath);

double naiveBayesTestResult = naiveBayes.TestNaiveBayesModel(testDataset: testData);
System.Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2));
System.Console.WriteLine("Naive Bayes Accuracy : " + Math.Round(naiveBayesTestResult, 2) * 100 + "%\n");

KNN knn = new KNN();

System.Console.WriteLine("KNN Calculation Could Take 5 Minutes(.net7.0) or More (9 Minutes with .net6.0) Depending to the Device.");
System.Console.WriteLine("Takes 5 Minutes(.net7.0), (9 Minutes with .net6.0)");
System.Console.WriteLine("KNN Accuracy Calculating ...");

double knnTestResult = knn.TestKNN(trainDataset: trainData, testDataset: testData, K: 5, useWeights: false);

System.Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2));
System.Console.WriteLine("KNN Accuracy : " + Math.Round(knnTestResult, 2) * 100 + "%");