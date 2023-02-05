using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Enemy : Character
{
    public override void onClicked() { /*nope*/ }
    private MapController mapController;

    void Start()
    {
        mapController = GameObject.Find("Tilemap").GetComponent<MapController>();
    }

    public override void targetedSkill(Unit target)
    {
        target.receiveDamage(skillStrength);
        var potato = target as Potato;
        if (potato != null)
        {
            potato.counterAttack(this);
        }
    }

    public override void areaSkill(List<Unit> targets)
    {
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
        List<Unit> hitable = Enumerable.Concat<Unit>(mapController.buildings, mapController.characters).ToList();
        Unit closest;
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
        mapController.
    }
}
