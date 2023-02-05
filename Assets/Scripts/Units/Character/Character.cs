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
    public bool hasActtion = true;

    public virtual void targetedSkill(Unit target)
    {
        animator.SetAnimationByName("Cast Spell", delegate { animator.SetAnimationByName("Idle"); });
        hasActtion = false;
    }
    public virtual void areaSkill(List<Unit> targets)
    {
        animator.SetAnimationByName("Cast Spell", delegate { animator.SetAnimationByName("Idle"); });
        hasActtion = false;
    }

    public void receiveHealing(int healing)
    {
        health += healing;
        if (health > maxHealth)
            health = maxHealth;
        healthBar.sizeDelta = new Vector2((float)health / (float)maxHealth * maxHealthWidth, healthBar.sizeDelta.y);
    }

    public void repair(Building target)
    {
        target.receiveRepair(repairStrength);
    }

    public override void refresh()
    {
        skillStrengthMultiplier = 1;
        currentMovement = movementRange;
        hasActtion = true;
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
