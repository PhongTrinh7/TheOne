using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Shock")]
public class Shock : StatusEffect
{
    public Stun stun;

    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override void OnApply(MovingObject target)
    {
        this.target = target;

        //Make sure there is only one stack of shock on a unit at a time.
        if (target.shock)
        {
            StatusEffect se = target.statuses.Find(x => x.statusName == "shock");
            target.statuses.Remove(se);
            Destroy(se);
        }
        else
        {
            if (target.wet)
            {
                target.ApplyStatus(Object.Instantiate(stun));
            }
            target.shock = true;
        }

        target.statuses.Add(this);
    }

    public override void Effect()
    {
        target.energy -= 2;
        timer++;
    }

    public override void ClearStatus()
    {
        target.shock = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
