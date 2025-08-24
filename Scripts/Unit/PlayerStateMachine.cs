using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace UnitState
{
    public class PlayerIdle : Idle
    {
        public override State Enter()
        {
            _owner.unit.PlayAnimation("Base Layer.Idle");
            _owner.ChangeState(StateType.CHASE);
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
    public class PlayerChase : Chase
    {
        public override State Enter()
        {

            dir = Vector3.right;
            _owner.unit.PlayAnimation("Base Layer.Run");
            return this;

        }
        public override void LogicUpdate(float deltaTime)
        {
            

        }

        public override void PhysicUpdate(float fixedDelta)
        {
            RaycastHit2D hit = Physics2D.Raycast(_owner.unit.center.position, dir, 1f, 1<<LayerMask.NameToLayer("Enemy"));
            if (hit.collider != null) 
            {
                _owner.unit.rigidbody2D.velocity = Vector2.zero;
                _owner.unit.target = hit.collider.GetComponent<Enemy>() ;
                _owner.unit.Attack();
                
                return;
            }
            _owner.unit.rigidbody2D.velocity = dir * 1.5f;
        }
    }


    public class PlayerAttack : Attack
    {

    }


    public class PlayerSkill : Skill
    {
     
    }

    public class PlayerKnockback : Knockback
    {
        
    }

    public class PlayerDeath : Death
    {
      
    }
}
public class PlayerStateMachine : UnitStateMachine
{
    public override void Initialize(Unit unit)
    {
        this.unit = unit;
        isDead = false;

        UnitState.PlayerIdle idle = new UnitState.PlayerIdle();
        idle.Initalize(this);
        stateDict.Add(StateType.IDLE, idle);

        UnitState.PlayerChase chase = new UnitState.PlayerChase();
        chase.Initalize(this);
        chase.fallback = idle;
        stateDict.Add(StateType.CHASE, chase);

        UnitState.PlayerAttack attack = new UnitState.PlayerAttack();
        attack.Initalize(this);
        attack.fallback = idle;
        stateDict.Add(StateType.ATTACK, attack);

        UnitState.PlayerSkill skill = new UnitState.PlayerSkill();
        skill.Initalize(this);
        skill.fallback = idle;
        stateDict.Add(StateType.SKILL, skill);

        UnitState.PlayerDeath death = new UnitState.PlayerDeath();
        death.Initalize(this);
        stateDict.Add(StateType.DEATH, death);

        UnitState.PlayerKnockback knockback = new UnitState.PlayerKnockback();
        knockback.Initalize(this);
        knockback.fallback = idle;
        stateDict.Add(StateType.KNOCKBACK, knockback);

        currentState = idle;
        currentState.Enter();
    }

}
