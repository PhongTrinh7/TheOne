using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun")]
public class Stun : StatusEffect
{
    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override void OnApply(MovingObject target)
    {
        this.target = target;

        //Make sure there is only one stack of stun on a unit at a time.
        if (target.stun)
        {
            StatusEffect se = target.statuses.Find(x => x.statusName == "stun");
            target.statuses.Remove(se);
            Destroy(se);
        }
        else
        {
            target.stun = true;
        }

        target.statuses.Add(this);
    }

    public override void Effect()
    {
        Debug.Log("stunned!");
        timer++;
    }

    public override void ClearStatus()
    {
        target.stun = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
