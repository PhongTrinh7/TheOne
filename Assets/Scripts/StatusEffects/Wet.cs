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
            StatusEffect wet = target.statuses.Find(x => x.name == "wet");
            target.statuses.Remove(wet);
            Destroy(wet);
        }
        else
        {
            if (target.shock)
            {
                target.ApplyStatus(Object.Instantiate(stun));
            }
            target.wet = true;
        }
    }

    public override void Effect()
    {
        timer++;
    }

    public override void ClearStatus()
    {
        target.wet = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
