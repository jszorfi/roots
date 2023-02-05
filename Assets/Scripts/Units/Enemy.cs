using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
    public override void onClicked() { /*nope*/ }
    private bool isWaitingToAttack = false;

    public override void targetedSkill(Unit target)
    {
        base.targetedSkill(target);
        target.receiveDamage(skillStrength);
        var potato = target as Potato;
        if (potato != null)
        {
            potato.counterAttack(this);
        }
    }

    public override void areaSkill(List<Unit> targets)
    {
        base.areaSkill(targets);
        foreach (var target in targets)
        {
            target.receiveDamage(areaSkillStrength);
        }
    }
    public void counterAttack(Character target)
    {
        target.receiveDamage(skillStrength);
    }

    public void Turn()
    {
        List<Unit> hitable = Enumerable.Concat<Unit>(canvasController.mapController.buildings, canvasController.mapController.characters).ToList();
        Unit closest = null;
        float min = 10000;
        foreach (var h in hitable)
        {
            var d = (h.pos - pos).magnitude;
            if (d < min)
            {
                min = d;
                closest = h;
            }
        }
        if (closest != null)
        {
            canvasController.mapController.moveEnemy(this, closest.pos);
            isWaitingToAttack = true;
        }
    }

    private void Attack()
    {
        var neaighbours = canvasController.mapController.neighbours4(pos);
        int minHealth = 10000;
        Unit minHealthUnit = null;
        foreach (var n in neaighbours)
        {
            var enemy = n as Enemy;
            if (enemy != null)
                continue;
            if (n.health < minHealth)
            {
                minHealth = n.health;
                minHealthUnit = n;
            }
        }
        if (minHealthUnit != null)
        {
            targetedSkill(minHealthUnit);
        }
    }
    public void Update()
    {
        if (isWaitingToAttack && !isBusy())
        {
            isWaitingToAttack = false;
            Attack();
        }
    }
}
