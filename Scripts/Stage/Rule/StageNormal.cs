

using UnityEngine;

[CreateAssetMenu(fileName = "Stage Normal", menuName = "ScriptableObject/Stage/Type/Normal", order = 1)]
public class StageNormal : StageRule
{
	private float spawnTime;

	public override void Begin()
	{
		SceneCamera.PlayableDirector.playableAsset = timelineCutScene;
		base.Begin();
		spawnTime = 0;

		GameUIManager.it.uiController.AdRewardChest.Init();
	}

	public override void End()
	{
		if (isWin == false)
		{
			_StageManager.it.ReturnNormalStage();
		}
		else
		{

			_StageManager.it.NextNormalStage();
		}
	}

	public override void OnUpdate(float deltaTime)
	{
		
		var next = currentFsm?.RunNextState(deltaTime);
		if (next != null)
		{
			currentFsm = StageFSM.Get(next);
		}
	}

	public override void OnLogicUpdate(float deltaTime)
	{
		if (GameManager.GameStop)
		{
			return;
		}
		if (isEnd)
		{
			return;
		}


		GameUIManager.it.uiController.AdRewardChest.OnUpdate(deltaTime);

		base.OnLogicUpdate(deltaTime);
		// 플레이어 죽음
		if (CheckEnd())
		{
			return;
		}

		SpawnUpdate(deltaTime);
		elapsedTime += deltaTime;
	}

	private void SpawnUpdate(float time)
	{
		if (_StageManager.it.bossSpawn)
		{
			return;
		}
		if (spawnTime == 0)
		{
			//SpawnManager.it.SpawnEnemies(StageManager.it.CurrentStage.SpawnMinDistance, StageManager.it.CurrentStage.SpawnMaxDistance);
		}
		spawnTime += time;
		if (spawnTime > _StageManager.it.CurrentStage.SpawnInterval)
		{
			spawnTime = 0;
		}
	}
}
