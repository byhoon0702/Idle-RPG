using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIBaseGrid<T> : MonoBehaviour
{
	[SerializeField] protected ScrollRect scroll;
	[SerializeField] protected GameObject itemPrefab;

	[SerializeField] protected Transform itemRoot;

	protected UIManagementEquip parent;

	public abstract void Init(UIManagementEquip _parent);
	public abstract void OnUpdate(List<T> itemList);

}
