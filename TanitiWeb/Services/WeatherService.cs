using TanitiWeb.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace TanitiWeb.Services;

public class WeatherService
{
    private readonly Random _rand = new();

    // Cached forecast for this app session
    private List<ForecastDay>? _cachedForecast;

    public List<ForecastDay> GetWeeklyForecast()
    {
        // If we already generated weather this session, reuse it
        if (_cachedForecast != null)
            return _cachedForecast;

        // Otherwise generate a fresh 7-day forecast
        _cachedForecast = GenerateWeeklyForecast();
        return _cachedForecast;
    }

    public List<ForecastDay> GetFooterForecast()
    {
        // Footer uses the same forecast, just fewer days
        return GetWeeklyForecast().Take(4).ToList();
    }

    private List<ForecastDay> GenerateWeeklyForecast()
    {
        var season = GetSeason(DateTime.Now);
        var list = new List<ForecastDay>();

        for (int i = 0; i < 7; i++)
        {
            var date = DateTime.Now.AddDays(i);
            var temps = GenerateDailyTemps(season);
            
            var cond = GenerateConditions();

            list.Add(new ForecastDay
            {
                Day = date.ToString("ddd"),
                High = temps.High,
                Low = temps.Low,
                Conditions = cond.Condition,
                Icon = cond.Icon,
            });
        }

        return list;
    }

    // --- Season Logic -------------------------------------------------------

    public enum Season
    {
        Dry,
        Wet
    }

    public Season GetSeason(DateTime date)
    {
        int month = date.Month;

        // Dry Season: May–Oct
        if (month >= 5 && month <= 10)
            return Season.Dry;

        // Wet Season: Nov–Apr
        return Season.Wet;
    }

    private (int High, int Low) GenerateDailyTemps(Season season)
    {
        return season switch
        {
            Season.Dry => (
                High: _rand.Next(84, 93),
                Low: _rand.Next(72, 79)
            ),
            Season.Wet => (
                High: _rand.Next(80, 89),
                Low: _rand.Next(70, 77)
            ),
            _ => (85, 75)
        };
    }

    private (string Condition, string Icon) GenerateConditions()
    {
        var options = new (string Condition, string Icon)[]
        {
            ("Sunny", "☼"),
            ("Partly Cloudy", "⛅"),
            ("Cloudy", "☁"),
            ("Rain Showers", "☂"),
            ("Thunderstorms", "⚡"),
            ("Hot & Dry", "♨")
        };

        return options[_rand.Next(options.Length)];
    }
}

public class ForecastDay
{
    public string Day { get; set; } = "";
    public int High { get; set; }
    public int Low { get; set; }
    public string Conditions { get; set; } = "";
    public string Icon { get; set; } = "";
}
