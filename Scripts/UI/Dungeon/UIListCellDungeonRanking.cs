using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.Services.Leaderboards.Models;

public class UIListCellDungeonRanking : MonoBehaviour
{
	[SerializeField] private TextMeshProUGUI textRank;
	[SerializeField] private TextMeshProUGUI textName;
	[SerializeField] private TextMeshProUGUI textScore;

	public void OnUpdate(LeaderboardEntry entry)
	{
		if (entry == null)
		{
			textRank.text = "-";
			textName.text = "----";
			textScore.text = "---";
			return;
		}
		textRank.text = $"{entry.Rank + 1}";
		textName.text = entry.PlayerName;
		textScore.text = entry.Score.ToString();
	}
}
