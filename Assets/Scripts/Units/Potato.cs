using System.Collections.Generic;

public class Potato : Character
{
    public int extraAttackDamage;
    public override void targetedSkill(Unit target)
    {
        target.receiveDamage(skillStrength * skillStrengthMultiplier);
    }

    public override void areaSkill(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
    }
}
