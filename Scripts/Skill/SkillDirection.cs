using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillDirection : SkillBehavior
{
	public float angle;
	public int branch;
	public bool useInverse;

	const float rightAngle = 90f;

	public override List<Vector2> GetPosition(Vector2 direction)
	{
		List<Vector2> result = new List<Vector2>();
		float branchAngle = angle / Mathf.Max(1, branch - 1);
		float startAngle = rightAngle;
		int count = branch - 1;

		if (count > 0)
		{
			startAngle = rightAngle - (angle / 2);
		}

		Vector2 rightVector = direction.GetRightVector2();

		for (int i = 0; i < branch; i++)
		{
			float finalAngle = startAngle + (branchAngle * i);
			result.Add(rightVector.GetAngledVector2(finalAngle));
			if (useInverse)
			{
				result.Add(rightVector.GetAngledVector2(finalAngle, true));
			}
		}


		return result;
	}
}
