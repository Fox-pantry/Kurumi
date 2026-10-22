using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using Newtonsoft.Json.Linq;

public class KurumiInternet : MonoBehaviour
{
    private string currentCity = "Москва";

    public void GetWeather()
    {
        StartCoroutine(FetchWeather());
    }

    IEnumerator FetchWeather()
    {
        using (UnityWebRequest web = UnityWebRequest.Get($"https://wttr.in/{currentCity}?format=%t+%C&lang=ru"))
        {
            yield return web.SendWebRequest();
            if (web.result == UnityWebRequest.Result.Success)
                EventBus.Trigger("OnInternetResult", $"Сейчас в {currentCity}: {web.downloadHandler.text.Trim()}");
            else
                EventBus.Trigger("OnInternetResult", "Не могу узнать погоду");
        }
    }

    public void GetNews()
    {
        EventBus.Trigger("OnInternetResult", "Для новостей нужен API-ключ");
    }

    public void SearchWikipedia(string query)
    {
        StartCoroutine(FetchWikipedia(query));
    }

    IEnumerator FetchWikipedia(string query)
    {
        string encoded = UnityWebRequest.EscapeURL(query);
        using (UnityWebRequest web = UnityWebRequest.Get("https://ru.wikipedia.org/api/rest_v1/page/summary/" + encoded))
        {
            web.SetRequestHeader("User-Agent", "Kurumi/1.0");
            yield return web.SendWebRequest();

            if (web.result == UnityWebRequest.Result.Success)
            {
                var data = JObject.Parse(web.downloadHandler.text);
                string extract = data["extract"]?.ToString() ?? "";
                if (extract.Length > 300) extract = extract.Substring(0, 300) + "...";
                if (!string.IsNullOrEmpty(extract))
                    EventBus.Trigger("OnInternetResult", extract);
                else
                    EventBus.Trigger("OnInternetResult", $"Не нашла информацию о '{query}'");
            }
            else
                EventBus.Trigger("OnInternetResult", $"Не удалось найти '{query}'");
        }
    }

    public void TellJoke()
    {
        StartCoroutine(FetchJoke());
    }

    IEnumerator FetchJoke()
    {
        using (UnityWebRequest web = UnityWebRequest.Get("https://official-joke-api.appspot.com/random_joke"))
        {
            yield return web.SendWebRequest();
            if (web.result == UnityWebRequest.Result.Success)
            {
                var joke = JObject.Parse(web.downloadHandler.text);
                string setup = joke["setup"]?.ToString() ?? "";
                string punchline = joke["punchline"]?.ToString() ?? "";
                EventBus.Trigger("OnInternetResult", $"{setup} ... {punchline}");
            }
            else
                EventBus.Trigger("OnInternetResult", "Почему программисты не любят природу? Слишком много багов!");
        }
    }

    public void GetQuote()
    {
        StartCoroutine(FetchQuote());
    }

    IEnumerator FetchQuote()
    {
        using (UnityWebRequest web = UnityWebRequest.Get("https://api.quotable.io/random"))
        {
            yield return web.SendWebRequest();
            if (web.result == UnityWebRequest.Result.Success)
            {
                var quote = JObject.Parse(web.downloadHandler.text);
                string content = quote["content"]?.ToString() ?? "";
                string author = quote["author"]?.ToString() ?? "Неизвестен";
                EventBus.Trigger("OnInternetResult", $"«{content}» — {author}");
            }
            else
                EventBus.Trigger("OnInternetResult", "Мечтай так, будто жить вечно. — Джеймс Дин");
        }
    }
}