using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entangle")]
public class Entangle : Ability
{
    public EntangleRoot entangleRoot;

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
        List<List<Vector3>> waves = new List<List<Vector3>>();

        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        //Store start position.
        Vector2 spot;
        Vector2 spot2;
        Vector2 spot3;

        RaycastHit2D hit1;
        //RaycastHit2D hit2;

        //Check for obstacles in the way.
        //caster.CastHitDetectBlocking(end, end, out hit1);

        //Check if there is already an environmental hazard in that spot.
        //caster.CastMaskDetect(end, end, bleedRoot.environmentalHazard, out hit2);

        for (int i = 2; i < range + 2; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            spot = start + dir * i;

            caster.CastHitDetectBlocking(spot, spot, out hit1);

            //Check if there is already an environmental hazard in that spot.
            //caster.CastMaskDetect(spot, spot, fire.environmentalHazard, out hit2);

            if (hit1.transform != null && hit1.transform.gameObject.CompareTag("Wall"))
            {
                break;
            }
            else
            {
                wave.Add(spot);
            }
            
            //spot 2
            spot2 = spot + Vector2.Perpendicular(dir);

            caster.CastHitDetectBlocking(spot2, spot2, out hit1);

            if (hit1.transform != null && hit1.transform.gameObject.CompareTag("Wall"))
            {

            }
            else
            {
                wave.Add(spot2);
            }

            //spot 3
            spot3 = spot - Vector2.Perpendicular(dir);

            caster.CastHitDetectBlocking(spot3, spot3, out hit1);

            if (hit1.transform != null && hit1.transform.gameObject.CompareTag("Wall"))
            {

            }
            else
            {
                wave.Add(spot3);
            }
            waves.Add(wave);
        }

        caster.PlaceHazardWave(entangleRoot, waves, 0.2f);

        PlaceOnCooldown();
        //UIManager.Instance.UpdateActiveUnitAbilities(caster);
    }
}
