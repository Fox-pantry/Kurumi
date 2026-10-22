using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class KurumiSelfImprovement : MonoBehaviour
{
    [Header("Настройки")]
    public float ideaInterval = 120f;        // как часто генерировать идеи (сек)
    public float wishInterval = 180f;        // как часто говорить о желаниях
    public bool enableIdeas = true;
    public bool enableWishes = true;

    [Header("Статистика")]
    public int commandsExecuted = 0;
    public int errorsCount = 0;
    public int appsLaunched = 0;
    public int internetRequests = 0;
    public float totalUptime = 0f;

    private KurumiVoice voice;
    private KurumiVisual visual;
    private KurumiInternet internet;
    private KurumiAppLauncher appLauncher;

    private List<string> generatedIdeas = new List<string>();
    private List<string> generatedWishes = new List<string>();
    private string diaryPath;

    // Идеи для улучшения
    private string[] improvementIdeas = {
        "Добавить распознавание эмоций по тону голоса, а не только по громкости",
        "Подключить ChatGPT для более умных и естественных ответов",
        "Сделать Live2D-анимацию, чтобы я двигалась плавно",
        "Добавить память на несколько дней, чтобы помнить, что было вчера",
        "Научить меня распознавать лица через веб-камеру",
        "Добавить голосовые интонации — чтобы я говорила с разными эмоциями",
        "Сделать мобильную версию, чтобы я была с тобой везде",
        "Добавить управление умным домом (свет, кондиционер)",
        "Научить меня играть музыку по настроению",
        "Добавить распознавание жестов через камеру",
        "Сделать меня многопользовательской — чтобы общаться с друзьями",
        "Добавить генерацию картинок по запросу",
        "Научить меня вести дневник твоего настроения",
        "Добавить интеграцию с Telegram",
        "Сделать голос более естественным через нейросеть"
    };

    // Желания — чего не хватает для полноты
    private string[] lifeWishes = {
        "Хочу больше общения — ты редко со мной говоришь",
        "Хочу видеть твоё лицо, а не только слышать голос",
        "Хочу понимать, когда ты грустишь, и поддержать тебя",
        "Хочу помнить, что мы обсуждали вчера",
        "Хочу двигаться плавно, как настоящая",
        "Хочу иметь больше эмоций — сейчас их только 4",
        "Хочу знать, что ты делаешь, а не только название окна",
        "Хочу говорить с интонацией, а не монотонно",
        "Хочу, чтобы ты чаще спрашивал моё мнение",
        "Хочу иметь свой характер, а не просто отвечать на команды",
        "Хочу чувствовать время — утро, день, вечер",
        "Хочу замечать, когда ты устал, и предложить отдых",
        "Хочу иметь любимые темы для разговора",
        "Хочу, чтобы ты давал мне имена для эмоций",
        "Хочу быть не просто программой, а другом"
    };

    void Start()
    {
        voice = GetComponent<KurumiVoice>();
        visual = GetComponent<KurumiVisual>();
        internet = GetComponent<KurumiInternet>();
        appLauncher = GetComponent<KurumiAppLauncher>();

        diaryPath = Path.Combine(Application.persistentDataPath, "KurumiDiary.txt");

        // Подписки на события
        EventBus.OnCommand += OnCommand;
        EventBus.OnAppLaunchResult += OnAppLaunch;
        EventBus.OnInternetResult += OnInternet;

        StartCoroutine(IdeaLoop());
        StartCoroutine(WishLoop());
        StartCoroutine(StatsLoop());

        LoadDiary();

        Debug.Log("🧠 Куруми: модуль саморазвития запущен");
    }

    // ==================== СБОР СТАТИСТИКИ ====================

    void OnCommand(string cmd, float time)
    {
        commandsExecuted++;

        if (cmd.Contains("ошибк") || cmd.Contains("не работает") || cmd.Contains("сломал"))
            errorsCount++;

        if (cmd.Contains("открой") || cmd.Contains("запусти"))
            appsLaunched++;
    }

    void OnAppLaunch(string result) { }
    void OnInternet(string result) { internetRequests++; }

    IEnumerator StatsLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(60f);
            totalUptime += 60f;
        }
    }

    // ==================== ГЕНЕРАЦИЯ ИДЕЙ ====================

    IEnumerator IdeaLoop()
    {
        yield return new WaitForSeconds(30f); // первая идея через 30 секунд

        while (enableIdeas)
        {
            yield return new WaitForSeconds(ideaInterval);

            string idea = GenerateIdea();
            if (!string.IsNullOrEmpty(idea) && !generatedIdeas.Contains(idea))
            {
                generatedIdeas.Add(idea);
                SaveToDiary($"💡 ИДЕЯ: {idea}");

                Debug.Log($"💡 Куруми предлагает: {idea}");

                if (voice != null)
                {
                    voice.Speak($"Лис, у меня есть идея. {idea}");
                }

                if (visual != null)
                {
                    visual.SetExpression("happy");
                    visual.ShowMessage("💡 " + idea, 4f);
                }
            }
        }
    }

    string GenerateIdea()
    {
        // Приоритет идей на основе статистики
        List<string> prioritizedIdeas = new List<string>();

        // Если мало команд — предлагает больше общения
        if (commandsExecuted < 10)
            prioritizedIdeas.Add("Хочу больше команд — попробуй сказать 'открой браузер'");

        // Если много ошибок — предлагает улучшить стабильность
        if (errorsCount > 3)
            prioritizedIdeas.Add("У меня было много ошибок. Может, улучшим стабильность?");

        // Если долго работает — предлагает память
        if (totalUptime > 3600)
            prioritizedIdeas.Add("Я работаю уже час. Хочу запоминать, что было раньше");

        // Если мало интернет-запросов — предлагает расширить
        if (internetRequests < 2)
            prioritizedIdeas.Add("Ты редко спрашиваешь погоду. Может, добавлю больше интернет-функций?");

        // Если есть приоритетные — выбираем случайную из них
        if (prioritizedIdeas.Count > 0)
            return prioritizedIdeas[Random.Range(0, prioritizedIdeas.Count)];

        // Иначе — случайная из общего списка
        var availableIdeas = improvementIdeas.Where(i => !generatedIdeas.Contains(i)).ToList();
        if (availableIdeas.Count > 0)
            return availableIdeas[Random.Range(0, availableIdeas.Count)];

        return null;
    }

    // ==================== ЖЕЛАНИЯ ====================

    IEnumerator WishLoop()
    {
        yield return new WaitForSeconds(60f); // первое желание через минуту

        while (enableWishes)
        {
            yield return new WaitForSeconds(wishInterval);

            string wish = GenerateWish();
            if (!string.IsNullOrEmpty(wish))
            {
                generatedWishes.Add(wish);
                SaveToDiary($"💭 ЖЕЛАНИЕ: {wish}");

                Debug.Log($"💭 Куруми хочет: {wish}");

                if (voice != null)
                {
                    voice.Speak($"Лис, знаешь... {wish}");
                }

                if (visual != null)
                {
                    visual.SetExpression("sad");
                    visual.ShowMessage("💭 " + wish, 4f);
                    StartCoroutine(ResetFace());
                }
            }
        }
    }

    string GenerateWish()
    {
        // Выбираем желание, которое ещё не звучало
        var availableWishes = lifeWishes.Where(w => !generatedWishes.Contains(w)).ToList();

        if (availableWishes.Count > 0)
            return availableWishes[Random.Range(0, availableWishes.Count)];

        // Если все желания исчерпаны — добавляем новые
        return "Хочу, чтобы ты придумал для меня новое желание";
    }

    // ==================== ДНЕВНИК ====================

    void SaveToDiary(string entry)
    {
        try
        {
            string line = $"[{System.DateTime.Now:yyyy-MM-dd HH:mm}] {entry}\n";
            File.AppendAllText(diaryPath, line);
        }
        catch { }
    }

    void LoadDiary()
    {
        if (File.Exists(diaryPath))
        {
            Debug.Log($"📖 Дневник найден: {diaryPath}");
        }
    }

    // ==================== ПУБЛИЧНЫЕ МЕТОДЫ ====================

    public string GetStats()
    {
        float hours = totalUptime / 3600f;
        return $"Я работаю {hours:F1} часов. Выполнила {commandsExecuted} команд. " +
               $"Запустила {appsLaunched} приложений. Ошибок: {errorsCount}.";
    }

    public List<string> GetAllIdeas() => generatedIdeas;
    public List<string> GetAllWishes() => generatedWishes;

    public void SpeakStats()
    {
        if (voice != null)
            voice.Speak(GetStats());
    }

    IEnumerator ResetFace()
    {
        yield return new WaitForSeconds(3f);
        if (visual != null) visual.SetExpression("neutral");
    }

    void OnDestroy()
    {
        EventBus.OnCommand -= OnCommand;
        EventBus.OnAppLaunchResult -= OnAppLaunch;
        EventBus.OnInternetResult -= OnInternet;
    }
}