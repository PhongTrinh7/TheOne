using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Entangle")]
public class Entangle : Ability
{
    public EntangleRoot entangleRoot;

    public override void ShowRange()
    {
        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        RaycastHit2D[] hitLayerMask;

        for (int i = 1; i < range + 1; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            Vector2 spot0 = start + dir * i;
            //Vector2 spot1 = start + dir * i + Vector2.Perpendicular(dir);
            //Vector2 spot2 = start + dir * i - Vector2.Perpendicular(dir);

            caster.CastMaskDetectMulti(spot0, spot0, layermask, out hitLayerMask);

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

        //Store start position.
        Vector2 start = caster.transform.position;

        Vector2 dir = caster.facingDirection;

        RaycastHit2D[] hitLayerMask;

        for (int i = 1; i < range + 1; i++)
        {
            List<Vector3> wave = new List<Vector3>();

            Vector2 spot0 = start + dir * i;
            //Vector2 spot1 = start + dir * i + Vector2.Perpendicular(dir);
            //Vector2 spot2 = start + dir * i - Vector2.Perpendicular(dir);

            caster.CastMaskDetectMulti(spot0, spot0, layermask, out hitLayerMask);

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

        caster.PlaceHazardWave(entangleRoot, waves, 0.2f);
    }
}
