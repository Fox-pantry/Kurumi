using System.Collections;
using UnityEngine;

public class KeepAliveManager : MonoBehaviour
{
    public float healthCheckInterval = 10f;
    private Coroutine healthCheck;
    private int keepAliveTicks = 0;

    void Start()
    {
        healthCheck = StartCoroutine(HealthCheckLoop());
        Debug.Log("🔄 KeepAliveManager запущен");
    }

    IEnumerator HealthCheckLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(healthCheckInterval);

            var visual = FindObjectOfType<KurumiVisual>();
            var voice = FindObjectOfType<KurumiVoice>();

            if (visual == null || voice == null)
                Debug.LogWarning("⚠️ KeepAlive: компоненты не найдены!");
            else
            {
                keepAliveTicks++;
                if (keepAliveTicks % 6 == 0)
                    Debug.Log($"❤️ KeepAlive: {keepAliveTicks} тиков");
            }
        }
    }

    void OnDestroy()
    {
        if (healthCheck != null) StopCoroutine(healthCheck);
    }
}