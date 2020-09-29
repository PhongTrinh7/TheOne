using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bleed")]
public class Bleed : StatusEffect
{
    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override void OnApply(MovingObject target)
    {
        this.target = target;

        if (target.bleed)
        {
            StatusEffect se = target.statuses.Find(x => x.statusName == "bleed");
            stacks = se.stacks + 1;
            target.statuses.Remove(se);
            Destroy(se);
        }
        else
        {
            target.bleed = true;
        }

        target.statuses.Add(this);
    }

    public override void Effect()
    {
        target.TakeDamage(stacks * damage);
        timer++;
    }

    public override void ClearStatus()
    {
        target.bleed = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
