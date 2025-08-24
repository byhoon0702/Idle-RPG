using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Rendering;
using System.Linq;

public class UIItemGacha : MonoBehaviour
{

	[SerializeField] ContentType contentType;
	[SerializeField] private Material grayScale;
	[Header("Main")]
	[SerializeField] private Image imageMain;
	[SerializeField] private UITextMeshPro uiTextTitle;

	[Header("Reward")]
	[SerializeField] private GameObject objReward;
	[SerializeField] private TextMeshProUGUI textSlider;
	[SerializeField] private Slider slider;
	[SerializeField] private TextMeshProUGUI textLevel;
	[SerializeField] private Button buttonGachaReward;
	[SerializeField] private GameObject objGachaRewardOn;

	[Header("Gacha Button")]
	[SerializeField] private Button buttonTenGacha;
	public Button ButtonTenGacha => buttonTenGacha;
	[SerializeField] private Image imageTenCost;
	[SerializeField] private UITextMeshPro textTenLabel;
	[SerializeField] private TextMeshProUGUI textTenCost;

	[SerializeField] private Button buttonThirtyGacha;
	public Button ButtonThirtyGacha => buttonThirtyGacha;
	[SerializeField] private Image imageThirtyCost;
	[SerializeField] private UITextMeshPro textThirtyLabel;
	[SerializeField] private TextMeshProUGUI textThirtyCost;

	[SerializeField] private Button buttonADGacha;
	public Button ButtonADGacha => buttonADGacha;
	[SerializeField] private Image imageAdsCost;
	[SerializeField] private UITextMeshPro textAdsLabel;
	[SerializeField] private TextMeshProUGUI textAdsCount;

	[SerializeField] private Button buttonChanceInfo;


	private RuntimeData.GachaInfo gachaInfo;
	public RuntimeData.GachaInfo GachaInfo => gachaInfo;
	private UIManagementGacha parent;


	RuntimeData.CurrencyInfo currency10;
	RuntimeData.CurrencyInfo currency100;

	GachaDataSummonInfo gacha10Info;
	GachaDataSummonInfo gacha100Info;

	private System.Action<RuntimeData.GachaInfo> _onClickInfo;

	private void Awake()
	{
		buttonTenGacha.onClick.AddListener(OnClick10);
		buttonThirtyGacha.onClick.AddListener(OnClick30);
		buttonADGacha.onClick.AddListener(OnClickAds);
		buttonChanceInfo.onClick.AddListener(OnClickGachaInfo);
		buttonGachaReward.onClick.AddListener(OnClickGachaReward);
	}

	private void OnDisable()
	{
		gachaInfo.OnLevelUp -= OnUpdateReward;
	}

	private void OnUpdateReward()
	{
		if (gachaInfo.currentLevelInfo != null)
		{
			objReward.SetActive(true);
			bool isEmpty = gachaInfo.currentLevelInfo.reward.tid == 0;
			buttonGachaReward.interactable = isEmpty == false;

			textLevel.text = $"Lv. {gachaInfo.Level}";



			if (gachaInfo.Level >= gachaInfo.MaxLevel)
			{
				textSlider.text = "Max";
				slider.value = 1f;
			}
			else
			{
				textSlider.text = $"{gachaInfo.Exp}/{gachaInfo.currentLevelInfo.exp}";
				slider.value = gachaInfo.Exp / (float)gachaInfo.currentLevelInfo.exp;
			}
			objGachaRewardOn.SetActive(isEmpty == false && gachaInfo.CanGetReward());
		}
		else
		{
			objReward.SetActive(false);
		}
	}

