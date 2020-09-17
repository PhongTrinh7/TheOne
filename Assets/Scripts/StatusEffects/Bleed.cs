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
        if (!target.bleed)
        {
            target.bleed = true;
        }
    }

    public override void Effect()
    {
        target.TakeDamage(damage);
        timer++;
    }

    public override void ClearStatus()
    {
        target.statuses.Remove(this);

        if (target.statuses.Find(x => x.name == "bleed") == null)
        {
            target.bleed = false;
        }

        Destroy(this);
    }
}
