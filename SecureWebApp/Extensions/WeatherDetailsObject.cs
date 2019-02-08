using System;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace SALUSUAV_Demo.Extensions
{
    public class WeatherDetailsObject
    {
        public string Temperature { get; set; }
        public string Wind { get; set; }
        public string Humidity { get; set; }
        public string Visibility { get; set; }
        public string Sunrise { get; set; }
        public string Sunset { get; set; }

        public WeatherDetailsObject()
        {
            Temperature = " ";
            Wind = " ";
            Humidity = " ";
            Visibility = " ";
            Sunrise = " ";
            Sunset = " ";
        }

        public static async Task<WeatherDetailsObject> GetWeather(string latitude, string longitude)
        {
            float lat = Convert.ToSingle(latitude);
            int lon = Convert.ToInt32(Convert.ToSingle(longitude));

            string key = "fab223ad98a5c2db1895953ffb77cdf0";
            string queryString = "http://api.openweathermap.org/data/2.5/weather?lat="
                                 + lat + "&lon=" + lon + ",&appid=" + key + "&units=imperial";

            //Make sure developers running this sample replaced the API key
            if (key == "YOUR API KEY HERE")
            {
                throw new ArgumentException("You must obtain an API key from openweathermap.org/appid and save it in the 'key' variable.");
            }

            dynamic results = await GetDataFromService(queryString).ConfigureAwait(false);

            if (results["weather"] != null)
            {
                WeatherDetailsObject weatherDetailsObject = new WeatherDetailsObject
                {
                    Temperature = (string)results["main"]["temp"] + " F",
                    Wind = (string)results["wind"]["speed"] + " mph",
                    Humidity = (string)results["main"]["humidity"] + " %",
                    Visibility = (string)results["weather"][0]["main"]
                };

                DateTime time = new DateTime(1970, 1, 1, 0, 0, 0, 0);
                DateTime sunrise = time.AddSeconds((double)results["sys"]["sunrise"]);
                DateTime sunset = time.AddSeconds((double)results["sys"]["sunset"]);
                weatherDetailsObject.Sunrise = sunrise + " UTC";
                weatherDetailsObject.Sunset = sunset + " UTC";
                return weatherDetailsObject;
            }

            return null;
        }

        public static async Task<dynamic> GetDataFromService(string queryString)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetAsync(queryString);

            dynamic data = null;
            if (response != null)
            {
                string json = response.Content.ReadAsStringAsync().Result;
                data = JsonConvert.DeserializeObject(json);
            }

            return data;
        }
    }

}
