using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class MainThreadDispatcher : MonoBehaviour
{

    private static readonly Queue<Action> _executionQueue = new Queue<Action>();

    public void Update()
    {
        lock (_executionQueue)
        {
            while (_executionQueue.Count > 0)
            {
                _executionQueue.Dequeue().Invoke();
            }
        }
    }

    /// <summary>
    /// Locks the queue and adds the IEnumerator to the queue
    /// </summary>
    /// <param name="action">IEnumerator function that will be executed from the main thread.</param>
    public void Enqueue(IEnumerator action)
    {
        lock (_executionQueue)
        {
            _executionQueue.Enqueue(() =>
            {
                if (action != null)
                {
                    StartCoroutine(action);
                } 
            });
        }
    }

    public void StopCoroutine()
    {
        Debug.Log("stop all coroutine");
        StopAllCoroutines();
    }

    public void ToStopCoroutine(IEnumerator coroution)
    {
        StopCoroutine(coroution);
    }

    /// <summary>
    /// Locks the queue and adds the Action to the queue
    /// </summary>
    /// <param name="action">function that will be executed from the main thread.</param>
    public void Enqueue(Action action)
    {
        Enqueue(ActionWrapper(action));
    }

    IEnumerator ActionWrapper1(Action a)
    {
        yield return new WaitForEndOfFrame();
        a();
    }

    IEnumerator ActionWrapper(Action a)
    {
        a();
        yield return null;
    }


    private static MainThreadDispatcher _instance = null;

    public static bool Exists()
    {
        return _instance != null;
    }

    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = GameObject.FindObjectOfType<MainThreadDispatcher>();
            }
            if (!Exists())
            {
                throw new Exception("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
            }
            return _instance;
        }
    }


    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }


}