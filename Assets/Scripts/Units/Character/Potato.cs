using System.Collections.Generic;

public class Potato : Character
{
    public override void onClicked()
    {
        canvasController.displayCarrotSkills();
    }
    public override void targetedSkill(Character target)
    {
        target.receiveDamage(skillStrength * skillStrengthMultiplier);
    }

    public override void areaSkill(List<Character> targets)
    {
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength * skillStrengthMultiplier);
        }
    }
}
