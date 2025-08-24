using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SerializeField]
public class StageInfo
{
    public int enemyCount;
    public int enemyPerWave;
}

public class StageManager : MonoBehaviour
{
    public static StageManager Instance;

    public StageInfo stageInfo;

    private Spawner spawner;
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        spawner = UnitManager.Instance.spanwer;
    }

}
