using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Rotation")]
public class Rotation : Ability
{
    public int displacement;

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

        for (int i = -1; i <= 1; i++)
        {
            for (int j = -1; j <= 1; j++)
            {
                if (i == 0 && j == 0)
                {

                }
                else {

                    // Calculate cast direction based on the direction the unit is facing.
                    Vector2 end = start + new Vector2(i, j);

                    RaycastHit2D hit;

                    caster.CastHitDetectBlocking(end, end, out hit);

                    //Check if anything was hit.
                    if (hit.transform != null && !hit.transform.gameObject.CompareTag("Wall"))
                    {
                        hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
                        hit.transform.gameObject.GetComponent<MovingObject>().Launch(new Vector2(i, j), displacement);
                    }
                }
            }
        }

        PlaceOnCooldown();
        //UIManager.Instance.UpdateActiveUnitAbilities(caster);
    }
}