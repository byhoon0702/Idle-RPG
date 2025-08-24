using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Zone : MonoBehaviour
{
    public ZoneTriggerEventListener zoneTriggerListener;
    public Transform spawnPos;
    
    private Spawner spawner;
    // Start is called before the first frame update
    void Start()
    {
        spawner = UnitManager.Instance.spanwer;
        zoneTriggerListener.triggerEvents += OnTriggerEvents;

 
    }
    private void Update()
    {
        

    }

    private void OnTriggerEvents()
    {
        
    }
}
