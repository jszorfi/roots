using UnityEngine;

public abstract class Building : Unit
{
    public void receiveRepair(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * maxHealthWidth, healthBar.sizeDelta.y);
    }
}
