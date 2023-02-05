using System.Collections.Generic;

public class Potato : Character
{
    public override void onClicked()
    {
        canvasController.displayPotatoSkills();
    }
    public override void targetedSkill(Unit target)
    {
        var enemy = target as Enemy;
        enemy.receiveDamage(skillStrength * skillStrengthMultiplier);
        enemy.counterAttack(this);
    }

    public override void areaSkill(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
    }

    public void counterAttack(Character target)
    {
        target.receiveDamage(skillStrength);
    }
}
