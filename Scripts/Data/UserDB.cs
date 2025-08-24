#if UNITY_EDITOR
#define IS_EDITOR
#endif


using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Newtonsoft.Json;
using RuntimeData;

using UnityEngine;
using System.Threading.Tasks;



[System.Serializable]
public class SerializedContainerDictionary : SerializableDictionary<System.Type, BaseContainer>
{
}


[System.Serializable]
public class SaveData
{
	public string type;
	public string json;

	[NonSerialized] private BaseContainer _container;
	public SaveData()
	{

	}
	public SaveData(Type _type, string _json)
	{
		type = _type.ToString().Split(',')[0];
		json = _json;
	}

	public SaveData Set<T>(T container) where T : BaseContainer
	{
		_container = container;
		type = _container.GetType().ToString().Split(',')[0]; ;
		return this;
	}

	public void UpdateData()
	{
		json = _container.Save();
	}
}

[System.Serializable]
public struct UserDBSave
{
	public static string directory
	{
		get
		{
#if SALES
				return "SalesSave";
#else
#if IS_EDITOR
			return $"{Application.dataPath}/../LocalSave";
#else
				return $"{Application.persistentDataPath}/LocalSave";
#endif
#endif
		}
	}
	public static string path
	{
		get
		{
#if SALES
				return $"{directory}/SalesSaveData";
#else
#if IS_EDITOR
			return $"{directory}/SaveData.bin";

#else
				return $"{directory}/SaveData.dat";
#endif
#endif
		}
	}

	//[SerializeField] public LoginInfo loginInfo;
	public List<SaveData> savedata;

	public Dictionary<string, object> dict;
	public const string k_LocalSave = "LocalSave";
	public const string k_LoginSave = "Login";
	public void Set(UserDB userDb)
	{
		//loginInfo = userDb.loginInfo;

		savedata = new List<SaveData>
		{
			////
			new SaveData().Set(userDb.userInfoContainer),
			new SaveData().Set(userDb.training),

			new SaveData().Set(userDb.skillContainer),
			////
			new SaveData().Set(userDb.stageContainer),
			////
			new SaveData().Set(userDb.inventory),
			////
	
			////
			new SaveData().Set(userDb.contentsContainer),
			////

			////
		};
	}

	public void DeleteLocalSave()
	{

		savedata.Clear();
		PlayerPrefs.DeleteKey(k_LocalSave);
	}

	public void UpdateDatas()
	{
		for (int i = 0; i < savedata.Count; i++)
		{
			savedata[i].UpdateData();
		}
	}

	public byte[] CloudSaveList()
	{
		string json = JsonUtility.ToJson(this, true);
		byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
		return bytes;
	}

	public void FileSave()
	{
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return;
		}
		UpdateDatas();

