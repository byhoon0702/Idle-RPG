using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SpawnManager
{
    public bool IsAllEnemyDead;
    public bool PlayerDead;

    public List<Enemy> enemyList;
    

    public void CreatePlayer(string resource)
    {
        GameObject prefab = Resources.Load(resource) as GameObject;
        GameObject go = UnityEngine.Object.Instantiate(prefab);
    }


    public void ClearUnits()
	{
		
	}
}
