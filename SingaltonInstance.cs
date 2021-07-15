using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
public class SingaltonInstance<T> where T : new()
{
    private static T instance;

    public static T Instance => GetInstance();

    private static T GetInstance()
    {
        if (instance == null)
        {
            lock (typeof(T))
            {
                if (instance == null)
                {
                    instance = new T();
                }
            }
        }
        return instance;
    }
}
