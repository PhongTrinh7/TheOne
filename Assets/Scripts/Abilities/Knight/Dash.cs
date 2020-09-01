using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash")]
public class Dash : Ability
{
    public int distance;

    public override void ShowRange(MovingObject caster)
    {
        //Store start position.
        Vector2 start = caster.transform.position;

        // Calculate cast direction based on the direction the unit is facing.
        Vector2 end = start + caster.facingDirection;

        RaycastHit2D hit;

        caster.CastMaskDetect(end, end, layermask, out hit);

        //Check if anything was hit.
        if (hit.transform != null)
        {
            hit.transform.gameObject.GetComponent<SpriteRenderer>().color = new Color32();
        }
    }

    public override void HideRange(MovingObject caster)
    {
        throw new System.NotImplementedException();

    }
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
