using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wet")]
public class Wet : StatusEffect
{
    public Stun stun;

    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override void OnApply(MovingObject target)
    {
        this.target = target;

        //Make sure there is only one stack of wet on a unit at a time.
        if (target.wet)
        {
            StatusEffect se = target.statuses.Find(x => x.statusName == "wet");
            target.statuses.Remove(se);
            Destroy(se);
        }
        else
        {
            if (target.shock)
            {
                target.ApplyStatus(Object.Instantiate(stun));
            }
            target.wet = true;
        }

        target.statuses.Add(this);
    }

    public override void Effect()
    {
        target.moveCost = 2;
        timer++;
    }

    public override void ClearStatus()
    {
        target.wet = false;
        target.moveCost = 1;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
