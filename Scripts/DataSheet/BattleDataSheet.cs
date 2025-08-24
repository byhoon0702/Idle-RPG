using System.Collections;
using System.Collections.Generic;
using UnityEngine;




[System.Serializable]
public class BattleData : BaseData
{
	public StageType stageType;

	public string leaderboardId;
	public long dungeonItemTid;
	public int itemCount;
	public ContentType contentType;
}

[System.Serializable]
public class BattleDataSheet : DataSheetBase<BattleData>
{


}
