using System.Collections.Generic;

public class Potato : Character
{
    public override void onClicked()
    {
        canvasController.displayPotatoSkills(hasActtion);
    }
    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        var enemy = target as Enemy;
        enemy.receiveDamage(skillStrength * skillStrengthMultiplier);
        enemy.counterAttack(this);
        canvasController.displayPotatoSkills(false);
    }

    public override void areaSkill(List<Unit> targets)
    {
        base.areaSkill(targets);
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
        canvasController.displayPotatoSkills(false);
    }

    public void counterAttack(Character target)
    {
        target.receiveDamage(skillStrength);
    }
}
