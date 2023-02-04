using System.Collections.Generic;

public abstract class Character : Unit
{
    public int skillStrength;
    public int areaSkillStrength;
    public int repairStrength;
    public int skillStrengthMultiplier = 1;
    public int skillkRange;
    public int movementRange;

    public abstract void targetedSkill(Character target);
    public abstract void areaSkill(List<Character> targets);

    public void receiveHealing(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void repair(Building target)
    {
        target.receiveRepair(repairStrength);
    }

    public void reset()
    {
        skillStrengthMultiplier = 1;
    }
}
