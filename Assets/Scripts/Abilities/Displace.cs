using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Displace")]
public class Displace : Ability
{
    public int displacement;

    public override void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public override bool Cast(MovingObject caster)
    {
        if (onCooldown)
        {
            return false;
        }

        caster.TriggerAnimation(animationName, abilitySlot);

        return true;
    }

    public override void Effect(MovingObject caster)
    {
        //Store start position.
        Vector2 start = caster.transform.position;

        // Calculate cast direction based on the direction the unit is facing.
        Vector2 end = start + caster.facingDirection;

        RaycastHit2D hit;

        caster.CastHitDetectBlocking(end, end, out hit);

        //Check if anything was hit.
        if (hit.transform != null && !hit.transform.gameObject.CompareTag("Wall"))
        {
            hit.transform.gameObject.GetComponent<MovingObject>().Launch(caster.facingDirection, displacement);
            hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(40);
        }
        else
        {
            Debug.Log("nothing was hit");
        }

        PlaceOnCooldown();
        UIManager.Instance.UpdateActiveUnitAbilities(caster);
    }
}
