using System.Collections.Generic;
using System.Linq;

namespace UfaSmartCity.Models
{
    public class WeatherCity
    {
        public Coord coord { get; set; }
        public List<Weather> weather { get; set; }
        public string @base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public Rain rain { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }

        public void Set(WeatherCity weatherCity)
        {
            @base = weatherCity.@base;
            clouds = new Clouds()
            {
                all = weatherCity.clouds.all
            };
            cod = weatherCity.cod;
            coord = new Coord()
            {
                Id = weatherCity.coord.Id,
                lat = weatherCity.coord.lat,
                lon = weatherCity.coord.lon
            };
            dt = weatherCity.dt;
            id = weatherCity.id;
            main = new Main()
            {
                humidity = weatherCity.main.humidity,
                pressure = weatherCity.main.pressure,
                temp = weatherCity.main.temp,
                temp_max = weatherCity.main.temp_max,
                temp_min = weatherCity.main.temp_min
            };
            name = weatherCity.name;
            rain = new Rain()
            {
                Precipitation_In_The_Last_3_Hours = weatherCity.rain.Precipitation_In_The_Last_3_Hours
            };
            sys = new Sys()
            {
                country = weatherCity.sys.country,
                id = weatherCity.sys.id,
                message = weatherCity.sys.message,
                sunrise = weatherCity.sys.sunrise,
                sunset = weatherCity.sys.sunset,
                type = weatherCity.sys.type
            };
            weather = new List<Weather>()
                {
                    new Weather()
                    {
                        description = weatherCity.weather
                                                 .ToList()
                                                 .FirstOrDefault()
                                                 .description,
                        icon = weatherCity.weather
                                                 .ToList()
                                                 .FirstOrDefault()
                                                 .icon,
                        id = weatherCity.weather
                                        .FirstOrDefault()
                                        .id,
                        main = weatherCity.weather
                                          .FirstOrDefault()
                                          .main                        
                    }
                };          
        }
    }
}