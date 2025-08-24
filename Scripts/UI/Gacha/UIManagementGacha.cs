using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum GachaMethodType
{
	NORMAL,
	TICKET,
}


public class UIManagementGacha : UIBase
{
	[SerializeField] private UIPopupGachaInfo uiPopupGachaInfo;
	[SerializeField] private GameObject itemPrefab;
	[SerializeField] private Transform content;

	[SerializeField] private Toggle toggleNormal;

	[SerializeField] private Toggle toggleTicket;
	public GachaMethodType methodType { get; private set; }
	public UIItemGacha Find(GachaType type)
	{
		for (int i = 0; i < content.childCount; i++)
		{
			var item = content.GetChild(i).GetComponent<UIItemGacha>();
			if (item.GachaInfo.rawData.gachaType == type)
			{
				return item;
			}
		}
		return null;
	}


	public void OnClickNormal(bool isOn)
	{
		if (isOn)
		{
			methodType = GachaMethodType.NORMAL;
		}
		OnUpdate();
	}

	public void OnClickTicket(bool isOn)
	{
		if (isOn)
		{
			methodType = GachaMethodType.TICKET;
		}
		OnUpdate();
	}

	protected override void OnActivate()
	{
		toggleNormal.SetIsOnWithoutNotify(true);
		methodType = GachaMethodType.NORMAL;
		OnUpdate();
	}

	public void OnUpdate()
	{
		int make = PlatformManager.UserDB.gachaContainer.GachaInfos.Count - content.childCount;

		if (make > 0)
		{
			for (int i = 0; i < make; i++)
			{
				Instantiate(itemPrefab, content);
			}
		}

		for (int i = 0; i < content.childCount; i++)
		{
			Transform child = content.GetChild(i);

			if (i < PlatformManager.UserDB.gachaContainer.GachaInfos.Count)
			{
				UIItemGacha gacha = child.GetComponent<UIItemGacha>();
				gacha.OnUpdate(this, PlatformManager.UserDB.gachaContainer.GachaInfos[i], OpenGachaInfo);
			}
		}
	}

	protected override void OnClose()
	{
		base.OnClose();
		uiPopupGachaInfo.Close();
	}
	public void OpenGachaInfo(RuntimeData.GachaInfo info)
	{
		uiPopupGachaInfo.SetData(info);
		if (uiPopupGachaInfo.Activate())
		{

		}
	}
}
