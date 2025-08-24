using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    // Start is called before the first frame update
    void Start()
    {
        stateMachine.Initialize(this);
        stateMachine.ChangeState(StateType.IDLE);
    }
    public override void Spawn()
    {
        info = new UnitInfo();
        
    }
    public override void Hit()
    {
        base.Hit();
        PlayAnimation("Base Layer.Hurt");
        info.Hp -= 10;
        if(info.Hp <= 0)
        {
            Debug.Log("Death");
            Destroy(gameObject);
        }
    }
}
