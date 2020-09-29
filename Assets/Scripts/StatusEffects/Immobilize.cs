using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Immobilize")]
public class Immobilize : StatusEffect
{
    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override void OnApply(MovingObject target)
    {
        this.target = target;

        //Make sure there is only one stack of wet on a unit at a time.
        if (target.immobilize)
        {
            StatusEffect se = target.statuses.Find(x => x.statusName == "immobilize");
            target.statuses.Remove(se);
            Destroy(se);
        }
        else
        {
            target.immobilize = true;
        }

        target.statuses.Add(this);
    }

    public override void Effect()
    {
        timer++;
    }

    public override void ClearStatus()
    {
        target.immobilize = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
