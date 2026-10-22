using UnityEngine;
using System.Collections.Generic;
using System;

public class UnityMainThreadDispatcher : MonoBehaviour
{
    private static UnityMainThreadDispatcher instance;
    private static readonly Queue<Action> actions = new Queue<Action>();

    public static UnityMainThreadDispatcher Instance()
    {
        if (instance == null)
        {
            GameObject go = new GameObject("Dispatcher");
            instance = go.AddComponent<UnityMainThreadDispatcher>();
            DontDestroyOnLoad(go);
        }
        return instance;
    }

    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != this) Destroy(gameObject);
    }

    void Update()
    {
        lock (actions)
        {
            while (actions.Count > 0)
                actions.Dequeue()?.Invoke();
        }
    }

    public void Enqueue(Action action)
    {
        lock (actions) actions.Enqueue(action);
    }
}