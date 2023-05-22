using DoAnTotNghiep.Enum;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace DoAnTotNghiep.Modules
{
    public class RequestAPI
    {
        public static async Task<T?> Get<T>(Uri url)
        {
            string result = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Get, url);
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(result, typeof(T));
            return JsonConvert.DeserializeObject<T>(result);
        }

        public static async Task<T?> Post<T>(Uri url, FormUrlEncodedContent content)
        {
            string result = string.Empty;
            using (HttpClient client = new HttpClient())
            {
                var request = new HttpRequestMessage(HttpMethod.Post, url);
                request.Content = content;
                HttpResponseMessage response = await client.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    result = await response.Content.ReadAsStringAsync();
                }
            }
            if (typeof(T) == typeof(string))
                return (T)Convert.ChangeType(result, typeof(T));
            return JsonConvert.DeserializeObject<T>(result);
        }

        public static Uri GeoCodeRequest(string protocol, string city, string district, string ward, string address, string key)
        {
            string addressLine = address + ", " + ward;
            Uri geocodeRequest = new Uri(string.Format("{0}://dev.virtualearth.net/REST/v1/Locations?CountryRegion=VN&adminDistrict={1}&locality={2}&addressLine={3}&key={4}", protocol, city, district, addressLine, key));
            return geocodeRequest;
        }
        public static Uri GeoCodeRequest(string protocol, string query, string key)
        {
            Uri geocodeRequest = new Uri(string.Format("{0}://dev.virtualearth.net/REST/v1/Locations?q={1}&key={2}", protocol, query, key));
            return geocodeRequest;
        }

        public static Uri CreateOrderZaloRequest()
        {
            Uri geocodeRequest = new Uri(string.Format("https://sandbox.zalopay.com.vn/v001/tpe/createorder"));
            return geocodeRequest;
        }

        public static Uri CheckOrderZaloRequest()
        {
            Uri geocodeRequest = new Uri(string.Format("https://sandbox.zalopay.com.vn/v001/tpe/getstatusbyapptransid"));
            return geocodeRequest;
        }


        public static async Task<List<double>> GetLocation(string protocol, string key, string query)
        {
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, query, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            double lat = 0;
            double lng = 0;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("coordinates") != -1)
            {
                var split = localResult.Split("\"coordinates\":[")[1].Split("]")[0].Trim();
                double.TryParse(split.Split(",")[0], out lat);
                double.TryParse(split.Split(",")[1], out lng);
            }

            return new List<double>() { lat, lng };
        }
        public static async Task<List<double>> GetLocation(string protocol, string key, string city, string district, string ward, string address)
        {
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, city, district, ward, address, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            double lat = 0;
            double lng = 0;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("coordinates") != -1)
            {
                var split = localResult.Split("\"coordinates\":[")[1].Split("]")[0].Trim();
                double.TryParse(split.Split(",")[0], out lat);
                double.TryParse(split.Split(",")[1], out lng);
            }

            return new List<double>() { lat, lng };
        }
        public static async Task<string> GetLocationInString(string protocol, string key, string query)
        {
            Uri localRequest = RequestAPI.GeoCodeRequest(protocol, query, key);
            var localResult = await RequestAPI.Get<string>(localRequest);
            string? res = string.Empty;
            if (!string.IsNullOrEmpty(localResult) && localResult.IndexOf("adminDistrict") != -1)
            {
                res = localResult.Split("adminDistrict")[1].Split("\",")[0].Replace("\"", "").Replace(":", "").Trim();
            }

            return res;
        }
    }
}
