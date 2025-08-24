using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Leaderboards.Models;

public class RankingDataSource : LoopScrollDataSource, LoopScrollPrefabSource
{
	public Transform parent;

	public List<LeaderboardEntry> rankingInfos = new List<LeaderboardEntry>();
	public GameObject prefab;


	Stack<Transform> pool = new Stack<Transform>();
	public GameObject GetObject(int index)
	{
		if (pool.Count == 0)
		{
			return Object.Instantiate(prefab);
		}

		Transform candidate = pool.Pop();
		candidate.gameObject.SetActive(true);
		return candidate.gameObject;
	}
	public void ProvideData(Transform transform, int index)
	{
		UIListItemRanking ranking = transform.GetComponent<UIListItemRanking>();
		ranking.SetData(rankingInfos[index]);
		ranking.gameObject.SetActive(true);
	}

	public void ReturnObject(Transform transform)
	{
		transform.SendMessage("ScrollCellReturn", SendMessageOptions.DontRequireReceiver);
		transform.gameObject.SetActive(false);
		transform.SetParent(parent, false);
		pool.Push(transform);
	}
}


public class UIPopupRanking : UIBase
{

	[SerializeField] private UIListItemRanking myRanking;

	[SerializeField] private TextMeshProUGUI textNoRanking;

	[SerializeField] private LoopVerticalScrollRect loopScrollRect;

	[SerializeField] private GameObject prefab;

	private RankingDataSource dataSource;

	public LeaderboardEntry CreateMyScore(double score)
	{
		var userinfo = PlatformManager.UserDB.userInfoContainer.userInfo;
		var mine = new LeaderboardEntry(userinfo.UUID, userinfo.UserName, 100, score);
		return mine;
	}
	public async void OpenStageRank()
	{
		var page = await PlatformManager.Instance.LeaderBoard.GetStageScores();
		var mine = CreateMyScore(PlatformManager.UserDB.stageContainer.LastPlayedNormalStage().StageNumber);

		if (Activate())
		{
			SetData(page, mine);
		}
	}


	public void SetData(LeaderboardScoresPage page, LeaderboardEntry myScore)
	{
		int count = page.Results.Count;

		if (count == 0)
		{
			loopScrollRect.gameObject.SetActive(false);
			textNoRanking.gameObject.SetActive(true);
			myRanking.gameObject.SetActive(false);
			return;
		}

		loopScrollRect.gameObject.SetActive(true);
		textNoRanking.gameObject.SetActive(false);
		myRanking.gameObject.SetActive(true);

		var mine = page.Results.Find(x => x.PlayerId == PlatformManager.UserDB.userInfoContainer.userInfo.UUID);
		if (mine == null)
		{
			mine = myScore;
		}

		myRanking.SetData(mine);

		dataSource = new RankingDataSource();
		dataSource.rankingInfos = page.Results;
		dataSource.prefab = prefab;
		dataSource.parent = loopScrollRect.transform;

		loopScrollRect.prefabSource = dataSource;
		loopScrollRect.dataSource = dataSource;
		loopScrollRect.totalCount = page.Results.Count;
		loopScrollRect.RefillCells();
	}
}
