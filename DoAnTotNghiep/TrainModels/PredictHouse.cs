using DoAnTotNghiep.Entity;
using Microsoft.ML;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;
using Microsoft.Data.Analysis;
using System.Net.WebSockets;
using System.Formats.Asn1;
using System.Text;
using System.Globalization;
using System.Data;
using System;
using System.Reflection;
using Microsoft.ML.Data;
using Microsoft.ML.Trainers;
using NuGet.Protocol;
using Microsoft.ML.Transforms;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using NuGet.Packaging;
using System.ComponentModel;
using System.Collections.Immutable;
using System.Collections.Generic;

namespace DoAnTotNghiep.TrainModels
{
    public class PredictHouse
    {
        private MLContext _mLContext;
        private ITransformer trainedModel;
        private DataViewSchema modelSchema;

        private string _modelPath = @"TrainModels/PredictHouse.zip";
        private string _outPath = @"TrainModels/out_clear_data.csv";
        private string _tempModelPath = @"TrainModels/TempPredictHouse.zip";


        public PredictHouse()
        {
            this._mLContext = new MLContext(seed: 0);
            this.trainedModel = this._mLContext.Model.Load(this._modelPath, out this.modelSchema);
        }
        public void LoadAndMakeNewFile()
        {
            string[] filePaths = Directory.GetFiles(@"TrainModels/dataNew", "*.csv");
            string outputPath = @"TrainModels/newData_out_put.csv";

            if (filePaths.Length > 0)
            {
                var headerLine = System.IO.File.ReadLines(filePaths[0]).First();
                System.IO.File.WriteAllText(outputPath, headerLine + Environment.NewLine);

                foreach (var filePath in filePaths)
                {
                    var lines = System.IO.File.ReadAllLines(filePath).Skip(1);
                    System.IO.File.AppendAllLines(outputPath, lines);
                }
            }
        }
        public void PrepareData()
        {
            var dataFrame = DataFrame.LoadCsv(@"TrainModels/newData_out_put.csv");
            Console.WriteLine(dataFrame.Info());
            string pattern = @"[^0-9]+";
            for (int x = 0; x < dataFrame.Rows.Count; x++)
            {
                if (dataFrame.Rows[x] != null)
                {
                    if (!string.IsNullOrEmpty(dataFrame["address"][x].ToString()))
                    {
                        string address = dataFrame["address"][x].ToString().Replace(" ", "").ToLower();
                        if (address.Contains("hanoi"))
                        {
                            dataFrame["address"][x] = "hanoi";
                        }
                        else if (address.Contains("hochiminh"))
                        {
                            dataFrame["address"][x] = "hochiminh";
                        }
                        else if (address.Contains("danang"))
                        {
                            dataFrame["address"][x] = "danang";
                        }
                        else if (address.Contains("dalat"))
                        {
                            dataFrame["address"][x] = "dalat";
                        }
                        else if (address.Contains("cantho"))
                        {
                            dataFrame["address"][x] = "cantho";
                        }
                        else if (address.Contains("haiphong"))
                        {
                            dataFrame["address"][x] = "haiphong";
                        }
                        else if (address.Contains("halong"))
                        {
                            dataFrame["address"][x] = "halong";
                        }
                    }
                    //address,distance,distance_type,rating,capaciity,area,price
                    if (!string.IsNullOrEmpty(dataFrame["distance"][x].ToString()) 
                        && dataFrame["distance_type"][x] != null)
                    {
                        if (dataFrame["distance_type"][x].ToString() == "km")
                        {
                            double value = -1;
                            double.TryParse(dataFrame["distance"][x].ToString(), out value);
                            if(value != -1) dataFrame["distance"][x] = (float) (value * 1000);
                        }
                    }

                    if (dataFrame["rating"][x] != null)
                    {
                        double value = -1;
                        double.TryParse(dataFrame["rating"][x].ToString(), out value);
                        if (value != -1) dataFrame["rating"][x] = (float) (value * 10);
                    }

                    if (dataFrame["capaciity"][x] != null)
                    {
                        dataFrame["capaciity"][x] = float.Parse(Regex.Replace(dataFrame["capaciity"][x].ToString(), pattern, "").Trim());
                    }
                    if (dataFrame["area"][x] != null)
                    {
                        float value = float.Parse(Regex.Replace(dataFrame["area"][x].ToString(), pattern, "").Trim());
                        if (value == 0) dataFrame["area"][x] = null;
                        else dataFrame["area"][x] = value;
                    }
                    if (dataFrame["price"][x] != null)
                    {
                        dataFrame["price"][x] = float.Parse(Regex.Replace(dataFrame["price"][x].ToString(), pattern, "").Trim());
                    }
                }
            }

            DataFrame.SaveCsv(dataFrame.DropNulls(), @"TrainModels/newData_out_put.csv");
        }
        public void ClearData(string filePath, string outPath)
        {
            var dataFrame = DataFrame.LoadCsv(filePath).DropNulls();
            Console.WriteLine(dataFrame.Info());
            List<string> check = new List<string>() { "#VALUE!", "\"\"hotel_name" };
            for (int x = 0; x < dataFrame.DropNulls().Rows.Count; x++)
            {
                if (dataFrame.Rows[x] != null && dataFrame.Rows[x].Any(item => check.Contains(item.ToString())))
                {
                    dataFrame[x, 0] = null;
                }
            }

            DataFrame.SaveCsv(dataFrame.DropNulls(), outPath);
            Console.WriteLine(dataFrame.DropNulls().Info());
        }
        public List<List<NewHouseData>> GetList(string filePath)
        {
            var dataFrame = DataFrame.LoadCsv(filePath).DropNulls();
            var train = new List<NewHouseData>();
            var test = new List<NewHouseData>();
            var temp = new List<List<NewHouseData>>();
            string city = "";

            for(var row = 0; row < dataFrame.Rows.Count; row++)
            {
                if (dataFrame.Rows[row] != null)
                {
                    NewHouseData newHouseData = new NewHouseData(dataFrame, row);
                    if(newHouseData.Address != city)
                    {
                        List<NewHouseData> Node = new List<NewHouseData>();
                        Node.Add(newHouseData);
                        temp.Add(Node);
                        city = newHouseData.Address;
                    }
                    else
                    {
                        temp.Last().Add(newHouseData);
                    }
                }
            }

            foreach(var item in temp)
            {
                int numtest = (int) Math.Max(1, Math.Ceiling((double)item.Count() * 0.2));
                test.AddRange(item.GetRange(0, numtest));
                int numtrain = (item.Count() - numtest);
                numtrain = numtrain < 0? 0 : numtrain;
                train.AddRange(item.GetRange(numtest, numtrain));
            }


            Console.WriteLine("Train------------" + train.Count());
            foreach(var item in train)
            {
                Console.WriteLine(item.ToJson());
            }

            Console.WriteLine("Test------------" + test.Count());
            foreach (var item in test)
            {
                Console.WriteLine(item.ToJson());
            }

            WriteCsv(train, @"TrainModels/train_data.csv");
            WriteCsv(train, @"TrainModels/test_data.csv");

            return new List<List<NewHouseData>>() { train, test};
        }
        public static void WriteCsv(List<NewHouseData> dt, string path)
        {
            string head = "address,distance,rating,capacity,area,price";
            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(head);
                foreach (var item in dt)
                {
                    writer.WriteLine(item.ObjectToString());
                }
            }
        }

