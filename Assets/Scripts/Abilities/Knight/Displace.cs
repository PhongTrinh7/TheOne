using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Displace")]
public class Displace : Ability
{
    public int displacement;

    public override void Effect()
    {
        //Store start position.
        Vector2 start = caster.transform.position;

        // Calculate cast direction based on the direction the unit is facing.
        Vector2 end = start + caster.facingDirection;

        RaycastHit2D[] hits;

        caster.CastHitDetectBlockingMulti(end, end, out hits);

        foreach (RaycastHit2D hit in hits)
        {
            //Check if anything was hit.
            if (hit.transform != null && !hit.transform.gameObject.CompareTag("Wall"))
            {
                hit.transform.gameObject.GetComponent<MovingObject>().Launch(caster.facingDirection, displacement);
                hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
            }
        }
    }
}
