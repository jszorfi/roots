using System.Collections.Generic;

public class Enemy : Unit
{
    public int skillStrength;
    public int areaSkillStrength;

    public override void onClicked() { /*nope*/ }

    public void targetedSkill(Unit target)
    {
        target.receiveDamage(skillStrength);
    }

    public void areaSkill(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength);
        }
    }
    public void counterAttack(Character target)
    {
        target.receiveDamage(skillStrength);
    }
}
