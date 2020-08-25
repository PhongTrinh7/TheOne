using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
    public string name;
    public string characterRestriction;
    public int abilitySlot;
    public string animationName;
    public string description;
    public int initialDamage;
    public int damage;
    public int cost;
    public int initialCooldown;
    public int cooldown;
    public float cooldownFill;
    public bool onCooldown = false;

    public virtual void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public abstract bool Cast(MovingObject caster);

    public virtual void Discharge(MovingObject caster)
    {
        caster.TriggerAnimation(animationName, abilitySlot);
    }

    public abstract void Effect(MovingObject caster);

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
