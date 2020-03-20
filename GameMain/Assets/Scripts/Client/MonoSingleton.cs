using UnityEngine;
using System.Collections;

public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance = null;
    public static T Instance
    {
        get
        {
            instance = instance ?? (FindObjectOfType(typeof(T)) as T);
            instance = instance ?? new GameObject(typeof(T).ToString(), typeof(T)).GetComponent<T>();
            return instance;
        }
    }

    protected virtual void OnDestroy()
    {
        instance = null;
    }
}