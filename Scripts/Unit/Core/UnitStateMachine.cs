using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

namespace UnitState
{
 
	public abstract class State
	{
		public State fallback;

		protected UnitStateMachine _owner;
		public void Initalize(UnitStateMachine owner)
		{
			_owner = owner;
		}
		public abstract State Enter();

		public virtual void InputHandle()
		{
			//if (_owner.unit.info.Hp <= 0)
			//{
			//	_owner.Dead();
			//}
		}

		public abstract void LogicUpdate(float deltaTime);

		public virtual void PhysicUpdate(float fixedDelta) { }
		public virtual void Exit() { }
        
		protected virtual void Fallback()
		{
			_owner.ChangeState(StateType.IDLE);
		}
	}

	public class Idle : State
	{

		public override State Enter()
		{
			_owner.unit.PlayAnimation("Base Layer.Idle");
			return this;
		}
		public override void LogicUpdate(float deltaTime)
		{
			//_owner.unit.FindTarget(0f, true);

			//if (_owner.unit.IsTargetAlive())
			//{
			//	_owner.ChangeState(StateType.CHASE);
			//	return;
			//}

		}
	}
	public class Chase : State
	{
        protected Vector3 targetPos;
        protected Vector3 dir;
		public override State Enter()
		{
			//_owner.unit.unitAnimation.SetParameter("moveSpeed", Mathf.Min(_owner.unit.MoveSpeed, 3f));
			//_owner.unit.PlayAnimation("Base Layer.run", 0);
			return this;

		}
		public override void LogicUpdate(float deltaTime)
		{
            //if (_owner.unit.IsTargetAlive() == false)
            //{
            //	Fallback();
            //	return;
            //}
            //targetPos = _owner.unit.target.position;
            //_owner.unit.GetTargetDistance(_owner.unit.target, out float distance, out float additionalRange);

            //if (distance + additionalRange < _owner.unit.AttackRange)
            //{
            //	_owner.ChangeState(StateType.ATTACK);
            //}
            //else
            //{
            //	_owner.unit.FindClosestTarget(_owner.unit.PursuitRange);
            //}
            dir = _owner.unit.HeadingPos();

        }
		public override void PhysicUpdate(float fixedDelta)
		{
			//_owner.unit.HeadingToTarget(targetPos);
			_owner.unit.rigidbody2D.MovePosition(_owner.unit.transform.position + dir * 1f * fixedDelta);
		}
	}


	public class Attack : State
	{
        protected string animationName = "Base Layer.Attack";
		public override State Enter()
		{
            _owner.unit.PlayAnimation(animationName);
			return this;
		}
        public override void LogicUpdate(float deltaTime)
        {
            var animaotrStateInfo = _owner.unit.animator.GetCurrentAnimatorStateInfo(0);
            if (animaotrStateInfo.IsName(animationName))
            {
                if (animaotrStateInfo.normalizedTime >= 1f)
                {
                    IsTargetInAttackRange();
                }
            }
            else
            {
                IsTargetInAttackRange();
            }

            if (_owner.unit.IsTargetAlive() == false)
            {
                _owner.ChangeState(StateType.IDLE);
            }
		}

		void IsTargetInAttackRange()
		{
			{
				Enter();
			}
		}
	}


	public class Skill : State
	{
        protected SkillSlot skillSlot;
        protected bool waitForSignal;
        protected string animationName;

		public void Set(SkillSlot slot)
		{
			skillSlot = slot;
		}
		public override State Enter()
		{
			animationName = $"Base Layer.{skillSlot.item.rawData.animation}";
			return this;
		}


		private void End()
		{
			waitForSignal = false;
			_owner.ChangeState(StateType.IDLE);
		}
		public override void LogicUpdate(float deltaTime)
		{
			if (waitForSignal)
			{
				return;
			}

			var animaotrStateInfo = _owner.unit.animator.GetCurrentAnimatorStateInfo(0);
			if (animaotrStateInfo.IsName(animationName))
			{
				if (animaotrStateInfo.normalizedTime >= 1f)
				{
					_owner.ChangeState(StateType.IDLE);
				}
			}
		}
	}

	public class Knockback : State
	{
        protected Vector2 dir;
        protected float power;
        protected Vector2 result;

        protected float friction = 0;
		public void Set(float power, Vector2 dir)
		{
			this.dir = dir;
			this.power = power;
		}
		public override State Enter()
		{
			friction = 0;
			result = dir * power;
			return this;
		}
		public override void LogicUpdate(float deltaTime)
		{
			if (result == Vector2.zero)
			{
				Fallback();
				return;
			}
			result = Vector2.Lerp(result, Vector2.zero, friction);
			friction += deltaTime;

		}

		public override void PhysicUpdate(float fixedDelta)
		{
			_owner.unit.GetComponent<Rigidbody2D>().velocity = result;
		}
	}

	public class Death : State
	{
		public override State Enter()
		{
			return this;
		}

		public override void InputHandle()
		{

		}

		public override void LogicUpdate(float deltaTime)
		{
			var stateInfo = _owner.unit.animator.GetCurrentAnimatorStateInfo(0);
			if (stateInfo.IsName("Base Layer.death"))
			{
				if (stateInfo.normalizedTime >= 1)
				{
					//	_owner.unit.endDeadAnimation = true;
				}
			}
		}
	}
}

public abstract class UnitStateMachine : MonoBehaviour
{
	public Unit unit { get; protected set; }
	protected bool isDead;
	protected Dictionary<StateType, UnitState.State> stateDict = new Dictionary<StateType, UnitState.State>();
	protected UnitState.State currentState;
	public StateType Now { get; protected set; }
    public abstract void Initialize(Unit unit);

	public virtual void ChangeSkillState(SkillSlot slot)
	{
		if (isDead)
			return;

		
		currentState.Exit();
		currentState = stateDict[StateType.SKILL];

		Now = StateType.SKILL;

        UnitState.Skill skillState = currentState as UnitState.Skill;

		skillState.Set(slot);
		skillState.Enter();
	}
	public virtual void ChangeKnockbackState(float power, Vector2 dir)
	{
		if (isDead)
			return;

		//unit.ChangeState(StateType.KNOCKBACK);
		currentState.Exit();
		currentState = stateDict[StateType.KNOCKBACK];
        Now = StateType.KNOCKBACK;
        UnitState.Knockback knockback = currentState as UnitState.Knockback;

		knockback.Set(power, dir);
		knockback.Enter();
	}

	public void ChangeState(StateType type)
	{
		if (isDead)
			return;

		Now = type ;
        if (stateDict.ContainsKey(type))
		{
			currentState.Exit();
			currentState = stateDict[type];
			currentState.Enter();
		}
	}
	public virtual void Dead()
	{
		//unit.ChangeState(StateType.DEATH);
		currentState.Exit();
		currentState = stateDict[StateType.DEATH];
		currentState.Enter();
		isDead = true;
	}


	 void Update()
	{
		if (isDead)
		{
			if (currentState is UnitState.Death)
			{
				currentState?.InputHandle();
				currentState?.LogicUpdate(Time.deltaTime);
			}
			return;
		}

		if (GameManager.GameStop)
		{
			if (currentState is  UnitState.Idle == false)
			{
				ChangeState(StateType.IDLE);
			}
			return;
		}

		currentState?.InputHandle();

		currentState?.LogicUpdate(Time.deltaTime);
	}

	private void FixedUpdate()
	{
		if (isDead || GameManager.GameStop)
		{
			return;
		}
		currentState?.PhysicUpdate(Time.fixedDeltaTime);
	}

}
