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

    public void SetHealth(int _value)
    {
        maxHealth = _value;
        Start();
    }

    public virtual void TakeHit(float hitAmount = 1)
    {
      
        currentHealth -= hitAmount;
        if (currentHealth <= 0) {
            if (tag == "Player")
                GameManager.instance.GameOver(false);
            else GameManager.instance.GameOver(true);
        }
        currentHealth = Mathf.Max(currentHealth, 0);
        myHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }

    public virtual void AddHealth(int _health)
    {
        currentHealth = Mathf.Min(currentHealth + _health, maxHealth);
        myHealthBar.UpdateHealthBar(currentHealth, maxHealth);
    }
}
