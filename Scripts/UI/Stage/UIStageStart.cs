using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIStageStart : MonoBehaviour
{
	[SerializeField] private CanvasGroup canvasGroup;
	[SerializeField] private TextMeshProUGUI textTitle;
	[SerializeField] private TextMeshProUGUI textStageNumber;
	[SerializeField] private RectTransform maskRecttransform;

	[SerializeField] private Vector2 size;
	public void Activate()
	{
		gameObject.SetActive(true);
		maskRecttransform.sizeDelta = new Vector2(0, size.y);

		textTitle.text = _StageManager.it.CurrentStage.StageName;
		switch (_StageManager.it.CurrentStage.StageType)
		{
			case StageType.Normal:
				textStageNumber.gameObject.SetActive(true);
				textStageNumber.text = $"{_StageManager.it.CurrentStage.StageNumber}";
				break;
			case StageType.Dungeon:
			case StageType.Tower:
				textStageNumber.gameObject.SetActive(true);
				textStageNumber.text = $"{_StageManager.it.CurrentStage.StageNumber}";
				break;
			default:
				textStageNumber.gameObject.SetActive(false);
				break;
		}

		canvasGroup.alpha = 1.0f;
		maskRecttransform.DOSizeDelta(size, 0.4f).OnComplete(() =>
		{
			canvasGroup.DOFade(0, 0.4f).SetDelay(0.5f).OnComplete(() =>
			{
				gameObject.SetActive(false);
			});
		});
	}

	public void OnDisable()
	{

	}


}
