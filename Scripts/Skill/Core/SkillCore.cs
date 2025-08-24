using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EffectPosType
{
	Target,
	Self,
	HitPosition,
	Effect,
	Center,
}

[Serializable]
public class SKillEvolutionData
{
	public int level;
	public GameObject effect;

	[Header("Camera")]
	public float amount;
	public float duration;
	public bool isFade;
	public bool useCameraShake;
}

public abstract class SkillCore : ScriptableObject
{
	[SerializeField] protected long tid;
	public long Tid => tid;
	[SerializeField] protected string description;
	public string Description => description;
	public Sprite Icon;

	public AudioClip audioClip;

	[SerializeField] protected GameObject obj;

	[SerializeField] protected StateType allowedUnitState = StateType.NONE;
	public StateType AllowedUnitState => allowedUnitState;

	public EffectPosType effectPosType;

	[SerializeField] protected bool isChangeState;

	public bool Instant;
	public bool IsChangeState => isChangeState;

	public SkillCameraShakeEffect skillCameraEffect;

	public bool waitForSignal;

	protected System.Action _callback;
	/// <summary>
	/// 진화 레벨은 0부터 시작이기 때문에 index 처럼 사용한다
	/// </summary>
	/// <typeparam name="T"></typeparam>
	/// <param name="level"></param>
	/// <returns></returns>
	protected virtual T GetEvolutinData<T>(int level) where T : SKillEvolutionData
	{
		return default(T);
	}

	public virtual bool Trigger(Unit _caster, RuntimeData.SkillInfo _info, System.Action callback)
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

		//HitInfo hitInfo = _caster.HitInfo;
		////if (_caster is Pet)
		////{
		////	hitInfo = UnitManager.it.Player.HitInfo;
		////}

		//if (_info.skillAbility.Value > 0)
		//{
		//	hitInfo.TotalAttackPower *= (_info.skillAbility.Value / 100f) * ((100 + _caster.SkillBuffValue) / 100f);
		//}
		//hitInfo.sprite = _info.IconImage;

		//_callback = callback;
		//_caster.StartCoroutine(Activation(_caster, _info, hitInfo));

