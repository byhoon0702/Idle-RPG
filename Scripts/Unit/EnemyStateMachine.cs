using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnitState
{
    public class EnemyIdle : Idle
    {

    }
    public class EnemyChase : Chase
    {
        public override State Enter()
        {

            dir = Vector3.left;
            _owner.unit.PlayAnimation("Base Layer.run");
            return this;

        }
        public override void LogicUpdate(float deltaTime)
        {


        }
        public override void PhysicUpdate(float fixedDelta)
        {
            //_owner.unit.rigidbody2D.MovePosition(_owner.transform.position + dir * 1.5f * fixedDelta);
        }
    }


    public class EnemyAttack : Attack
    {
    }


    public class EnemySkill : Skill
    {

    }

    public class EnemyKnockback : Knockback
    {

    }

    public class EnemyDeath : Death
    {

    }
}
public class EnemyStateMachine : UnitStateMachine
{
    public override void Initialize(Unit unit)
    {
        this.unit = unit;
        isDead = false;

        UnitState.EnemyIdle idle = new UnitState.EnemyIdle();
        idle.Initalize(this);
        stateDict.Add(StateType.IDLE, idle);

        UnitState.EnemyChase chase = new UnitState.EnemyChase();
        chase.Initalize(this);
        chase.fallback = idle;
        stateDict.Add(StateType.CHASE, chase);

        UnitState.EnemyAttack attack = new UnitState.EnemyAttack();
        attack.Initalize(this);
        attack.fallback = idle;
        stateDict.Add(StateType.ATTACK, attack);

        UnitState.EnemySkill skill = new UnitState.EnemySkill();
        skill.Initalize(this);
        skill.fallback = idle;
        stateDict.Add(StateType.SKILL, skill);

        UnitState.EnemyDeath death = new UnitState.EnemyDeath();
        death.Initalize(this);
        stateDict.Add(StateType.DEATH, death);

        UnitState.EnemyKnockback knockback = new UnitState.EnemyKnockback();
        knockback.Initalize(this);
        knockback.fallback = idle;
        stateDict.Add(StateType.KNOCKBACK, knockback);

        currentState = idle;
        currentState.Enter();
    }
}
