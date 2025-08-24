using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Stage Vs Boss", menuName = "ScriptableObject/Stage/Type/Vs Boss", order = 1)]
public class StageVsBoss : StageRule
{
	public bool usePhase;
	private int bossIndex = 0;

	RuntimeData.StageInfo stageInfo;
	public override void Begin()
	{
		bossIndex = 0;
		base.Begin();
		_StageManager.it.usePhase = usePhase;
		stageInfo = _StageManager.it.CurrentStage;
	}

	public override void End()
	{
		_StageManager.it.ReturnNormalStage();
	}

	public override void OnLogicUpdate(float deltaTime)
	{
		if (isEnd)
		{
			return;
		}

		if (CheckEnd())
		{
			return;
		}

		base.OnLogicUpdate(deltaTime);

		elapsedTime += deltaTime;

		//if ((stageInfo.SpawnInterval > 0 && elapsedTime > stageInfo.SpawnInterval) || UnitManager.it.GetBosses().Count == 0)
		//{
		//	SpawnBoss();
		//	elapsedTime = 0;
		//}


	}

	private void SpawnBoss()
	{

		int displayCount = stageInfo.DisplayUnitCount;
		int bossSpawnCount = stageInfo.totalBossSpawnCount;

		int countLimit = stageInfo.BossCountLimit;
		int perWaveCount = 0;// Mathf.Min(stageInfo.SpawnPerWave, Mathf.Min(displayCount - UnitManager.it.GetBosses().Count));

		if (countLimit > 0)
		{
			int leftCount = countLimit - bossSpawnCount;
			perWaveCount = Mathf.Min(leftCount, perWaveCount);
		}

		if (perWaveCount == 0)
		{
			return;
		}

		int i = 0;
		for (; i < perWaveCount; i++)
		{
			int index = bossIndex + i;
			index = stageInfo.spawnBoss.Count > index ? index : 0;

			Transform[] bossSpawnPoses = _StageManager.it.map.bossSpawnPos;
			Vector3 pos = bossSpawnPoses != null && bossSpawnPoses.Length > 0 ? bossSpawnPoses[Random.Range(0, bossSpawnPoses.Length - 1)].position : new Vector3(2, 0, 0);

			//SpawnManager.it.SpawnLast(stageInfo.spawnBoss[index], pos, 1);
			bossIndex++;

		}
	}
}
