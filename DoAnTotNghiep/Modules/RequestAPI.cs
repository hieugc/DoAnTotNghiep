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
    }
}
