using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class GameObjectPool : Pool
{

	protected override void Release(GameObject obj)
	{
		base.Release(obj);
		obj.transform.SetParent(parent);
	}
}

public class ParticleEffectPoolManager : MonoBehaviour
{
	private static ParticleEffectPoolManager instance;

	public static ParticleEffectPoolManager it => instance;


	Dictionary<string, Pool> poolDictionary = new Dictionary<string, Pool>();

	private void Awake()
	{
		instance = this;
	}
	private void OnDestroy()
	{
		instance = null;
	}
	void Start()
	{
		Pool pool = new Pool();
		pool.parent = transform;
		pool.prefab = Resources.Load("FX/CuteDeath2") as GameObject;
		pool.CreatePool();
		poolDictionary.Add("death", pool);
		Pool hitpool = new Pool();
		hitpool.parent = transform;
		hitpool.prefab = Resources.Load("FX/HitEffect") as GameObject;
		hitpool.CreatePool();
		poolDictionary.Add("hit", hitpool);
	}

	public Pool GetPool(string name)
	{

		if (poolDictionary.ContainsKey(name))
		{
			return poolDictionary[name];
		}
		return null;
	}
	public Pool CreatePool(string name, GameObject prefab)
	{
		GameObjectPool pool = new GameObjectPool();
		pool.parent = transform;
		pool.prefab = prefab;
		pool.CreatePool();

		if (poolDictionary.ContainsKey(name) == false)
		{
			poolDictionary.Add(name, pool);
		}
		return pool;
	}

	public void RemovePool(string name)
	{
		if (poolDictionary.ContainsKey(name))
		{
			poolDictionary.Remove(name);
		}
	}

	public void RemoveGameObjectPool()
	{
		List<string> nameList = new List<string>();
		foreach (var pool in poolDictionary)
		{
			if (pool.Value is GameObjectPool)
			{
				nameList.Add(pool.Key);
			}
		}

		for (int i = 0; i < nameList.Count; i++)
		{
			if (poolDictionary.ContainsKey(nameList[i]))
			{
				poolDictionary[nameList[i]].pool.Clear();
				poolDictionary[nameList[i]].pool = null;

				poolDictionary.Remove(nameList[i]);
			}
		}
	}

}
