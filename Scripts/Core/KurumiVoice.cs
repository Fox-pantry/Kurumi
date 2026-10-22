using System;
using System.Collections;
using System.Speech.Synthesis;
using UnityEngine;
using Whisper;

public class KurumiVoice : MonoBehaviour
{
    public float detectionThreshold = 0.015f;
    public bool enableWhisper = true, enableTTS = true;
    public int recordingLength = 3;

    private AudioClip micClip;
    private string micDevice;
    private bool isListening = true;
    private int lastPos = 0;
    private bool isRecognizing = false;
    private SpeechSynthesizer tts;
    private WhisperManager whisper;

    void Start()
    {
        if (enableTTS)
        {
            try
            {
                tts = new SpeechSynthesizer();
                tts.SetOutputToDefaultAudioDevice();
                foreach (var voice in tts.GetInstalledVoices())
                {
                    if (voice.VoiceInfo.Culture.Name.StartsWith("ru") &&
                        voice.VoiceInfo.Gender == VoiceGender.Female)
                    {
                        tts.SelectVoice(voice.VoiceInfo.Name);
                        Debug.Log($"TTS: {voice.VoiceInfo.Name}");
                        break;
                    }
                }
            }
            catch (Exception e) { Debug.LogWarning($"TTS не работает: {e.Message}"); enableTTS = false; }
        }

        InitMicrophone();
        whisper = FindObjectOfType<WhisperManager>();
        if (whisper == null && enableWhisper)
            Debug.LogWarning("WhisperManager не найден на сцене!");

        StartCoroutine(ListenLoop());
        Speak("Куруми запущена");
    }

    void InitMicrophone()
    {
        if (Microphone.devices.Length == 0)
        {
            Debug.LogError("Микрофон не найден!");
            return;
        }
        micDevice = Microphone.devices[0];
        micClip = Microphone.Start(micDevice, true, 10, 16000);
        int timeout = 0;
        while (Microphone.GetPosition(micDevice) <= 0 && timeout < 100) timeout++;
        Debug.Log($"Микрофон: {micDevice}");
    }

    IEnumerator ListenLoop()
    {
        while (isListening && micClip != null)
        {
            int pos = Microphone.GetPosition(micDevice);
            if (pos > 0 && lastPos > 0 && pos != lastPos)
            {
                int diff = (pos - lastPos + micClip.samples) % micClip.samples;
                if (diff > 0 && diff < 16000 && diff > 5000)
                {
                    float[] samples = new float[diff];
                    micClip.GetData(samples, lastPos);

                    float energy = 0f;
                    foreach (float s in samples) energy += Mathf.Abs(s);
                    energy /= diff;

                    if (energy > detectionThreshold)
                    {
                        EventBus.Trigger("OnVoiceDetected", energy, Time.time);
                        if (enableWhisper && whisper != null && !isRecognizing)
                            StartCoroutine(RecognizeFromMic());
                    }
                }
            }
            lastPos = pos;
            yield return new WaitForSeconds(0.1f);
        }
    }

    IEnumerator RecognizeFromMic()
    {
        if (isRecognizing) yield break;
        isRecognizing = true;

        Debug.Log("🎙️ Запись для распознавания...");
        Microphone.End(micDevice);

        AudioClip recordClip = Microphone.Start(micDevice, false, recordingLength, 16000);
        yield return new WaitForSeconds(recordingLength);
        Microphone.End(micDevice);

        micClip = Microphone.Start(micDevice, true, 10, 16000);

        if (whisper != null)
        {
            Debug.Log("🧠 Распознаю речь...");
            var task = whisper.GetTextAsync(recordClip);
            yield return new WaitUntil(() => task.IsCompleted);

            try
            {
                var result = task.Result;
                string recognizedText = result?.Result ?? "";

                if (!string.IsNullOrEmpty(recognizedText))
                {
                    Debug.Log($"📝 Распознано: {recognizedText}");
                    GlobalState.LastVoiceTime = Time.time;
                    EventBus.Trigger("OnCommand", recognizedText.ToLower(), Time.time);
                }
            }
            catch (Exception e) { Debug.LogWarning($"Whisper ошибка: {e.Message}"); }
        }

        isRecognizing = false;
    }

    public void Speak(string text)
    {
        Debug.Log($"🦊 Куруми: {text}");
        EventBus.Trigger("OnKurumiSpeak", text, Time.time);

        if (enableTTS && tts != null)
        {
            try { tts.SpeakAsync(text); }
            catch (Exception e) { Debug.LogWarning($"TTS ошибка: {e.Message}"); }
        }

        var visual = GetComponent<KurumiVisual>();
        if (visual != null) visual.StartTalking(text);
    }

    void OnDestroy()
    {
        isListening = false;
        if (micDevice != null && Microphone.IsRecording(micDevice))
            Microphone.End(micDevice);
        if (tts != null) tts.Dispose();
    }
}