	bool _isOpen;
	string _contentMessage;
	public void OnUpdate(UIManagementGacha _parent, RuntimeData.GachaInfo _gachaInfo, System.Action<RuntimeData.GachaInfo> onClickInfo)
	{
		parent = _parent;
		gachaInfo = _gachaInfo;

		var content = PlatformManager.UserDB.contentsContainer.Get(contentType);
		_isOpen = content != null ? content.IsOpen : false;
		_contentMessage = content != null ? content.Description : "";

#if UNITY_EDITOR
		_isOpen = PlatformManager.ConfigMeta.CheckContent == false ? true : _isOpen;
#endif

		_onClickInfo = onClickInfo;
		gachaInfo.OnLevelUp += OnUpdateReward;
		uiTextTitle.SetKey(gachaInfo.rawData.name);
		imageMain.sprite = gachaInfo.IconImage;
		if (_isOpen)
		{
			imageMain.material = null;
		}
		else
		{
			imageMain.material = grayScale;
		}

		OnUpdateReward();

		gacha10Info = gachaInfo.gacha10;
		gacha100Info = gachaInfo.gacha100;

		if (parent.methodType == GachaMethodType.NORMAL)
		{
			currency10 = PlatformManager.UserDB.inventory.FindCurrency(gachaInfo.gacha10.itemTid);
			currency100 = PlatformManager.UserDB.inventory.FindCurrency(gachaInfo.gacha100.itemTid);
		}
		else
		{
			gacha10Info = gachaInfo.gachaTicket10;
			gacha100Info = gachaInfo.gachaTicket100;
			currency10 = PlatformManager.UserDB.inventory.FindCurrency(gachaInfo.gachaTicket10.itemTid);
			currency100 = PlatformManager.UserDB.inventory.FindCurrency(gachaInfo.gachaTicket100.itemTid);
		}


		imageTenCost.sprite = currency10.itemObject.ItemIcon;
		imageThirtyCost.sprite = currency100.itemObject.ItemIcon;

		textTenLabel.SetKey("str_ui_10_gacha");
		textThirtyLabel.SetKey("str_ui_30_gacha");
		textAdsLabel.SetKey("str_ui_ads");

		textTenCost.text = $"{gacha10Info.cost}";
		textThirtyCost.text = $"{gacha100Info.cost}";

		textTenCost.color = currency10.Check(gacha10Info.cost) ? Color.white : Color.red;
		textThirtyCost.color = currency100.Check(gacha100Info.cost) ? Color.white : Color.red;

		buttonADGacha.gameObject.SetActive(gachaInfo.gachaAds != null);
		if (gachaInfo.gachaAds != null)
		{
			int count = Mathf.Max(gachaInfo.gachaAds.summonMaxCount - gachaInfo.ViewAdsCount, 0);
			textAdsCount.text = $"{count}/{gachaInfo.gachaAds.summonMaxCount}";
			if (count == 0)
			{
				textAdsCount.color = Color.red;
			}
			else
			{
				textAdsCount.color = Color.white;
			}
		}

	}

	public void OnClick10()
	{
		if (_isOpen == false)
		{
			ToastUI.Instance.Enqueue(_contentMessage);
			return;
		}

		if (currency10.Pay(gacha10Info.cost) == false)
		{
			ToastUI.Instance.EnqueueKey("str_ui_warn_lack_of_currency");
			return;
		}
		gachaInfo.OnClickGacha(GachaButtonType.Gacha10, parent.methodType);
		parent.OnUpdate();
	}

	public void OnClick30()
	{
		if (_isOpen == false)
		{
			ToastUI.Instance.Enqueue(_contentMessage);
			return;
		}
		if (currency100.Pay(gacha100Info.cost) == false)
		{
			ToastUI.Instance.EnqueueKey("str_ui_warn_lack_of_currency");
			return;
		}
		gachaInfo.OnClickGacha(GachaButtonType.Gacha100, parent.methodType);

		parent.OnUpdate();
	}

	public void OnClickAds()
	{
		if (_isOpen == false)
		{
			ToastUI.Instance.Enqueue(_contentMessage);
			return;
		}


		int count = gachaInfo.gachaAds.summonMaxCount - gachaInfo.ViewAdsCount;
		if (count <= 0)
		{
			ToastUI.Instance.Enqueue(PlatformManager.Language["str_ui_no_more_ads_gacha"]);
			return;
		}

		var item = PlatformManager.UserDB.inventory.GetPersistent(InventoryContainer.AdFreeTid);
		bool free = item.unlock;
		if (free)
		{
			gachaInfo.OnClickGachaAds();
			gachaInfo.OnClickGacha(GachaButtonType.Ads, parent.methodType);
			parent.OnUpdate();
			return;
		}

		MobileAdsManager.Instance.ShowAds(() =>
		{
			gachaInfo.OnClickGachaAds();
			gachaInfo.OnClickGacha(GachaButtonType.Ads, parent.methodType);
			parent.OnUpdate();
		});

	}

	public void OnClickGachaInfo()
	{
		_onClickInfo?.Invoke(gachaInfo);
	}

	public void OnClickGachaReward()
	{
		if (_isOpen == false)
		{
			ToastUI.Instance.Enqueue(_contentMessage);
			return;
		}
		gachaInfo.OnReceiveReward();
	}
}
