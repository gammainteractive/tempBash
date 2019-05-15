using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingBody : MonoBehaviour {

    public int maxHealth;
    public float currentHealth;
    public HealthBar myHealthBar;

	// Use this for initialization
	protected virtual void Start () {
        currentHealth = maxHealth;
        myHealthBar.UpdateHealthBar(maxHealth, maxHealth);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public virtual void TakeHit(int hitAmount = 1)
    {
        currentHealth -= hitAmount;
        if (currentHealth <= 0) {
            if (tag == "Player")
                GameManager.instance.GameOver(false);
            else GameManager.instance.GameOver(true);
        }
        currentHealth = Mathf.Max(currentHealth, 0);
        Debug.Log(this.name + "Took {" + hitAmount + "} damage", this);
        myHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
}
