using Microsoft.Data.Analysis;
using Microsoft.ML.Data;

namespace DoAnTotNghiep.TrainModels
{
    public class HouseData
    {
        [LoadColumn(0)]
        [ColumnName("district")]
        public string District { get; set; } = string.Empty;
        [LoadColumn(1)]
        [ColumnName("city")]
        public string City { get; set; } = string.Empty;
        [LoadColumn(2)]
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [LoadColumn(3)]
        [ColumnName("area")]
        public float Area { get; set; } = 0f;
        [LoadColumn(4)]
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [LoadColumn(5)]
        [ColumnName("price")]
        public float Price { get; set; } = 0f;
        [LoadColumn(6)]
        [ColumnName("facilities")]
        public string Facilities { get; set; } = string.Empty;
    }

    public class NewHouseData
    {
        public NewHouseData() { }
        public NewHouseData(DataFrame dataFrame, int row)
        {
            this.Address = (string) dataFrame["address"][row];
            this.Distance = (float)dataFrame["distance"][row];
            this.Rating = (float)dataFrame["rating"][row];
            this.Capacity = (float)dataFrame["capaciity"][row];
            this.Area = (float)dataFrame["area"][row];
            this.Price = (float)dataFrame["price"][row];
        }

        [LoadColumn(0)]
        [ColumnName("address")]
        public string Address { get; set; } = string.Empty;
        [LoadColumn(1)]
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [LoadColumn(2)]
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [LoadColumn(3)]
        [ColumnName("capaciity")]
        public float Capacity { get; set; } = 0f;
        [LoadColumn(4)]
        [ColumnName("area")]
        public float Area { get; set; } = 0f;
        [LoadColumn(5)]
        [ColumnName("price")]
        public float Price { get; set; } = 0;

        public string ObjectToString()
        {
            return this.Address + "," + this.Distance + "," + this.Rating + "," + this.Capacity + "," + this.Area + "," + this.Price;
        }
    }


    public class NewModelTrainInput
    {
        public NewModelTrainInput()
        {
        }

        public NewModelTrainInput(string Address, float Distance, float Rating, float Capacity, float Area)
        {
            this.Address = Address;
            this.Distance = Distance;
            this.Rating= Rating;
            this.Capacity = Capacity;
            this.Area = Area;
            this.Price = 0f;
        }
        [ColumnName("address")]
        public string Address { get; set; } = string.Empty;
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [ColumnName("capaciity")]
        public float Capacity { get; set; } = 0;
        [ColumnName("area")]
        public float Area { get; set; } = 0f;

        [ColumnName("price")]
        public float Price { get; set; } = 0f;
    }

    public class NewModelTrainCheck
    {
        public NewModelTrainCheck()
        {
        }
        [ColumnName("addressEncoded")]
        public float[] AddressEncoded { get; set; }
        [ColumnName("address")]
        public string Address { get; set; } = string.Empty;
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [ColumnName("capaciity")]
        public float Capacity { get; set; } = 0;
        [ColumnName("capacityEncoded")]
        public float[] CapacityEncoded { get; set; }
        [ColumnName("area")]
        public float Area { get; set; } = 0f;

        [ColumnName("price")]
        public float Price { get; set; } = 0f;
        [ColumnName("Label")]
        public float Label { get; set; }

        [ColumnName("Score")]
        public float Score { get; set; }
    }


    public class ModelTrainCheck
    {
        //FacilitiesEncoded
        //FacilitiesToken
        //CityEncoded
        //DistrictEncoded
        public ModelTrainCheck()
        {
        }
        [ColumnName("FacilitiesToken")]
        public float[] FacilitiesToken { get; set; }

        [ColumnName("DistrictEncoded")]
        public float[] DistrictEncoded { get; set; }
        [ColumnName("CityEncoded")]
        public float[] CityEncoded { get; set; }
        [ColumnName("district")]
        public string District { get; set; } = string.Empty;
        [ColumnName("city")]
        public string City { get; set; } = string.Empty;
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [ColumnName("area")]
        public float Area { get; set; } = 0f;
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [ColumnName("price")]
        public float Price { get; set; } = 0f;
        [ColumnName("facilities")]
        public string Facilities { get; set; } = string.Empty;
    }

    public class ModelTrainInput
    {
        public ModelTrainInput() { }
        public ModelTrainInput(string district, string city, float distance, float area, float rating, string facilities)
        {
            District = district;
            City = city;
            Distance = distance;
            Area = area;
            Rating = rating;
            Facilities = facilities;
            Price = 0;
        }
        [ColumnName("district")]
        public string District { get; set; } = string.Empty;
        [ColumnName("city")]
        public string City { get; set; } = string.Empty;
        [ColumnName("distance")]
        public float Distance { get; set; } = 0f;
        [ColumnName("area")]
        public float Area { get; set; } = 0f;
        [ColumnName("rating")]
        public float Rating { get; set; } = 0f;
        [ColumnName("price")]
        public float Price { get; set; } = 0f;
        [ColumnName("facilities")]
        public string Facilities { get; set; } = string.Empty;
    }

    public class ModelTrainOutput
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }
}