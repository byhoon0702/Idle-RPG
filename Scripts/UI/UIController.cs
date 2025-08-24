using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using RuntimeData;

public class UIController : MonoBehaviour
{
	private static UIController instance;
	public static UIController it
	{
		get
		{
			if (instance == null)
			{
				instance = GameObject.FindObjectOfType<UIController>();

			}
			return instance;
		}
	}

	[SerializeField] private UIStageInfo uiStageInfo;


	[Header("Bottoms")]
	[SerializeField] private UIManagement management;
	[SerializeField] private UIManagementEquip equipment;
	
	[SerializeField] private UIManagementShop shop;
	[SerializeField] private UIManagementGacha gacha;
	


	[Header("-------------------------")]
	[SerializeField] private UITraining training;
	[SerializeField] private UITopMoney topMoney;
	[SerializeField] private UserSkillUi hyperSkill;
	[SerializeField] private SkillGlobalUi skillGlobal;

	[SerializeField] private UIAdRewardChest adRewardChest;
	public UIAdRewardChest AdRewardChest => adRewardChest;
	//[SerializeField] private UIRewardLog uiRewardLog;
	[SerializeField] private UIPopupAcquiredRewardInfo uiPopupAcquiredRewardInfo;

	[SerializeField] private UIBottomMenu bottomMenu;

	[SerializeField] private UIPublicPopupRewardDisplay uiPopupRewardDisplay;
	[SerializeField] private UIPublicToastRewardDisplay uiToastRewardDisplay;
	[SerializeField] private UIPopupOfflineRewardDisplay uiPopupOfflineRewardDisplay;

	[SerializeField] private UIPopupStageSelect uiPopupStageSelect;
	[SerializeField] private UIPopupAttendance uiPopupAttendance;


	public UIBottomMenu BottomMenu => bottomMenu;
	public UIManagement Management => management;
	public UIManagementEquip Equipment => equipment;
	//public UIManagementSkill Skill => skill;
	//	public UIManagementPet Pet => pet;


	public UIStageInfo UiStageInfo => uiStageInfo;
	public UserSkillUi HyperSkill => hyperSkill;
	public SkillGlobalUi SkillGlobal => skillGlobal;

	private bool isCoinEffectActivated = true;



	private void Awake()
	{
		instance = this;
	}




	public void Init()
	{
		training.OnUpdate(false);

		topMoney.Init();
		adRewardChest.Init();

		
	}
	public void ShowAdRewardChest(bool isShow)
	{
		adRewardChest.Switch(isShow);
	}


	public void ShowDefeatNavigator()
	{
		// 패배시 강화컨텐츠 네비게이션
	}

	public void ShowCoinEffect(Transform _fromObject)
	{
		if (isCoinEffectActivated == false)
		{
			return;
		}

		Vector2 pos = GameUIManager.it.ToUIPosition(_fromObject.position);
	}

	public void ShowItemLog(bool isTrue)
	{
		if (isTrue)
		{
			uiPopupAcquiredRewardInfo.Show();
		}
		else
		{
			uiPopupAcquiredRewardInfo.Close();
		}
	}

	public void SetCoinEffectActivate(bool _isActive)
	{
		isCoinEffectActivated = _isActive;
	}

	public void ToggleManagement(System.Action onClose, UIManagement.ViewType view = UIManagement.ViewType.Training)
	{
		InactiveAllMainUI();
		if (management.gameObject.activeInHierarchy)
		{
			return;
		}

		if (management.Activate(onClose))
		{
			management.OnUpdate(view);
		}
	}

	public void ToggleEquipment(EquipTabType type = EquipTabType.WEAPON, long tid = 0, System.Action onClose = null)
	{
		InactiveAllMainUI();

		if (equipment.gameObject.activeInHierarchy)
		{

			return;
		}


		if (equipment.Activate(onClose))
		{
			equipment.OnUpdate(type, tid);
		}
	}

	public void TogglePet(System.Action onClose)
	{
		InactiveAllMainUI();
		
	}

	public void ToggleShop(System.Action onClose)
	{
		InactiveAllMainUI();
		if (shop.gameObject.activeInHierarchy)
		{
			return;
		}

		if (shop.Activate(onClose))
		{
			shop.OnUpdate(ShopType.PACKAGE);
		}
	}
	public void ShowDungeonList(System.Action onClose)
	{
		InactiveAllMainUI();
	

	}

	public void ToggleRelic(System.Action onClose)
	{
		InactiveAllMainUI();
	
	}

	public void ToggleGacha(System.Action onClose)
	{
		InactiveAllMainUI();
		if (gacha.gameObject.activeInHierarchy)
		{
			return;
		}

		if (gacha.Activate(onClose))
		{

		}

	}

	public void InactiveAllMainUI()
	{
		management.gameObject.SetActive(false);
		equipment.gameObject.SetActive(false);

		gacha.gameObject.SetActive(false);
		shop.gameObject.SetActive(false);

	}

	public void InactivateAllBottomToggle()
	{
		bottomMenu.InactivateAllToggle();
	}

	public void ShowMap()
	{

	}

	public void ShowBuffPage()
	{

	}

	public void ShowOfflineRewardPopup(List<AddItemInfo> rewardInfo, int totalMinutes, int totalKill)
	{
		uiPopupOfflineRewardDisplay.Show(rewardInfo);
		uiPopupOfflineRewardDisplay.SetInfo(totalMinutes, totalKill);
	}

	public void ShowRewardPopup(List<AddItemInfo> rewardInfo)
	{
		uiPopupRewardDisplay.Show(rewardInfo);
	}

	public void ShowRewardToast(List<AddItemInfo> rewardInfo)
	{
		uiToastRewardDisplay.Show(rewardInfo);
	}
	public void ShowStageSelect()
	{
		if (uiPopupStageSelect.Activate())
		{
			uiPopupStageSelect.Show();
		}
	}

	public void ShowQuest()
	{

	}
	public void ShowCollection()
	{

	}
	public void ShowAttendance()
	{
		if (uiPopupAttendance.Activate())
		{
			uiPopupAttendance.Open();
		}
	}
}
