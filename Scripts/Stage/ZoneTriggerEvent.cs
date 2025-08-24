using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void ZoneTriggerEvent();
 
public class ZoneTriggerEventListener : MonoBehaviour
{
    public event ZoneTriggerEvent triggerEvents;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        triggerEvents?.Invoke();
    }
}
