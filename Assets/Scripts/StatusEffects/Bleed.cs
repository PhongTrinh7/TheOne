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
        target.bleed = true;
    }

    public override void Effect()
    {
        target.TakeDamage(damage);
        timer++;
    }

    public override void ClearStatus()
    {
        target.bleed = false;
        target.statuses.Remove(this);
        Destroy(this);
    }
}
