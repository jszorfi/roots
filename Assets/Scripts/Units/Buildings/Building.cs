public abstract class Building : Unit
{
    public void receiveRepair(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
    }
}
