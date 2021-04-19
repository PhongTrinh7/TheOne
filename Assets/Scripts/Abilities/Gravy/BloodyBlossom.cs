using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BloodyBlossom")]
public class BloodyBlossom : Ability
{
    public BleedRoot bleedRoot;

    public override void ShowRange()
    {
        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        RaycastHit2D[] hitLayerMask;
        //RaycastHit2D hitBlockingLayer;


        for (int i = 1; i < range + 1; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            Vector2 spot1 = start + dir * i + Vector2.Perpendicular(dir);
            Vector2 spot2 = start + dir * i - Vector2.Perpendicular(dir);

            caster.CastMaskDetectMulti(spot1, spot2, layermask, out hitLayerMask);

            foreach (RaycastHit2D hit in hitLayerMask)
            {
                RaycastHit2D h;

                caster.CastHitDetectBlockingSingle(hit.transform.position, hit.transform.position, out h);

                if (h.transform != null && (h.transform.CompareTag("Player") || h.transform.CompareTag("Enemy")))
                {
                    targets.Add(h.transform.gameObject.GetComponent<MovingObject>());
                    h.transform.gameObject.GetComponent<MovingObject>().highlight(true);
                }

                //Check if anything was hit.
                if (hit.transform != null && h.transform == null || !h.transform.CompareTag("Wall"))
                {
                    GameObject ht = Instantiate(highlight, hit.transform.position, Quaternion.identity, caster.transform);
                    ht.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
                    affectedTiles.Add(ht);
                }
            }
        }
    }

    public override void Effect()
    {
        List<List<Vector3>> waves = new List<List<Vector3>>();
        /*
        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        //Store start position.
        //Vector2 spot2;
        //Vector2 spot3;

        RaycastHit2D hit1;
        //RaycastHit2D hit2;

        //Check for obstacles in the way.
        //caster.CastHitDetectBlocking(end, end, out hit1);

        //Check if there is already an environmental hazard in that spot.
        //caster.CastMaskDetect(end, end, bleedRoot.environmentalHazard, out hit2);

        for (int i = 1; i < range + 1; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            Vector2 spot = start + dir * i;

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
            /*
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
        }*/

        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        RaycastHit2D[] hitLayerMask;

        for (int i = 1; i < range + 1; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            Vector2 spot1 = start + dir * i + Vector2.Perpendicular(dir);
            Vector2 spot2 = start + dir * i - Vector2.Perpendicular(dir);

            caster.CastMaskDetectMulti(spot1, spot2, layermask, out hitLayerMask);

            foreach (RaycastHit2D hit in hitLayerMask)
            {
                Vector2 spot = hit.transform.position;

                RaycastHit2D hitBL;
                caster.CastHitDetectBlockingSingle(spot, spot, out hitBL);

                if (hitBL.transform == null || !hitBL.transform.gameObject.CompareTag("Wall"))
                {
                    wave.Add(hit.transform.position);
                }
            }
            waves.Add(wave);
        }

        caster.PlaceHazardWave(bleedRoot, waves, 0.2f);
    }
}