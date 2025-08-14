namespace HumidityFanControl.Data.Models;

public class WeatherData
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public double Generationtime_ms { get; set; }
    public int Utc_offset_seconds { get; set; }
    public string Timezone { get; set; }
    public string Timezone_abbreviation { get; set; }
    public double Elevation { get; set; }
    public CurrentWeatherUnits Current_weather_units { get; set; }
    public CurrentWeather Current_weather { get; set; }
    public HourlyUnits Hourly_units { get; set; }
    public Hourly Hourly { get; set; }
}

public class CurrentWeatherUnits
{
    public string Time { get; set; }
    public string Interval { get; set; }
    public string Temperature { get; set; }
    public string Windspeed { get; set; }
    public string Winddirection { get; set; }
    public string Is_day { get; set; }
    public string Weathercode { get; set; }
}

public class CurrentWeather
{
    public string Time { get; set; }
    public int Interval { get; set; }
    public double Temperature { get; set; }
    public double Windspeed { get; set; }
    public int Winddirection { get; set; }
    public int Is_day { get; set; }
    public int Weathercode { get; set; }
}

public class HourlyUnits
{
    public string Time { get; set; }
    public string Relative_humidity_2m { get; set; }
}

public class Hourly
{
    public List<string> Time { get; set; }
    public List<int> Relative_humidity_2m { get; set; }
}