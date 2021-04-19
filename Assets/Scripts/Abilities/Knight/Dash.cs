using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Dash")]
public class Dash : Ability
{
    public int distance;

    public override void ShowRange()
    {
        //Store start position.
        Vector2 start = (Vector2) caster.transform.position + caster.facingDirection;

        // Calculate cast direction based on the direction the unit is facing.
        Vector2 end = start + caster.facingDirection * (distance - 1);

        RaycastHit2D[] hits;

        caster.CastMaskDetectMulti(start, end, layermask, out hits);

        foreach (RaycastHit2D hit in hits)
        {
            RaycastHit2D h;

            caster.CastHitDetectBlockingSingle(hit.transform.position, hit.transform.position, out h);

            //Check if anything was hit.
            if (hit.transform != null && (h.transform == null || !h.transform.CompareTag("Wall")))
            {
                GameObject ht = Instantiate(highlight, hit.transform.position, Quaternion.identity, caster.transform);
                ht.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
                affectedTiles.Add(ht);
            }
            else
            {
                break;
            }

            if (h.transform != null && (h.transform.CompareTag("Player") || h.transform.CompareTag("Enemy")))
            {
                targets.Add(h.transform.gameObject.GetComponent<MovingObject>());
                h.transform.gameObject.GetComponent<MovingObject>().highlight(true);
                break;
            }
        }
    }

    public override void Cast()
    {
        HideRange();

        caster.Dash(caster.facingDirection, distance, damage);

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
