using System.Collections.Generic;

public class Radish : Character
{
    public override void onClicked()
    {
        canvasController.displayRadishSkills(hasActtion);
    }
    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        var ally = target as Character;
        if (ally == null) return;
        ally.receiveHealing(skillStrength * skillStrengthMultiplier);
        canvasController.displayRadishSkills(false);
    }

    public override void areaSkill(List<Unit> targets)
    {
        base.areaSkill(targets);
        foreach (var target in targets)
        {
            var ally = target as Character;
            if (ally == null) continue;
            ally.receiveHealing(areaSkillStrength * skillStrengthMultiplier);
        }
        canvasController.displayRadishSkills(false);
    }
}
