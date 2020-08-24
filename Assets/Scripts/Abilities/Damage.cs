/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Damage")]
public class Damage : Ability
{
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

        //Store start position.
        Vector2 start = caster.transform.position;

        // Calculate cast direction based on the direction the unit is facing.
        Vector2 end = start + caster.facingDirection;

        RaycastHit2D hit;

        caster.CastHitDetectBlocking(end, end, out hit);

        //Check if anything was hit.
        if (hit.transform != null && !hit.transform.gameObject.CompareTag("Wall"))
        {
            hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);

            cooldown = initialCooldown;
            onCooldown = true;

            return true;
        }

        else
        {
            Debug.Log("nothing was hit");

            return false;
        }
    }
}
*/