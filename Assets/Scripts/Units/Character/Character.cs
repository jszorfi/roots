using System.Collections.Generic;
using UnityEngine;

public abstract class Character : Unit
{
    public int skillStrength;
    public int areaSkillStrength;
    public int repairStrength;
    public int skillStrengthMultiplier = 1;
    public int skillkRange;
    public int movementRange;
    public int currentMovement;

    public abstract void targetedSkill(Unit target);
    public abstract void areaSkill(List<Unit> targets);

    public void receiveHealing(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
    }

    public void repair(Building target)
    {
        target.receiveRepair(repairStrength);
    }

    public override void refresh()
    {
        base.refresh();
        skillStrengthMultiplier = 1;
        currentMovement = movementRange;
    }

    public void move(Vector2Int coordinates)
    {
        CharacterMovement m = gameObject.GetComponent<CharacterMovement>();
        m.StartMovingTo(coordinates);
        pos = coordinates;
    }

    public bool isBusy()
    {
        CharacterMovement m = gameObject.GetComponent<CharacterMovement>();
        return m.isMoving();
    }
}
