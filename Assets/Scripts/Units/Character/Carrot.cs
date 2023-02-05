using System.Collections.Generic;

public class Carrot : Character
{
    public override void onClicked()
    {
        canvasController.displayCarrotSkills();
    }

    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        target.receiveDamage(skillStrength * skillStrengthMultiplier);
    }

    public override void areaSkill(List<Unit> targets)
    {
        base.areaSkill(targets);
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
    }
}
