using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    private void Awake()
    {
        EventCenter.Instance.AddEventListener<GameObject>(GG.ABC, (x) =>
        {
            Debug.Log(x.GetInstanceID());
        });
    }
}
