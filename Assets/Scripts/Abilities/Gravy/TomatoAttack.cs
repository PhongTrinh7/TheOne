using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TomatoAttack")]
public class TomatoAttack : Ability
{
    public Fire fire;

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
                if (hit.transform != null)
                {
                    GameObject ht = Instantiate(highlight, hit.transform.position, Quaternion.identity);
                    ht.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
                    affectedTiles.Add(ht);
                }
            }
        }
    }

    public override void Effect()
    {
        List<List<Vector3>> waves = new List<List<Vector3>>();

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

        caster.PlaceHazardWave(fire, waves, 0.2f);
    }
}
