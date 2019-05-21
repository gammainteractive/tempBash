using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : LivingBody {

    public int m_damage = 50;

    private void Awake()
    {
    }
    // Use this for initialization
    protected override void Start () {
        base.Start();
    }

    // Update is called once per frame
    void Update () {
		
	}
}
