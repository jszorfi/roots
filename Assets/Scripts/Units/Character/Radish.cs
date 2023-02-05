using System.Collections.Generic;

public class Radish : Character
{
    public override void onClicked()
    {
        canvasController.displayRadishSkills();
    }
    public override void targetedSkill(Unit target)
    {
        var ally = target as Character;
        if (ally == null) return;
        ally.receiveHealing(skillStrength * skillStrengthMultiplier);
    }

    public override void areaSkill(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            var ally = target as Character;
            if (ally == null) continue;
            ally.receiveHealing(areaSkillStrength * skillStrengthMultiplier);
        }
    }
}
