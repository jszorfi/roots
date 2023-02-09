using System.Collections.Generic;

public class Carrot : Character
{
    public override void onClicked()
    {
        base.onClicked();
        canvasController.displayCarrotSkills(hasActtion);
    }

    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        target.receiveDamage(skillStrength * skillStrengthMultiplier);
        canvasController.displayCarrotSkills(false);
    }

    public override void areaSkill(List<Unit> targets)
    {
        base.areaSkill(targets);
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
        canvasController.displayCarrotSkills(false);
    }
}
