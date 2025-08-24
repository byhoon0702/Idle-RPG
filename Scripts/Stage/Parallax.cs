using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
public class Parallax : MonoBehaviour
{
    public Camera vccamera;
    public Tilemap tilemap;
    public ZoneTriggerEventListener zoneTriggerListener;
    public Transform spawnPos;
    
    private Spawner spawner;

    public Vector2 parallaxEffect;
    private float startPosX;
    private float boundX;
    // Start is called before the first frame update
    void Start()
    {
        startPosX = transform.position.x;
        tilemap.CompressBounds();
        Debug.Log(tilemap.cellBounds);
        boundX = tilemap.cellBounds.size.x;
        spawner = UnitManager.Instance.spanwer;
        zoneTriggerListener.triggerEvents += OnTriggerEvents;
    }
    private void OnTriggerEvents()
    {
        UnitManager.Instance.CreateEnemy();
    }
    // Update is called once per frame
    void Update()
    {
        float relativeDIst = vccamera.transform.position.x * parallaxEffect.x;
        float relativeDistY = vccamera.transform.position.y * parallaxEffect.y;

        transform.position = new Vector3(startPosX + relativeDIst, transform.position.y + relativeDistY, 0);
        float relativeCameraDist = vccamera.transform.position.x * (1 - parallaxEffect.x);
        if(relativeCameraDist > startPosX + boundX)
        {
            startPosX += boundX;
            Debug.Log("Shifting Map");
        }
        else if(relativeCameraDist< startPosX - boundX)
        {
            startPosX -= boundX;
        }
        
    }
}
