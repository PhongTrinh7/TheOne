using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ability : ScriptableObject
{
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
    public bool onCooldown = false;

    public virtual string Description()
    {
        string summary = abilityName + "\n" + description + "\nDamage: " + damage + "\nCost: " + cost + "\nCooldown: " + initialCooldown + "\nRange: " + range;
        return summary;
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
