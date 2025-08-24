using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SkillBehavior : ScriptableObject
{
	public abstract List<Vector2> GetPosition(Vector2 center);

}
