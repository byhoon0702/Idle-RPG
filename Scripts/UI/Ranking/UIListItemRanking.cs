using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Services.Leaderboards.Models;
using TMPro;
public class UIListItemRanking : MonoBehaviour
{
	[SerializeField] private Sprite[] spriteRank;
	[SerializeField] private Image imageRankIcon;
	[SerializeField] private Image imageRankBg;
	[SerializeField] private TextMeshProUGUI textRank;
	[SerializeField] private TextMeshProUGUI textNickname;
	[SerializeField] private TextMeshProUGUI textScore;
	public void SetData(LeaderboardEntry entity)
	{
		imageRankIcon.enabled = true;
		if (entity.Rank < 3)
		{
			imageRankIcon.sprite = spriteRank[entity.Rank];
		}
		else if (entity.Rank >= 3 && entity.Rank < 50)
		{
			imageRankIcon.sprite = spriteRank[spriteRank.Length - 1];
		}
		else
		{
			imageRankIcon.enabled = false;
		}

		if (entity.Rank >= 100)
		{
			textRank.text = "-";
			textScore.text = $"STAGE ---";
		}
		else
		{
			textRank.text = $"{entity.Rank + 1}";
			textScore.text = $"STAGE {entity.Score}";
		}

		string[] names = entity.PlayerName.Split('#');
		if (name.Length > 1)
		{
			textNickname.text = names[0];
		}
		else
		{
			textNickname.text = entity.PlayerName;
		}
	}
}
