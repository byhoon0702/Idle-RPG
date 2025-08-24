using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillObject : MonoBehaviour
{
	private SkillCore _skillInstance;
	public void Initialize(SkillCore core, Unit caster, RuntimeData.SkillInfo info, AffectedInfo hitInfo)
	{
		_skillInstance = Instantiate(core);
	}

	// Update is called once per frame
	void Update()
	{

	}
}