		string json = JsonUtility.ToJson(this, true);
		//byte[] bytes = System.Text.Encoding.UTF8.GetBytes(json);
		using (FileStream fs = File.Open(UserDBSave.path, FileMode.OpenOrCreate, FileAccess.ReadWrite))
		{
			using (BinaryWriter br = new BinaryWriter(fs))
			{
				br.Write(json);
				br.Close();
			}
			fs.Close();
		}
	}

	public string Save()
	{
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return "";
		}

		JsonSerializerSettings settings = new JsonSerializerSettings();
		settings.Formatting = Formatting.None;
		var json = JsonConvert.SerializeObject(this, settings);
		PlayerPrefs.SetString(k_LocalSave, json);
		PlayerPrefs.Save();
		return json;
	}

	public void Load(UserDB userDB)
	{
		//bool loadComplete = false;
		//if (File.Exists(path))
		//{
		//	using (FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read))
		//	{
		//		using (BinaryReader br = new BinaryReader(fs))
		//		{
		//			string jsosn = br.ReadString();
		//			LoadData(userDB, jsosn);
		//			loadComplete = true;
		//			br.Close();
		//		}
		//		fs.Close();
		//	}
		//}

		//if (loadComplete)
		//{
		//	return;
		//}

		if (PlayerPrefs.HasKey(k_LocalSave) == false)
		{
			return;
		}
		string json = PlayerPrefs.GetString(k_LocalSave);

		LoadData(userDB, json);
	}

	public void LoadUserInfoOnly(UserDB userDB, string json)
	{
		UserDBSave data = JsonConvert.DeserializeObject<UserDBSave>(json);
		System.Type type = userDB.GetType();
		var fields = type.GetField("userInfoContainer");


		foreach (var savedata in data.savedata)
		{
			if (savedata.type.Contains("UserInfoContainer"))
			{
				Type datatype = System.Type.GetType($"{savedata.type}");
				if (datatype == null)
				{
					datatype = System.Type.GetType($"{savedata.type}, Assembly-CSharp");
				}
				MethodInfo methodinfo = datatype.GetMethod("FromJson");
				if (methodinfo == null)
				{
					break;
				}
				try
				{
					methodinfo.Invoke(fields.GetValue(userDB), new object[] { (string)savedata.json });
				}
				catch (Exception e)
				{
					Debug.LogError(e);
					PlatformManager.UserDB._error_on_load = true;
				}
				break;
			}
		}
	}

	public void LoadData(UserDB userDB, string json)
	{
		Debug.Log($"Json Data: {json}");
		//UserDBSave data = JsonConvert.DeserializeObject<UserDBSave>(json);
		UserDBSave data = JsonUtility.FromJson<UserDBSave>(json);
		System.Type type = userDB.GetType();
		var fields = type.GetFields();
		for (int i = 0; i < fields.Length; i++)
		{

			foreach (var savedata in data.savedata)
			{
				if (fields[i].FieldType.ToString().Contains(savedata.type))
				{
					MethodInfo methodinfo = fields[i].FieldType.GetMethod("FromJson");
					if (methodinfo == null)
					{
						break;
					}
					try
					{
						methodinfo.Invoke(fields[i].GetValue(userDB), new object[] { (string)savedata.json });
					}
					catch (Exception e)
					{
						Debug.LogError(e);
						PlatformManager.UserDB._error_on_load = true;
					}
					break;
				}
			}
		}
	}

	//public void DeleteFile()
	//{
	//	File.Delete(path);
	//}


}


[System.Serializable]
public class ScriptableDictionary : SerializableDictionary<long, ScriptableObject>
{ }


public abstract class BaseContainer 
{

	protected ScriptableDictionary scriptableDictionary;
	protected UserDB parent;
	public abstract void Load(UserDB _parent);

	/// <summary>
	/// 데이터 로드가 끝난후 데이터 갱신
	/// </summary>
	public abstract void UpdateData();

	public abstract void DailyResetData();

	public abstract void LoadScriptableObject();


	public void LoadListTidMatch<T>(ref List<T> origin, List<T> saved) where T : BaseInfo
	{
		if (saved == null)
		{
			return;
		}

		for (int i = 0; i < origin.Count; i++)
		{

			if (i < saved.Count)
			{
				if (saved[i] == null)
				{ continue; }
				long tid = origin[i].Tid;
				origin[i].Load(saved.Find(x => x.Tid == tid));
			}
		}
	}


	public void LoadListIndexMatch<T>(ref T[] origin, T[] saved) where T : BaseInfo
	{
		if (saved == null)
		{
			return;
		}

		for (int i = 0; i < origin.Length; i++)
		{
			if (i < saved.Length)
			{

				if (saved[i] == null)
				{ continue; }
				origin[i].Load(saved[i]);
			}
		}
	}

	public void LoadListIndexMatch<T>(ref List<T> origin, List<T> saved) where T : BaseInfo
	{
		if (saved == null)
		{
			return;
		}

		for (int i = 0; i < origin.Count; i++)
		{
			if (i < saved.Count)
			{

				if (saved[i] == null)
				{ continue; }
				origin[i].Load(saved[i]);
			}
		}
	}

	public virtual void GetScriptableObject<T>(long tid, out T outObject) where T : ScriptableObject
	{
		outObject = null;

		if (scriptableDictionary.ContainsKey(tid) == false)
		{
			return;
		}

		outObject = scriptableDictionary[tid] as T;
	}

