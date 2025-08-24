using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public interface IUnit
{
    void PlayAnimation(string name);
    void Spawn();
    void Attack();
    
    void Hit();
    void Death();
    void Skill();

}


public class UnitInfo
{
    public UnitStats stats;


    public IdleNumber Hp;
    public IdleNumber MaxHp;
    public IdleNumber AttackPower;
    public UnitInfo()
    {
        stats = new UnitStats();
        stats.Generate();

        MaxHp = stats.GetValue(StatsType.Hp);
        Hp = MaxHp;
        AttackPower = stats.GetValue(StatsType.Atk);
    }
}

public class Unit : MonoBehaviour, IUnit
{
    public Animator animator;
    public new Rigidbody2D rigidbody2D;
    public Transform center;
    public UnitInfo info;
    
    public Vector3 position;

    public Unit target;

    [SerializeField] protected UnitStateMachine stateMachine;
    public virtual void Attack()
    {
        stateMachine.ChangeState(StateType.ATTACK);
    }

    public virtual void Death()
    {
        stateMachine.ChangeState(StateType.DEATH);
    }

    public virtual bool IsTargetAlive()
    {
        if(target== null)
        {
            return false;

        }
        return target.info.Hp > 0;
    }
    public virtual Vector3 HeadingPos()
    {
        return Vector3.right;
    }
    public StateType GetState()
    {
        return stateMachine.Now;
    }

    public virtual void Hit()
    {

    }

    // Start is called before the first frame update


    public bool IsAlive()
    {
        return true;
    }

    public virtual  void Skill()
    {

    }
    public void PlayAnimation(string name)
    {
        animator.Play(name,0,0);
    }

    public virtual void Spawn()
    {
        
    }

    void Start()
    {
        PlayAnimation("Base Layer.Run");
    }
}
