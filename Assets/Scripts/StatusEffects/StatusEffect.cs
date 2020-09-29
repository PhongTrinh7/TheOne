using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StatusEffect : ScriptableObject
{
    public string statusName;
    public string description;
    public Image icon;
    public int stacks = 0;
    public int damage;
    public int duration;
    public int timer = 0;
    protected MovingObject target;

    public virtual void Description()
    {
        Debug.Log(name + ": " + description);
    }

    public abstract void OnApply(MovingObject target);

    public abstract void Effect();

    public virtual bool CheckTimer()
    {
        if (timer == duration)
        {
            ClearStatus();
            return true;
        }
        return false;
    }

    public abstract void ClearStatus();
}
