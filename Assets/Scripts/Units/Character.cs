public abstract class Character : Unit
{
    public int damage;
    public int damageMultiplier = 1;
    public int attackRange;
    public int movementRange;

    public void Reset()
    {
        damageMultiplier = 1;
    }
}
