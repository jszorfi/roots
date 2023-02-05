using System.Collections.Generic;

public class Radish : Character
{
    public override void onClicked()
    {
        canvasController.displayRadishSkills();
    }
    public override void targetedSkill(Unit target)
    {
        target.receiveHealing(skillStrength * skillStrengthMultiplier);
    }

    public override void areaSkill(List<Unit> targets)
    {
        foreach (var target in targets)
        {
            target.receiveHealing(areaSkillStrength * skillStrengthMultiplier);
        }
    }
}