		return true;
	}
	protected Vector3 GetPos(Unit caster, GameObject effect, Vector3 targetPos)
	{
		switch (effectPosType)
		{

			case EffectPosType.Self:
				return caster.position;
			//case EffectPosType.HitPosition:
			//	return caster.unitAnimation.HitPosition != null ? caster.unitAnimation.HitPosition.position : caster.position;
			case EffectPosType.Target:
				return targetPos;
			case EffectPosType.Effect:
				return effect != null ? effect.transform.position : targetPos;
			//case EffectPosType.Center:
			//	return caster.CenterPosition;
		}
		return targetPos;
	}

	protected virtual GameObject ShowEffect(Unit caster, Vector3 targetPos, int level = 1)
	{
		GameObject effect = obj;
		var data = GetEvolutinData<SKillEvolutionData>(level);
		if (data != null)
		{
			effect = data.effect;
		}

		if (effect == null)
		{
			//effect = caster.attackEffectObject;
		}

		if (effect == null)
		{
			return null;
		}
		GameObject go = Instantiate(effect);
		switch (effectPosType)
		{
			//case EffectPosType.Self:
			//	{
			//		go.transform.SetParent(caster.unitAnimation.transform);
			//		go.transform.position = caster.position;
			//	}
			//	break;
			//case EffectPosType.Center:
			//	{
			//		go.transform.SetParent(caster.unitAnimation.transform);
			//		go.transform.position = caster.CenterPosition;
			//	}
			//	break;
			//case EffectPosType.Target:
			//case EffectPosType.Effect:
			//	{
			//		go.transform.position = targetPos;
			//	}
			//	break;
			//case EffectPosType.HitPosition:
			//	if (caster.unitAnimation.HitPosition != null)
			//	{
			//		go.transform.position = caster.unitAnimation.HitPosition.position;
			//	}

				//break;

	
		}
		return go;
	}

	public void SetBasicData(SkillData data)
	{
		tid = data.tid;
		description = data.description;
	}

	protected virtual IEnumerator Activation(Unit caster, RuntimeData.SkillInfo info, AffectedInfo hitInfo)
	{

		if (caster.IsAlive() == false)
		{
			_callback?.Invoke();
			yield break;
		}

		//if (caster is EnemyUnit)
		//{
		//	Vector3 pos = caster.target != null ? caster.target.position : caster.position;
		//	caster.skillModule.StartCoroutine(AffectNonTarget(caster, info, hitInfo, pos));
		//}
		//else
		//{
		//	if (hitInfo.targetLayer == LayerMask.NameToLayer("Enemy"))
		//	{
		//		Vector3 pos = caster.target != null ? caster.target.position : caster.position;
		//		caster.skillModule.StartCoroutine(AffectNonTarget(caster, info, hitInfo, pos));
		//	}
		//	else
		//	{
		//		var list = UnitManager.it.GetRandomEnemies(caster.position, info.AttackRange, info.TargetCount);

		//		for (int i = 0; i < list.Count; i++)
		//		{unit
		//			caster.skillModule.StartCoroutine(AffectNonTarget(caster, info, hitInfo, list[i].position));
		//		}
		//	}
		//}

		yield return null;
		_callback?.Invoke();
	}


	protected virtual IEnumerator AffectNonTarget(Unit caster, RuntimeData.SkillInfo info, AffectedInfo hitInfo, Vector3 targetPos)
	{
		GameObject effect = ShowEffect(caster, targetPos, info.EvolutionLevel);
		Vector3 pos = GetPos(caster, effect, targetPos);
		int attackCount = 0;
		while (attackCount < info.AttackCount)
		{
			if (caster.IsAlive() == false)
			{
				break;
			}
			if (effectPosType == EffectPosType.Self)
			{
				pos = GetPos(caster, effect, targetPos);
			}

			OnAffectTasrget(caster, pos, info.HitRange, pos - caster.position, hitInfo, info);

			skillCameraEffect?.DoEffect();
			attackCount++;
			if (info.Duration == 0)
			{
				yield return null;
			}
			else
			{
				yield return new WaitForSeconds(info.Duration / info.AttackCount);
			}
		}
	}

	protected void OnAffectTasrget(Unit caster, Vector3 pos, float hitrange, Vector3 knockbackDir, AffectedInfo hitInfo, RuntimeData.SkillInfo skillInfo)
	{
		if (hitrange == 0)
		{
			OnAffectToFixedTarget(caster, hitInfo, skillInfo.KnockbackPower, knockbackDir, skillInfo);
			return;
		}
		AffectOverlapCircle(caster, pos, hitrange, knockbackDir, hitInfo, skillInfo);
	}

	protected virtual void OnAffectToFixedTarget(Unit caster, AffectedInfo hitInfo, float knockbackPower, Vector3 knockbackDir, RuntimeData.SkillInfo skillInfo)
	{
		//if (caster.TargetInRange(caster.target, out float distance, out float additionalRange))
		//{
		//	OnAffectTarget(caster, caster.target, hitInfo, skillInfo.KnockbackPower, knockbackDir, skillInfo);
		//}
	}

	protected virtual void AffectOverlapCircle(Unit caster, Vector3 pos, float hitrange, Vector3 knockbackDir, AffectedInfo hitInfo, RuntimeData.SkillInfo skillInfo)
	{
		SoundManager.Instance.PlayEffect(audioClip);


		var colliders = Physics2D.OverlapCircleAll(pos, hitrange, 1 << hitInfo.targetLayer);

		int count = skillInfo.HitCount > 0 ? Mathf.Min(skillInfo.HitCount, colliders.Length) : colliders.Length;

		for (int i = 0; i < count; i++)
		{
			Unit target = colliders[i].GetComponentInParent<Unit>();
			//OnAffectTarget(caster, target, hitInfo, skillInfo.KnockbackPower, knockbackDir, skillInfo);
		}
	}

	protected virtual void OnAffectTarget(Unit caster, Unit target, AffectedInfo hitInfo, float knockbackPower, Vector3 knockbackDir, RuntimeData.SkillInfo skillInfo)
	{
		HitTarget(caster, target, hitInfo as HitInfo, skillInfo.KnockbackPower, knockbackDir, skillInfo);
	}

	protected void HitTarget(Unit caster, Unit target, HitInfo hitInfo, float knockbackPower, Vector3 knockbackDir, RuntimeData.SkillInfo skillInfo)
	{
		if (target == null)
		{
			return;
		}

		//if (knockbackPower > 0)
		//{
		//	target.KnockBack(knockbackPower, knockbackDir, 1);
		//}
		//target.Hit(hitInfo, skillInfo);
		//if (target.IsAlive() == false)
		//{
		//	//caster.killCount++;
		//}
	}




}
