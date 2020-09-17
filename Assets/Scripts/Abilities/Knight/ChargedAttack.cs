using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChargedAttack")]
public class ChargedAttack : Ability
{
    public string dischargeAnimationName;

    public override void Cast()
    {
        damage = caster.energy * 2;
        caster.energy = 0;
        caster.Charge();
    }

    public override void Discharge()
    {
        HideRange();
        damage += caster.energy * 2;
        caster.energy = 0;
        caster.TriggerAnimation(animationName);
        PlaceOnCooldown();
    }

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
                hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
            }
        }
    }
}