        public static DataTable BuildDataTable<T>(IList<T> lst)
        {
            //create DataTable Structure
            DataTable tbl = CreateTable<T>();
            Type entType = typeof(T);
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            //get the list item and add into the list
            foreach (T item in lst)
            {
                DataRow row = tbl.NewRow();
                foreach (PropertyDescriptor prop in properties)
                {
                    row[prop.Name] = prop.GetValue(item);
                }
                tbl.Rows.Add(row);
            }
            return tbl;
        }

        private static DataTable CreateTable<T>()
        {
            //T –> ClassName
            Type entType = typeof(T);
            //set the datatable name as class name
            DataTable tbl = new DataTable(entType.Name);
            //get the property list
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(entType);
            foreach (PropertyDescriptor prop in properties)
            {
                //add property as column
                tbl.Columns.Add(prop.Name, prop.PropertyType);
            }
            return tbl;
        }

        public ViewModelTrained NewTrainAndSave(string dataPath)
        {
            List<List<NewHouseData>> datas = this.GetList(dataPath);
            IDataView dataTrain = this._mLContext.Data.LoadFromTextFile<NewHouseData>(@"TrainModels/train_data.csv", hasHeader: true, separatorChar: ',');

            IDataView dataTest = this._mLContext.Data.LoadFromTextFile<NewHouseData>(@"TrainModels/test_data.csv", hasHeader: true, separatorChar: ',');

            var pipeline = this._mLContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "price")
                    .Append(this._mLContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "addressEncoded", inputColumnName: "address"))
                    .Append(this._mLContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "capacityEncoded", inputColumnName: "capaciity"))
                    .Append(this._mLContext.Transforms.Concatenate("Features", new string[] { "addressEncoded", "distance", "area", "rating", "capacityEncoded" }))
                    .Append(this._mLContext.Regression.Trainers.FastTree());

            this.trainedModel = pipeline.Fit(dataTrain);

            this.modelSchema = dataTrain.Schema;
            this._mLContext.Model.Save(this.trainedModel, this.modelSchema, @"TrainModels/PredictHouse.zip");

            var predictions = this.trainedModel.Transform(dataTest);
            var metrics = this._mLContext.Regression.Evaluate(predictions);

            System.IO.File.WriteAllLines(@"TrainModels/check.csv",this._mLContext.Data.CreateEnumerable<NewModelTrainCheck>(predictions, false).Select(t => t.ToJson()));

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

            return new ViewModelTrained(metrics.RSquared, metrics.RootMeanSquaredError);
        }

        private ViewModelTrained TrainAndSave(string dataPath, string outModel, float num)
        {
            //ClearData(dataPath, this._outPath);
            IDataView dataView = this._mLContext.Data.LoadFromTextFile<HouseData>(this._outPath, hasHeader: true, separatorChar: ',');
            DataOperationsCatalog.TrainTestData dataSplit = this._mLContext.Data.TrainTestSplit(dataView, testFraction: num);
            IDataView trainData = dataSplit.TrainSet;

            var pipeline = this._mLContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: "price")
                    .Append(this._mLContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "DistrictEncoded", inputColumnName: "district"))
                    .Append(this._mLContext.Transforms.Categorical.OneHotEncoding(outputColumnName: "CityEncoded", inputColumnName: "city"))
                    .Append(this._mLContext.Transforms.Text.FeaturizeText(outputColumnName: "FacilitiesToken", inputColumnName: "facilities"))
                    .Append(this._mLContext.Transforms.Concatenate("Features", new string[] { "DistrictEncoded", "CityEncoded", "distance", "area", "rating", "FacilitiesToken" }))
                    .Append(this._mLContext.Regression.Trainers.FastTree());

            this.trainedModel = pipeline.Fit(trainData);
            var overView = trainedModel.Transform(dataView);
            this.modelSchema = trainData.Schema;
            this._mLContext.Model.Save(this.trainedModel, this.modelSchema, @"TrainModels/PredictHouse.zip");
            //ToDataTable(overView);

            IDataView testData = dataSplit.TestSet;
            var predictions = this.trainedModel.Transform(testData);
            var metrics = this._mLContext.Regression.Evaluate(predictions);

            System.IO.File.WriteAllLines(@"TrainModels/check.csv", this._mLContext.Data.CreateEnumerable<ModelTrainCheck>(predictions, false).Select(t => t.ToJson()));

            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

            return new ViewModelTrained(metrics.RSquared, metrics.RootMeanSquaredError);
        }
        public float GetPrediction(ModelTrainInput input)
        {
            var predictionFunction = this._mLContext.Model.CreatePredictionEngine<ModelTrainInput, ModelTrainOutput>(this.trainedModel);
            var prediction = predictionFunction.Predict(input);
            return prediction.Price;
        }
        public ViewModelTrained ReTrain(string dataPath, int stt, float num = 0.2f)
        {
            if(this.trainedModel != null)
            {
                //return this.TrainAndSave(dataPath, this._tempModelPath, num);
                string path = @"TrainModels/TempPredictHouse_" + stt + ".zip";
                return this.TrainAndSave(dataPath, path, num);
            }
            else
            {
                return this.TrainAndSave(dataPath, this._modelPath, num);
            }
        }

        //get Param so sánh giữa 2 cái model

        public ViewModelTrained GetSquare()
        {
            IDataView dataView = this._mLContext.Data.LoadFromTextFile<HouseData>(this._outPath, hasHeader: true, separatorChar: ',');
            DataOperationsCatalog.TrainTestData dataSplit = this._mLContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
            IDataView trainData = dataSplit.TrainSet;
            IDataView testData = dataSplit.TestSet;
            var predictions = this.trainedModel.Transform(dataView);
            var metrics = this._mLContext.Regression.Evaluate(predictions);

            Console.WriteLine("=============== End of model evaluation ===============");
            Console.WriteLine();
            Console.WriteLine($"*************************************************");
            Console.WriteLine($"*       Model quality metrics evaluation         ");
            Console.WriteLine($"*------------------------------------------------");
            Console.WriteLine($"*       RSquared Score:      {metrics.RSquared:0.##}");
            Console.WriteLine($"*       Root Mean Squared Error:      {metrics.RootMeanSquaredError:#.##}");

            return new ViewModelTrained(metrics.RSquared, metrics.RootMeanSquaredError);
        }
        public float GetPredict(NewModelTrainInput input)
        {
            var predictionFunction = this._mLContext.Model.CreatePredictionEngine<NewModelTrainInput, ModelTrainOutput>(this.trainedModel);
            var prediction = predictionFunction.Predict(input);
            return prediction.Price;
        }
        private void ToDataTable(IDataView? dataView)
        {
            if(dataView != null)
            {
                var preview = dataView.Preview(10000);
                //dt.Columns.AddRange(preview.Schema.Select(x => new DataColumn(x.Name)).ToArray());
                foreach (var row in preview.RowView)
                {
                    foreach (var val in row.Values)
                    {
                        Console.Write(val.ToJson());
                    }
                    Console.WriteLine();
                }
            }
        }
    }

    public class ViewModelTrained
    {
        public ViewModelTrained(double squared, double meanError)
        {
            Squared = squared;
            MeanError = meanError;
        }
        public double Squared { get; set; }
        public double MeanError { get; set; }
    }

}