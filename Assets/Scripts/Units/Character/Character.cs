using System.Collections.Generic;

public abstract class Character : Unit
{
    public int skillStrength;
    public int areaSkillStrength;
    public int skillStrengthMultiplier = 1;
    public int skillkRange;
    public int movementRange;

    public abstract void targetedSkill(Unit target);
    public abstract void areaSkill(List<Unit> targets);

    public void reset()
    {
        skillStrengthMultiplier = 1;
    }
}
