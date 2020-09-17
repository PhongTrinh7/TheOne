﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentalHazard : MonoBehaviour
{
    public int damage;
    public List<StatusEffect> effects;
    public LayerMask environmentalHazard;
    public int duration;
    protected int durationTimer;
    protected BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>(); // for collision
        durationTimer = duration;
    }

    public void DurationCountDown()
    {
        duration--;
        if (duration <= 0)
        {
            Destroy(gameObject);
        }
    }
}
