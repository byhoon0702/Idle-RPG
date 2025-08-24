using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Debuff Skill", menuName = "ScriptableObject/Skill/Debuff", order = 1)]
public class Debuff : SkillCore
{
	public override bool Trigger(Unit _caster, RuntimeData.SkillInfo _info, System.Action _callback)
	{
		if (_caster == null)
		{
			return false;
		}

		//if (allowedUnitState != StateType.NONE)
		//{
		//	if (allowedUnitState.HasFlag(_caster.currentState) == false)
		//	{
		//		return false;
		//	}
		//}

		//DebuffInfo debuffinfo = new DebuffInfo(_caster.gameObject.layer, _info.Tid, _info.Duration, _info.skillAbility.type, _caster.HitInfo.TotalAttackPower);

		//if (_info.skillAbility.Value > 0)
		//{
		//	debuffinfo.power = _info.skillAbility.Value;
		//}
		//this._callback = _callback;
		//_caster.StartCoroutine(Activation(_caster, _info, debuffinfo));
		return true;
	}

	protected override IEnumerator AffectNonTarget(Unit caster, RuntimeData.SkillInfo info, AffectedInfo hitInfo, Vector3 targetPos)
	{
		GameObject effect = ShowEffect(caster, targetPos, info.EvolutionLevel);
		Vector3 pos = GetPos(caster, effect, targetPos);

		if (caster.IsAlive() == false)
		{
			yield break;
		}
		if (effectPosType == EffectPosType.Self)
		{
			pos = GetPos(caster, effect, targetPos);
		}

		OnAffectTasrget(caster, pos, info.HitRange, pos - caster.position, hitInfo, info);

		yield return null;

	}

	protected override void OnAffectTarget(Unit caster, Unit target, AffectedInfo hitInfo, float knockbackPower, Vector3 knockbackDir, RuntimeData.SkillInfo skillInfo)
	{
		DebuffTarget(caster, target, skillInfo, hitInfo as DebuffInfo);
	}



	private void DebuffTarget(Unit caster, Unit target, RuntimeData.SkillInfo skillInfo, DebuffInfo info)
	{
		SoundManager.Instance.PlayEffect(audioClip);
		//target.Debuff(info);
	}
}
