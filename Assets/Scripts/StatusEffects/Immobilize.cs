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
            StatusEffect immobilize = target.statuses.Find(x => x.name == "immobilize");
            target.statuses.Remove(immobilize);
            Destroy(immobilize);
        }
        else
        {
            target.immobilize = true;
        }
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
