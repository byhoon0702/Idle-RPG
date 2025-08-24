using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalStageResult : ResultCondition
{
	public override bool IsWin()
	{
        return true;
	}

	public override bool IsLose()
	{
        return false;
	}
}
