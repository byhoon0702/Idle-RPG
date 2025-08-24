using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public Transform attackPoint;
    
    // Start is called before the first frame update
    void Start()
    {
        stateMachine.Initialize(this);
        stateMachine.ChangeState(StateType.CHASE);
        
    }
    public override void Spawn()
    {
        info = new UnitInfo();

    }
    public void HitTarget()
    {
        var colliders = Physics2D.OverlapCircleAll(attackPoint.position, 1f, 1 << LayerMask.NameToLayer("Enemy"));
        for(int i=0; i<colliders.Length; i++)
        {
            Enemy enemy =colliders[i].GetComponent<Enemy>();
            if (enemy == null)
                continue;
            enemy.Hit();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (center != null)
        {
            Gizmos.DrawRay(center.position, Vector3.right);
        }
    }
}
