using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingBody {

    private void Awake()
    {
    }
    // Use this for initialization
    protected override void Start () {
        maxHealth = GameManager.instance.MaxEnemyHealth;
        base.Start();

        
    }

    // Update is called once per frame
    void Update () {
		
	}
}
