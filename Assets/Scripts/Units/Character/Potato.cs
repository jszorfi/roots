using System.Collections.Generic;

public class Potato : Character
{
    public override void onClicked()
    {
        base.onClicked();
        canvasController.displayPotatoSkills(hasActtion);
    }
    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        target.receiveDamage(skillStrength * skillStrengthMultiplier);
        target.counterAttack(this);
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

    public override void counterAttack(Unit target)
    {
        target.receiveDamage(skillStrength);
    }
}
