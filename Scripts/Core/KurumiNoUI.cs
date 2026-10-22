using System.Collections;
using UnityEngine;

public class KurumiNoUI : MonoBehaviour
{
    public KurumiVisual visual;
    public KurumiVoice voice;
    public KurumiAppLauncher appLauncher;
    public KurumiTyper typer;
    public KurumiInternet internet;
    public KurumiSelfImprovement selfImprovement;

    private string[] idlePhrases = {
        "Лис, ты меня слышишь?",
        "Могу помочь, если что",
        "Скажи 'открой браузер'",
        "Хочешь, расскажу шутку?",
        "Я всегда здесь"
    };

    void Start()
    {
        visual = GetComponent<KurumiVisual>();
        voice = GetComponent<KurumiVoice>();
        appLauncher = GetComponent<KurumiAppLauncher>();
        typer = GetComponent<KurumiTyper>();
        internet = GetComponent<KurumiInternet>();
        selfImprovement = GetComponent<KurumiSelfImprovement>();

        EventBus.OnCommand += OnCommand;
        EventBus.OnAppLaunchResult += OnAppResult;
        EventBus.OnTypeResult += OnTypeResult;
        EventBus.OnInternetResult += OnInternetResult;

        voice.Speak("Привет, лис! Куруми готова к работе");
        StartCoroutine(IdleTalkLoop());
    }

    void OnCommand(string cmd, float time)
    {
        visual.SetExpression("happy");

        // === ЗАПУСК ПРИЛОЖЕНИЙ ===
        if (cmd.Contains("открой") || cmd.Contains("запусти"))
        {
            string appName = cmd.Replace("открой", "").Replace("запусти", "").Trim();
            if (appLauncher != null) appLauncher.LaunchApp(appName);
        }
        // === ПЕЧАТЬ ТЕКСТА ===
        else if (cmd.Contains("напиши") || cmd.Contains("набери"))
        {
            string text = cmd.Replace("напиши", "").Replace("набери", "").Trim();
            if (typer != null) typer.TypeText(text);
        }
        // === ИНТЕРНЕТ ===
        else if (cmd.Contains("погод")) internet?.GetWeather();
        else if (cmd.Contains("новост")) internet?.GetNews();
        else if (cmd.Contains("кто такой") || cmd.Contains("что такое"))
        {
            string query = cmd.Replace("кто такой", "").Replace("что такое", "").Trim();
            internet?.SearchWikipedia(query);
        }
        else if (cmd.Contains("шутк")) internet?.TellJoke();
        else if (cmd.Contains("цитат")) internet?.GetQuote();

        // === СТАТИСТИКА И ИДЕИ ===
        else if (cmd.Contains("статистика") || cmd.Contains("что ты умеешь"))
        {
            if (selfImprovement != null) selfImprovement.SpeakStats();
        }
        else if (cmd.Contains("какие идеи") || cmd.Contains("что улучшить"))
        {
            if (selfImprovement != null)
            {
                var ideas = selfImprovement.GetAllIdeas();
                if (ideas.Count > 0)
                    voice.Speak($"У меня {ideas.Count} идей. Последняя: {ideas[ideas.Count - 1]}");
                else
                    voice.Speak("Пока идей нет, но я подумаю!");
            }
        }
        else if (cmd.Contains("чего хочешь") || cmd.Contains("о чём мечтаешь"))
        {
            if (selfImprovement != null)
            {
                var wishes = selfImprovement.GetAllWishes();
                if (wishes.Count > 0)
                    voice.Speak($"Я мечтаю: {wishes[wishes.Count - 1]}");
                else
                    voice.Speak("Я хочу больше общения с тобой, лис~");
            }
        }

        // === БАЗОВЫЕ КОМАНДЫ ===
        else if (cmd.Contains("привет")) voice.Speak("Привет, лис! Как настроение?");
        else if (cmd.Contains("время")) voice.Speak($"Сейчас {System.DateTime.Now:HH:mm}");
        else if (cmd.Contains("как дела")) voice.Speak("У меня всё отлично! А у тебя?");
        else if (cmd.Contains("спасибо")) voice.Speak("Пожалуйста! Всегда рада помочь~");
        else if (cmd.Contains("пока")) voice.Speak("Пока, лис! Зови, если что~");
        else voice.Speak("Не поняла команду. Скажи 'открой браузер' или 'погода'");

        StartCoroutine(ResetFace());
    }

    IEnumerator IdleTalkLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(30f, 90f));
            if (Time.time - GlobalState.LastVoiceTime > 60f)
            {
                string phrase = idlePhrases[Random.Range(0, idlePhrases.Length)];
                voice.Speak(phrase);
            }
        }
    }

    void OnAppResult(string result) { voice.Speak(result); }
    void OnTypeResult(string result) { voice.Speak(result); }
    void OnInternetResult(string result) { voice.Speak(result); }

    IEnumerator ResetFace()
    {
        yield return new WaitForSeconds(2f);
        visual.SetExpression("neutral");
    }

    void OnDestroy()
    {
        EventBus.OnCommand -= OnCommand;
        EventBus.OnAppLaunchResult -= OnAppResult;
        EventBus.OnTypeResult -= OnTypeResult;
        EventBus.OnInternetResult -= OnInternetResult;
    }
}