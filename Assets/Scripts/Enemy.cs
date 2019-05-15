using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingBody {

    private void Awake()
    {
    }
    // Use this for initialization
    protected override void Start () {
        base.Start();

        maxHealth = GameManager.instance.MaxEnemyHealth;
    }

    // Update is called once per frame
    void Update () {
		
	}
}
