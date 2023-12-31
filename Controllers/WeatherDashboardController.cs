using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ReactDemo.Models;
using System.Text.Json;

namespace ReactDemo.Controllers
{
    public class WeatherDashboardController : Controller
    {
        static WeatherDashboardController()
        {}

        private static async Task<BagModel> searchCity(string query)
        {
            string apiKey = getWeatherApiKey();
            if (apiKey == null || query == null || query == "")
            {
                return new BagModel{Cities = new List<CityModel>()};
            }

            string searchUri = $"http://api.weatherapi.com/v1/search.json?key={apiKey}&q={query}";
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(searchUri);
            string content = await response.Content.ReadAsStringAsync();

            List<CityModel> cities = JsonSerializer.Deserialize<List<CityModel>>(
                content, 
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                }                
            );

            BagModel bag = new BagModel{Cities = cities};
            return bag;
        }

        private static async Task<BagModel> getCityWeather(string cityId)
        {
            string apiKey = getWeatherApiKey();
            if (apiKey == null || cityId == null || cityId == "")
            {
                return new BagModel {
                    Cities = new List<CityModel>(),
                    CityWeather = new CityWeather()
                };
            }

            string searchUri =$"http://api.weatherapi.com/v1/forecast.json?key={apiKey}&q=id:{cityId}&days=3&aqi=no&alerts=yes";
            HttpClient client = new HttpClient();
            using HttpResponseMessage response = await client.GetAsync(searchUri);
            string content = await response.Content.ReadAsStringAsync();

            CityWeather cityWeather;
            try {
                cityWeather = JsonSerializer.Deserialize<CityWeather>(
                    content, 
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }                
                );
            }
            catch (JsonException e){
                Console.WriteLine(e.ToString());
                return new BagModel {
                    Cities = new List<CityModel>(),
                    CityWeather = new CityWeather()
                };

            }
            catch(ArgumentNullException e) {
                Console.WriteLine(e.ToString());
                return new BagModel {
                    Cities = new List<CityModel>(),
                    CityWeather = new CityWeather()
                };
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
                return new BagModel {
                    Cities = new List<CityModel>(),
                    CityWeather = new CityWeather()
                };
            }

            Console.WriteLine(cityWeather.ToString());

            BagModel bag = new BagModel{
                Cities = new List<CityModel>(),
                CityWeather = cityWeather    
            };
            return bag;
        }

        [Route("weather-dashboard")]
        [HttpGet]
        public  async Task<ActionResult> Index()
        {
            return View(await searchCity(""));
        }

        [Route("city-search")]
        [HttpGet]
        public async Task<ActionResult> CitySearch(string query)
        {
            return Json(await searchCity(query));
        }

        [Route("city-weather")]
        [HttpGet]
        public async Task<ActionResult> CityWeather(string cityId)
        {
            Console.WriteLine($"cityId: {cityId}");
            return Json(await getCityWeather(cityId));
        }

        private static string getWeatherApiKey()
        {
            string apiKey = Environment.GetEnvironmentVariable("WEATHER_API_KEY");
            if (apiKey == null)
            {
                Console.WriteLine("WEATHER_API_KEY not set");
            }
            return apiKey;
        }
    }

}