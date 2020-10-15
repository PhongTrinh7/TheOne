﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Stun")]
public class Stun : StatusEffect
{
    public override void OnApply(MovingObject target)
    {
        this.target = target;

        StatusEffect se = target.statuses.Find(x => x.statusName == statusName);

        //Make sure there is only one stack of stun on a unit at a time.
        if (se != null)
        {
            se.timer += duration;
            Destroy(this.gameObject);
        }
        else
        {
            target.stun = true;
            target.Interrupted();
            timer = duration;
            stacks = 1;
            target.statuses.Add(this);
            transform.parent = target.statusEffectContainer.transform;
        }
    }
    public override void Effect()
    {
        target.skipTurn = true;
        timer--;
    }

    public override void ClearStatus()
    {
        target.stun = false;
        target.statuses.Remove(this);
        Destroy(this.gameObject);
    }
}
