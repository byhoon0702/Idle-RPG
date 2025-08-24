using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UserSkillUi : MonoBehaviour
{

	[SerializeField] GameObject objHyperPhase;
	[SerializeField] private TextMeshProUGUI buttonAutoSkillText;
	[SerializeField] private TextMeshProUGUI buttonAutoHyperText;
	[SerializeField] private Image[] hyperModeGauge;
	[SerializeField] private Image[] hyperBreakGauge;

	[SerializeField] private Button buttonHyper;
	[SerializeField] private Toggle toggleAutoHyper;
	[SerializeField] private Toggle toggleAutoSkill;

	
	bool isHyper;
	private void Awake()
	{


		toggleAutoSkill.onValueChanged.RemoveAllListeners();
		toggleAutoSkill.onValueChanged.AddListener(OnClickAutoSkill);


		buttonHyper.onClick.RemoveAllListeners();
		buttonHyper.onClick.AddListener(OnClickHyper);
	}

	private void Start()
	{
		objHyperPhase.SetActive(false);
		foreach (var v in hyperModeGauge)
		{
			v.fillAmount = 0;
		}
		toggleAutoHyper.isOn = PlatformManager.UserDB.skillContainer.isAutoHyper;
		toggleAutoSkill.isOn = PlatformManager.UserDB.skillContainer.isAutoSkill;
	}


	public void SetHyperMode(bool isTrue)
	{
		isHyper = isTrue;
	}
	public void OnClickHyper()
	{
		
	}

	public void OnClickAutoSkill(bool isTrue)
	{
		PlatformManager.UserDB.skillContainer.isAutoSkill = isTrue;
		if (isTrue)
		{
			buttonAutoSkillText.color = Color.yellow;
			//PlatformManager.UserDB.questContainer.ProgressOverwrite(QuestGoalType.ACTIVATE_AUTO_SKILL, 0, (IdleNumber)1);
		}
		else
		{
			buttonAutoSkillText.color = Color.white;
		}
	}
	
	public void SetProgressHyperMode(float _ratio, float _value)
	{
		foreach (var v in hyperModeGauge)
		{
			v.fillAmount = _ratio;
		}

		
	}

	public void SetProgressHyperBreak(float _value)
	{
		foreach (var v in hyperBreakGauge)
		{
			v.fillAmount = _value;
		}
	}
}
