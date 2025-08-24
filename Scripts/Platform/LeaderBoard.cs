using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Leaderboards;
using Newtonsoft.Json;
using Unity.Services.Leaderboards.Models;
using System.Threading.Tasks;

public class LeaderBoard : MonoBehaviour
{
	public const string k_LeaderBoardId = "StageRating";
	public const string k_TowerId = "TowerRating";
	public const string k_TreasureId = "TreasureRanking";


	public async void AddStageScore()
	{
		var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(k_LeaderBoardId, PlatformManager.UserDB.stageContainer.LastPlayedNormalStage().StageNumber);

	}


	public async void AddScore(string boardId, double score)
	{
		var scoreResponse = await LeaderboardsService.Instance.AddPlayerScoreAsync(boardId, score);
	}

	public async Task<LeaderboardScoresPage> GetScores(string boardId, int limit = 100)
	{
		var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(boardId, new GetScoresOptions() { Offset = 0, Limit = limit });
		return scoreResponse;
	}

	public async Task<LeaderboardScoresPage> GetTreasureScores()
	{
		var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(k_TreasureId, new GetScoresOptions() { Offset = 0, Limit = 100 });
		return scoreResponse;
	}

	public async Task<LeaderboardScoresPage> GetStageScores()
	{
		var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(k_LeaderBoardId, new GetScoresOptions() { Offset = 0, Limit = 100 });
		return scoreResponse;

	}
	public async Task<LeaderboardScoresPage> GetTowerScores()
	{
		var scoreResponse = await LeaderboardsService.Instance.GetScoresAsync(k_TowerId, new GetScoresOptions() { Offset = 0, Limit = 100 });
		return scoreResponse;

	}
}
