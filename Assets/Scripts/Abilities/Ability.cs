using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    protected MovingObject caster;
    protected List<MovingObject> targets = new List<MovingObject>();
    public string abilityName;
    public Sprite sprite;
    public string characterRestriction;
    public int abilitySlot;
    public string animationName;
    public string description;
    public int initialDamage;
    public int damage;
    public int cost;
    public int initialCooldown;
    public int cooldown;
    public int range;
    public float cooldownFill;
    public bool onCooldown;
    public GameObject highlight;
    public LayerMask layermask;
    public Color32 highlightColor;
    public List<GameObject> affectedTiles;

    public virtual void OnEnable()
    {
        layermask = 1 << LayerMask.NameToLayer("Floor");
        highlightColor = new Color32(255, 0, 0, 120);
        affectedTiles = new List<GameObject>();
        cooldownFill = 1;
    }

    public virtual string Description()
    {
        string summary = abilityName + "\n" + description + "\nDamage: " + damage + "\nCost: " + cost + "\nCooldown: " + initialCooldown + "\nRange: " + range;
        return summary;
    }

    public virtual void ShowRange()
    {
        //Store start position.
        Vector2 spot = (Vector2) caster.transform.position + caster.facingDirection;

        RaycastHit2D[] hits;

        caster.CastMaskDetectMulti(spot, spot, layermask, out hits);

        foreach (RaycastHit2D hit in hits)
        {
            RaycastHit2D h;

            caster.CastHitDetectBlockingSingle(hit.transform.position, hit.transform.position, out h);

            if (h.transform != null && (h.transform.CompareTag("Player") || h.transform.CompareTag("Enemy")))
            {
                targets.Add(h.transform.gameObject.GetComponent<MovingObject>());
                h.transform.gameObject.GetComponent<MovingObject>().highlight(true);
            }

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
        }
    }

    public virtual void HideRange()
    {
        foreach (MovingObject target in targets)
        {
            target.highlight(false);
        }
        targets.Clear();

        if (affectedTiles.Count != 0)
        {
            foreach (GameObject highlight in affectedTiles)
            {
                Destroy(highlight);
                //tile.GetComponent<SpriteRenderer>().color = Color.white;
            }
            affectedTiles.Clear();
        }
    }

    public virtual bool Ready(MovingObject caster)
    {
        if (onCooldown)
        {
            return false;
        }

        this.caster = caster;
        Debug.Log("ready");
        ShowRange();
        return true;
    }

    public virtual void Cast()
    {
        HideRange();

        caster.TriggerAnimation(animationName);

        PlaceOnCooldown();
    }

    public virtual void Discharge()
    {
        caster.TriggerAnimation(animationName);
    }

    public abstract void Effect();

    public virtual void PlaceOnCooldown()
    {
        cooldown = initialCooldown;
        onCooldown = true;
        cooldownFill = 0;
    }

    public virtual void Cooldown()
    {
        if (onCooldown)
        {
            cooldown--;
            cooldownFill += (1f / initialCooldown);
            if (cooldown == 0)
            {
                onCooldown = false;
                cooldownFill = 1;
            }
        }
    }
}
