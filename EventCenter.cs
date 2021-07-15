using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

public enum GG
{
    ABC,
}
interface IEventInfo
{

}

class EventInfo<T> : IEventInfo
{
    private List<Action<T>> actions = new List<Action<T>>();
    // public EventInfo(Action<T> act)
    // {
    //     Add(act);
    // }
    public void Add(Action<T> action)
    {
        actions.Add(action);
    }

    public void Remove(Action<T> action)
    {
        actions.Remove(action);
    }

    public void Invoke(T parm)
    {
        for (var i = 0; i < actions.Count; i++)
        {
            try
            {
                if (actions[i]!=null)
                {
                    actions.RemoveAt(i--);
                }
                else
                {
                    actions[i]?.Invoke(parm);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}

class EventInfo : IEventInfo
{
    private List<Action> actions = new List<Action>();
    // public EventInfo(Action act)
    // {
    //     Add(act);
    // }

    public void Add(Action action)
    {
        actions.Add(action);
    }

    public void Remove(Action action)
    {
        actions.Remove(action);
    }

    public void Invoke()
    {
        for (var i = 0; i < actions.Count; i++)
        {
            try
            {
                if (actions[i]!=null)
                {
                    actions.RemoveAt(i--);
                }
                else
                {
                    actions[i]?.Invoke();
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex);
            }
        }
    }
}


public class EventCenter : SingaltonInstance<EventCenter>
{
    private class VoidType { }
    private Dictionary<string, Dictionary<Type, IEventInfo>> eventToInfo = new Dictionary<string, Dictionary<Type, IEventInfo>>();
    public void AddEventListener<T>(Enum eventEnum, Action<T> act)
    {
        AddEventListener(EnumToString(eventEnum), act);
    }

    public void AddEventListener(Enum eventEnum, Action act)
    {
        AddEventListener(EnumToString(eventEnum), act);
    }

    public void AddEventListener<T>(string eventName, Action<T> act)
    {
        if (!eventToInfo.ContainsKey(eventName))
        {
            eventToInfo.Add(eventName, new Dictionary<Type, IEventInfo>());
        }

        if (!eventToInfo[eventName].ContainsKey(typeof(T)))
        {
            eventToInfo[eventName].Add(typeof(T), new EventInfo<T>());
        }
        (eventToInfo[eventName][typeof(T)] as EventInfo<T>).Add(act);
    }

    public void AddEventListener(string eventName, Action act)
    {
        if (!eventToInfo.ContainsKey(eventName))
        {
            eventToInfo.Add(eventName, new Dictionary<Type, IEventInfo>());
        }

        if (!eventToInfo[eventName].ContainsKey(typeof(VoidType)))
        {
            eventToInfo[eventName].Add(typeof(VoidType), new EventInfo());
        }
        (eventToInfo[eventName][typeof(VoidType)] as EventInfo).Add(act);
    }


    public void RemoveEventListener<T>(Enum eventEnum, Action<T> act)
    {
        RemoveEventListener(EnumToString(eventEnum), act);
    }

    public void RemoveEventListener(Enum eventEnum, Action act)
    {
        RemoveEventListener(EnumToString(eventEnum), act);
    }
    public void RemoveEventListener<T>(string eventName, Action<T> act)
    {
        if (eventToInfo.ContainsKey(eventName) && eventToInfo[eventName].ContainsKey(typeof(T)))
            (eventToInfo[eventName] as EventInfo<T>).Remove(act);
    }

    public void RemoveEventListener(string eventName, Action act)
    {
        if (eventToInfo.ContainsKey(eventName) && eventToInfo[eventName].ContainsKey(typeof(VoidType)))
            (eventToInfo[eventName][typeof(VoidType)] as EventInfo).Remove(act);
    }

    public void EventTrigger<T>(Enum eventEnum, T value)
    {
        EventTrigger(EnumToString(eventEnum), value);
    }

    public void EventTrigger(Enum eventEnum)
    {
        EventTrigger(EnumToString(eventEnum));
    }

    public void EventTrigger<T>(string eventName, T value)
    {
        (eventToInfo?[eventName][typeof(T)] as EventInfo<T>)?.Invoke(value);
    }

    public void EventTrigger(string eventName)
    {
        (eventToInfo?[eventName]?[typeof(VoidType)] as EventInfo)?.Invoke();
    }

    public void Reset()
    {
        eventToInfo.Clear();
    }

    private string EnumToString(Enum _enum)
    {
        return _enum.GetType().FullName + "+" + _enum.ToString();
    }
}