	public virtual T GetScriptableObject<T>(long tid) where T : ScriptableObject
	{
		if (scriptableDictionary.ContainsKey(tid) == false)
		{
			return null;
		}

		return scriptableDictionary[tid] as T;
	}

	protected void AddDictionary<T>(ScriptableDictionary dict, T[] datas) where T : ScriptableObject
	{
		for (int i = 0; i < datas.Length; i++)
		{
			var data = datas[i];
			string[] split = data.name.Split('_');
			long tid = long.Parse(split[1]);
			dict.Add(tid, data);
		}
	}


	protected virtual void SetItemListRawData<T1, T2>(ref List<T1> infolist, T2 _data) where T1 : BaseInfo, new() where T2 : BaseData
	{
		if (infolist == null)
		{
			infolist = new List<T1>();
		}
		infolist.Clear();
		if (infolist.Count == 0)
		{
			infolist = new List<T1>();
			T1 data = new T1();
			data.SetRawData(_data);
			infolist.Add(data);
			return;
		}

		for (int i = 0; i < infolist.Count; i++)
		{
			infolist[i].SetRawData(_data);
		}

	}

	protected virtual void SetListRawData<T1, T2>(ref List<T1> infolist, List<T2> datas) where T1 : IDataInfo, new() where T2 : BaseData
	{
		if (infolist == null)
		{
			infolist = new List<T1>();
		}
		infolist.Clear();
		if (infolist.Count == 0)
		{
			infolist = new List<T1>();
			for (int i = 0; i < datas.Count; i++)
			{
				T1 data = new T1();
				data.SetRawData(datas[i]);
				infolist.Add(data);
			}
			return;
		}

		for (int i = 0; i < infolist.Count; i++)
		{
			infolist[i].SetRawData(datas[i]);
		}
	}
	public abstract string Save();

	public abstract void FromJson(string json);

	public abstract void Dispose();

}

public class UserDB
{
	public const float killLimit = 1000f;
	public List<StatusData> statusDataList { get; private set; }
	public UnitStats UserStats { get; private set; }
	public Dictionary<StatsType, RuntimeData.AbilityInfo> HyperStats = new Dictionary<StatsType, RuntimeData.AbilityInfo>();

	/// <summary>
	/// 각종 정보 집합체
	/// </summary>
	#region Container
	public UserInfoContainer userInfoContainer;
	public TrainingContainer training;
	public SkillContainer skillContainer;
	public EquipContainer equipContainer;

    public InventoryContainer inventory;

	public StageContainer stageContainer;
	public GachaContainer gachaContainer;
	public ContentsContainer contentsContainer;
	public ShopContainer shopContainer;

	#endregion

	public string UUID
	{
		get
		{
			if (userInfoContainer == null || userInfoContainer.userInfo == null)
			{
				return "";
			}
			return userInfoContainer.userInfo.UUID;
		}
	}
	public UserDBSave saveData = new UserDBSave();


	public bool CanDataSave { get; private set; }
	public System.Random rewardChance { get; private set; } = new System.Random(21);
	public bool _error_on_load = false;

	public void OnUpdateContainer()
	{

	}
	public void SetLoginInfo(string uuid, string nickname, string platform)
	{

		if (uuid.IsNullOrEmpty())
		{
			return;
		}

		userInfoContainer?.SetAccountInfo(nickname, uuid);
		userInfoContainer?.SetUserInfo("", 1, (IdleNumber)0, (IdleNumber)0);

	}

	public UserDB LoadLocalSave()
	{
		UserDBSave save = new UserDBSave();
		UserDB local = new UserDB();
		bool load = false;
		if (File.Exists(UserDBSave.path))
		{
			using (FileStream fs = File.Open(UserDBSave.path, FileMode.Open, FileAccess.Read))
			{
				using (BinaryReader br = new BinaryReader(fs))
				{
					string jsosn = br.ReadString();
					local.InitializeContainer();
					save.LoadUserInfoOnly(local, jsosn);
					load = true;
					br.Close();
				}
				fs.Close();
			}
		}
		if (load)
		{
			return local;
		}

		if (PlayerPrefs.HasKey(UserDBSave.k_LocalSave))
		{
			string json = PlayerPrefs.GetString(UserDBSave.k_LocalSave);

			local.InitializeContainer();
			save.LoadUserInfoOnly(local, json);
			return local;
		}
		return null;
	}

