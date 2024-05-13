using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarBehavior : MonoBehaviour
{
	public Slider slider;
    public Gradient gradient;
    public Image fill;
    public int maxHealth;
    private int health;

    public void SetMaxHealth(int Health)
	{
        maxHealth = Health;
        health = Health;
        slider.maxValue = Health;
		slider.value = Health;

        fill.color = gradient.Evaluate(1f);
    }

    public bool TakeDemageAndDie(int damage)
    {
        if (health - damage > 0)
        {
            health -= damage;
            SetHealth(health);
            Debug.Log("Current Health:" + health);
            return false;
        }
        else
        {
            return true;
        }
    }

    public void SetHealth(int health)
	{
		slider.value = health;

		fill.color = gradient.Evaluate(slider.normalizedValue);
	}
}
