using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash")]
public class Dash : Ability
{
    public int distance;

    public override bool Cast(MovingObject caster)
    {
        if (onCooldown)
        {
            return false;
        }

        caster.Dash(caster.facingDirection, distance, damage);
        PlaceOnCooldown();

        return true;
    }

    public override void Effect(MovingObject caster)
    {
        PlaceOnCooldown();
    }
}
