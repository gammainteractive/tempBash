using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingBody {

    public int m_damage = 50;

    [HideInInspector]
    public int m_currentHealthPercent = 100;

    public delegate void EnemyTakeHitHandler();

    public event EnemyTakeHitHandler EnemyTakeHitHandle;

    public void TakeHitHandle()
    {
        if (EnemyTakeHitHandle != null)
        {
            EnemyTakeHitHandle.Invoke();
        }
    }

    public delegate void EnemyAttackHandler();

    public event EnemyAttackHandler EnemyAttackHandle;

    public void AttackHandle()
    {
        if (EnemyAttackHandle != null)
        {
            EnemyAttackHandle.Invoke();
        }
    }

    protected override void Start() {
        base.Start();
    }

    public override void TakeHit(float hitAmount = 1)
    {
        base.TakeHit(hitAmount);
        m_currentHealthPercent = (int)((base.currentHealth / base.maxHealth) * 100);
        TakeHitHandle();
    }

    public void Attack()
    {
        AttackHandle();
    }
}
