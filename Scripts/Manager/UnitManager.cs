using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner
{
    public bool IsAllEnemyDead;
    public bool PlayerDead;

    public List<Enemy> enemyList;


    public T CreateUnit<T>(string resource) where T : Unit
    {
        GameObject prefab = Resources.Load(resource) as GameObject;
        GameObject go = UnityEngine.Object.Instantiate(prefab);

        return go.GetComponent<T>();
    }


    public void ClearUnits()
    {

    }
}

public class UnitFactory
{
    public void Generate()
    {

    }

}


public class UnitManager : MonoBehaviour
{
	
	public static UnitManager Instance;

    public Spawner spanwer;
    public Transform unitParent;
    public Transform playerPos;
    public Transform enemyPos;

    public Player player;
    public List<Enemy> enemies= new List<Enemy>();
    private void Awake()
    {
        Instance = this;
        spanwer = new Spawner();
    }
    public void CreatePlayer()
    {
        player = spanwer.CreateUnit<Player>("Player");
        player.transform.SetParent(unitParent);
        player.transform.position = playerPos.position;
        player.Spawn();
    }

    public void CreateEnemy()
    {
        var enemy =spanwer.CreateUnit<Enemy>("LightBandit");
        enemy.transform.SetParent(unitParent);
        enemy.transform.position = enemyPos.position;
        enemy.Spawn();
        enemies.Add(enemy);
    }


}