	public async Task<bool> LoadLoginData()
	{
#if UNITY_EDITOR
		if (PlatformManager.Instance.overrideJson)
		{

			string json = JsonConvert.DeserializeObject<string>(PlatformManager.Instance.jsonText.text);
			PlayerPrefs.SetString(UserDBSave.k_LocalSave, json);
			PlayerPrefs.Save();
			return true;
		}
#endif
		//var newcloudJson = await PlatformManager.RemoteSave.CloudData();

		var cloudJson = await PlatformManager.RemoteSave.OldCloudData();
		UserDB cloudDb = null;
		if (cloudJson.IsNullOrEmpty() == false)
		{
			UserDBSave saveData = new UserDBSave();
			cloudDb = new UserDB();
			cloudDb.InitializeContainer();
			saveData.LoadUserInfoOnly(cloudDb, cloudJson);
		}

		var localData = LoadLocalSave();

		UserDBSave userDbSave = new UserDBSave();
		if (localData != null)
		{
			PlatformManager.UserDB.SetLoginInfo(localData.userInfoContainer.userInfo.UUID, localData.userInfoContainer.userInfo.UserName, localData.userInfoContainer.userInfo.LoginPlatform);
		}

		if (cloudDb != null)
		{
			if (localData == null)
			{
				PlayerPrefs.SetString(UserDBSave.k_LocalSave, cloudJson);
				userDbSave.LoadUserInfoOnly(PlatformManager.UserDB, cloudJson);
				return true;
			}
			if (cloudDb.userInfoContainer.userInfo.UUID == localData.userInfoContainer.userInfo.UUID)
			{
				if (cloudDb.userInfoContainer.userInfo.UserLevel > localData.userInfoContainer.userInfo.UserLevel)
				{
					PlayerPrefs.SetString(UserDBSave.k_LocalSave, cloudJson);
					userDbSave.LoadUserInfoOnly(PlatformManager.UserDB, cloudJson);

					return true;
				}
				string json = PlayerPrefs.GetString(UserDBSave.k_LocalSave);
				if (json.IsNullOrEmpty())
				{
					PlayerPrefs.SetString(UserDBSave.k_LocalSave, cloudJson);
				}
			}
			userDbSave.LoadUserInfoOnly(PlatformManager.UserDB, cloudJson);
			return true;
		}
		if (cloudJson.IsNullOrEmpty())
		{

			PlayerPrefs.DeleteKey(UserDBSave.k_LocalSave);
		}
		else
		{
			userDbSave.LoadUserInfoOnly(PlatformManager.UserDB, cloudJson);
			PlayerPrefs.SetString(UserDBSave.k_LocalSave, cloudJson);
		}

		return true;
	}


	public void LogOut()
	{
		Dispose();
		PlayerPrefs.DeleteKey(UserDBSave.k_LoginSave);
		saveData.DeleteLocalSave();
	}


	public IdleNumber GetValue(StatsType type)
	{
		return UserStats.GetValue(type);
	}
	public IdleNumber GetBaseValue(StatsType type)
	{
		return UserStats.GetBaseValue(type);
	}


	public void Dispose()
	{
		userInfoContainer?.Dispose();
		training?.Dispose();
		
		skillContainer?.Dispose();
	
		inventory?.Dispose();

		stageContainer?.Dispose();
		
		contentsContainer?.Dispose();
		shopContainer?.Dispose();
	

	}

	public void Clear()
	{


	}

