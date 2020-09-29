using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    protected MovingObject caster;
    public string abilityName;
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
        highlightColor = new Color32(255, 0, 0, 60);
        affectedTiles = new List<GameObject>();
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
            //Check if anything was hit.
            if (hit.transform != null)
            {
                GameObject ht = Instantiate(highlight, hit.transform.position, Quaternion.identity);
                ht.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
                affectedTiles.Add(ht);
                /*if (hit.transform.gameObject.GetComponent<SpriteRenderer>().color == Color.white)
                {
                    affectedTiles.Add(hit.transform.gameObject);
                    hit.transform.gameObject.GetComponent<SpriteRenderer>().color = highlightColor;
                }*/
            }
        }
    }

    public virtual void HideRange()
    {
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
