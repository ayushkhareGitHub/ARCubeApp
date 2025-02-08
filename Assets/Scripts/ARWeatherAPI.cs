using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections;

public class ARWeatherAPI : MonoBehaviour
{
    public Text weatherText; // UI Text element to display weather info

    private string apiKey = "d536ae0eaa7567214db13fab35355d03"; // OpenWeatherMap API Key
    private string zipCode = "248001"; // ZIP code for location
    private string countryCode = "IN"; // Country code

    void Start()
    {
        StartCoroutine(GetWeatherData());
    }

    IEnumerator GetWeatherData()
    {
        // Construct API request URL
        string url = $"https://api.openweathermap.org/data/2.5/weather?zip={zipCode},{countryCode}&appid={apiKey}&units=metric";

        UnityWebRequest request = UnityWebRequest.Get(url);
        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            string response = request.downloadHandler.text;
            DisplayWeather(response);
        }
        else
        {
            weatherText.text = "Failed to fetch weather!";
        }
    }

    void DisplayWeather(string json)
    {
        // Parse JSON response
        WeatherResponse weatherData = JsonUtility.FromJson<WeatherResponse>(json);

        if (weatherData != null && weatherData.main != null && weatherData.weather.Length > 0)
        {
            weatherText.text = $"Temperature: {weatherData.main.temp}°C\nWeather: {weatherData.weather[0].main}";
        }
        else
        {
            weatherText.text = "Invalid Weather Data!";
        }
    }
}

// Classes to map JSON response
[System.Serializable]
public class WeatherResponse
{
    public Main main;
    public Weather[] weather;
}

[System.Serializable]
public class Main
{
    public float temp;
}

[System.Serializable]
public class Weather
{
    public string main;
}