	public Dictionary<string, object> CloudSave()
	{
		Dictionary<string, object> data = new Dictionary<string, object>();
		if (PlatformManager.Instance.overrideJson)
		{
			return data;
		}
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return data;
		}
		if (CanDataSave == false)
		{
			return data;
		}
		//data = saveData.Save();
		return data;

	}


	public byte[] CloudFileSave()
	{
		if (PlatformManager.Instance.overrideJson)
		{
			return null;
		}
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return null;
		}
		if (CanDataSave == false)
		{
			return null;
		}

		return saveData.CloudSaveList();
	}

	public string Save()
	{
		if (PlatformManager.Instance.overrideJson)
		{
			return "";
		}
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return "";
		}
		if (CanDataSave == false)
		{
			return "";
		}

		saveData.UpdateDatas();
		return saveData.Save();
	}

	public void LoadFromCloud(Dictionary<string, string> jsonDict)
	{
		foreach (var data in jsonDict.Values)
		{
			//for (int i = 0; i < data)
		}
	}

	public void NewSave()
	{
		if (PlatformManager.Instance.overrideJson)
		{
			return;
		}
		if (ExceptionManager.Instance.ExceptionOccurred)
		{
			return;
		}
		if (CanDataSave == false)
		{
			return;
		}

		saveData.FileSave();
	}

	public void Load()
	{
		saveData.Load(this);
	}

	public void SetUnitData()
	{
		statusDataList = new List<StatusData>();

		var list = DataManager.Get<StatusDataSheet>().GetInfosClone();
		for (int i = 0; i < list.Count; i++)
		{
			statusDataList.Add(list[i]);
		}
	}

	public void InitializeContainer()
	{
		UserStats = new UnitStats();
		UserStats.Generate();
		LoadContainer(ref userInfoContainer);
	}

	public void Init()
	{
		CanDataSave = false;
		_error_on_load = false;
		try
		{
			LoadContainer(ref training);
		
			LoadContainer(ref inventory);

			LoadContainer(ref skillContainer);
			
			LoadContainer(ref stageContainer);
			


			
			LoadContainer(ref contentsContainer);

			LoadContainer(ref shopContainer);
			

			saveData.Set(this);
			Load();
			SetUnitData();
			InitDatas();

			VLog.Log("Data Load 성공");
			RemoteConfigManager.Instance.UpdateUserLevel(userInfoContainer.userInfo.UserLevel);
			RemoteConfigManager.Instance.UpdateStageLevel(stageContainer.LastPlayedNormalStage().StageNumber);

			CanDataSave = true;

		}
		catch (Exception ex)
		{
			VLog.LogError($"데이터 로딩중 오류발생!!!\n{ex.StackTrace}");
			_error_on_load = true;
			CanDataSave = false;
		}
	}

	public void LoadContainer<T>(ref T container) where T : BaseContainer, new()
	{
		if (container == null)
		{
			container = new T();
		}

		container.Load(this);
	}

	public void AddModifiers(StatsType type, StatsModifier modifier)
	{
		UserStats.AddModifier(type, modifier);
	}

	public void UpdateModifiers(StatsType type, StatsModifier modifier)
	{
		UserStats.UpdataModifier(type, modifier);
	}

	public void RemoveModifiers(StatsType type, object source)
	{
		UserStats.RemoveModifier(type, source);
	}

	public void RemoveAllModifiers(object source)
	{
		UserStats.RemoveAllModifiers(source);
	}

	public void RemoveAllModifiers(StatModeType type)
	{
		UserStats.RemoveAllModifiers(type);
	}

	private void OnUpdateData<T>(T container) where T : BaseContainer
	{
		//VLog.Log(container.GetType() + " UpdateData");
		container.UpdateData();
	}

	public void InitDatas()
	{
		Debug.Log("Init Data");
		OnUpdateData(stageContainer);
		OnUpdateData(contentsContainer);
		OnUpdateData(training);
		
		OnUpdateData(skillContainer);
		OnUpdateData(inventory);
		
		UpdateUserStats();
	}

	private void OnResetData<T>(T container) where T : BaseContainer
	{
		container.DailyResetData();
	}

	/// <summary>
	/// 새벽 5시 이후로 초기화가 필요한 데이터들을 이곳에서 초기화 한다.
	/// </summary>
	public void ResetDataByDateTime()
	{
		VLog.Log("!!!!!!!==========초기화 성공==========!!!!!!!!");
		OnResetData(userInfoContainer);
		OnResetData(inventory);
		OnResetData(shopContainer);
	
	}

	public void UpdateUserStats()
	{
		UserStats.UpdateAll();
	}

	public void AddRewards(List<RuntimeData.RewardInfo> rewardList, bool displayReward, bool showToast = false)
	{
		InternalAddStageRewards(rewardList, displayReward, showToast);
	}


	private void InternalAddStageRewards(List<RuntimeData.RewardInfo> rewardList, bool displayReward, bool showToast = false)
	{
		List<AddItemInfo> infoList = new List<AddItemInfo>();
		for (int i = 0; i < rewardList.Count; i++)
		{
			var reward = rewardList[i];
			infoList.Add(new AddItemInfo(reward));

		}
		AddRewards(infoList, displayReward, showToast);

	}

	public void AddRewards(List<AddItemInfo> rewardList, bool displayReward, bool showToast = false)
	{
		//for (int i = 0; i < rewardList.Count; i++)
		//{
		//	var reward = rewardList[i];
		//	switch (reward.category)
		//	{
		//		case RewardCategory.Equip:
		//			{
		//				equipContainer.AddEquipItem(reward.tid, reward.value.GetValueToInt());
		//			}
		//			break;
		//		case RewardCategory.Pet:
		//			{
		//				petContainer.AddPetItem(reward.tid, reward.value.GetValueToInt());
		//			}
		//			break;
		//		case RewardCategory.Skill:
		//			{
		//				skillContainer.AddSkill(reward.tid, reward.value.GetValueToInt());
		//			}
		//			break;
		//		case RewardCategory.EXP:
		//			{
		//				userInfoContainer.GainUserExp(reward.value);
		//			}
		//			break;
		//		case RewardCategory.Costume:
		//			{
		//				costumeContainer.Buy(reward.tid);
		//			}
		//			break;
		//		case RewardCategory.Event_Currency:
		//		case RewardCategory.Currency:
		//			{
		//				var data = DataManager.Get<CurrencyDataSheet>().Get(reward.tid);
		//				inventory.FindCurrency(data.tid).Earn(reward.value);
		//			}
		//			break;
		//		case RewardCategory.RewardBox:
		//			{
		//				//var list = RewardUtil.OpenRewardBox(new RewardInfo(reward.tid, reward.category, reward.value));
		//				//AddRewards(list, false);
		//			}
		//			break;
		//		case RewardCategory.Relic:
		//			{
		//				relicContainer.AddItem(reward.tid, reward.value.GetValueToInt());

		//			}
		//			break;
		//		case RewardCategory.Persistent:
		//			{
		//				var info = inventory.GetPersistent(reward.tid);
		//				info.unlock = true;
		//				displayReward = false;
		//				showToast = false;
		//			}
		//			break;
		//	}
		//}

		Save();
		DisplayReward(displayReward, showToast, rewardList);
		GameManager.it.CallAddRewardEvent();
	}

	protected void DisplayReward(bool displayReward, bool showToast, List<AddItemInfo> rewardList)
	{
		if (displayReward)
		{
			if (showToast)
			{
				GameUIManager.it.uiController.ShowRewardToast(rewardList);
			}
			else
			{
				GameUIManager.it.uiController.ShowRewardPopup(rewardList);

			}
		}
	}



	public void RemoveHyperAbility(object source)
	{
		foreach (var stats in HyperStats)
		{
			stats.Value.RemoveAllModifiersFromSource(source);
		}
	}

	public void AddHyperAbilityInfo(RuntimeData.AbilityInfo ability, object source)
	{

		if (HyperStats.ContainsKey(ability.type))
		{
			HyperStats[ability.type].AddModifiers(new StatsModifier(ability.Value, ability.modeType, source));
		}
		else
		{
			HyperStats.Add(ability.type, ability.Clone());
		}
	}

	public void AddHyperAbilityInfo(List<RuntimeData.AbilityInfo> abilities, object source)
	{
		for (int i = 0; i < abilities.Count; i++)
		{
			var ability = abilities[i];
			AddHyperAbilityInfo(ability, source);
		}
	}


}

