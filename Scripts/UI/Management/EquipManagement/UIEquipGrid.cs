using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIEquipGrid : UIBaseGrid<RuntimeData.EquipItemInfo>

{
	public Transform GetChild(int index)
	{
		var go = itemRoot.GetChild(index);

		return go;
	}
	public override void Init(UIManagementEquip _parent)
	{
		parent = _parent;
	}

	public override void OnUpdate(List<RuntimeData.EquipItemInfo> itemList)
	{
		var list = itemList;
		itemRoot.CreateListCell(list.Count, itemPrefab);

		for (int i = 0; i < itemRoot.childCount; i++)
		{

			var child = itemRoot.GetChild(i);
			if (i > list.Count - 1)
			{
				child.gameObject.SetActive(false);
				continue;
			}

			child.gameObject.SetActive(true);
			UIEquipSlot slot = child.GetComponent<UIEquipSlot>();

			var info = list[i];
			slot.OnUpdate(parent, info, () =>
			{
				parent.SelectEquipItem(info.Tid);
				parent.UpdateInfo();
				parent.OnUpdateEquip(info.type, info.Tid);
			});
		}

		int index = list.FindIndex(x => x.Tid == parent.selectedItemTid);
		int line = index / 4;
		int totalLine = list.Count / 4;


		scroll.verticalNormalizedPosition = Mathf.Clamp01(1f - ((float)line / (float)totalLine));

	}
}
