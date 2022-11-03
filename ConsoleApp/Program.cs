using ConsoleApp.Data;
using ConsoleApp.NaiveBayes;

string filePath = "ConsoleApp/Data/dry_bean_dataset.csv";
// string trainDataPath = "ConsoleApp/Data/dry_bean_dataset_train.csv";
// string testDataPath = "ConsoleApp/Data/dry_bean_dataset_test.csv";

DataProcessing dataProcessing = new DataProcessing(filePath);

// if you do not want to save train and test data:
// Use SeperateTrainAndTest method with only 1 argument trainDataPercentile

// Tuple<List<string>, List<string>> seperatedData = await dataProcessing.SeperateTrainAndTest(0.7f, trainDataPath, testDataPath);
// List<string> trainData = seperatedData.Item1;
// List<string> testData = seperatedData.Item2;

// System.Console.WriteLine("Train Data Count : " + trainData.Count());
// System.Console.WriteLine("Test Data Count : " + testData.Count());

// TODO: Create NaiveBayes and KNN Algorithms And Test Them

// if you want to save the NaiveBayes model give true as Constructor Parameter as "saveModel: true"
// if you already saved a model give saved model's path as path
NaiveBayes naiveBayes = new NaiveBayes();

System.Console.WriteLine(naiveBayes.Mean(new double[] { 27, 16, 5 }));
System.Console.WriteLine(naiveBayes.Mean(new double[] { 25, 30, 8 }));

System.Console.WriteLine(naiveBayes.StandartDeviation(new double[] { 27, 16, 5 }));
System.Console.WriteLine(naiveBayes.StandartDeviation(new double[] { 25, 30, 8 }));

// naiveBayes.CalculateYesNoSample(23);

// naiveBayes.TrainNaiveBayesModel(trainData);
// naiveBayes.TestNaiveBayes(testData);