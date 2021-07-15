using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class Asss : MonoBehaviour
{
    private void OnEnable()
    {
        //Mathf.Max(5, 6);
        EventCenter.Instance.EventTrigger<GameObject>(GG.ABC, new GameObject());
        //Assembly assembly = Assembly.GetExecutingAssembly();
        //Attribute a = Attribute.GetCustomAttribute(typeof(AttributeUse).GetMethod("Print"), typeof(CusAttitude));
        //Debug.LogError(((CusAttitude)a).name);
    }
}
