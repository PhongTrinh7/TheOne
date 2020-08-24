using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnvironmentalHazard : MonoBehaviour
{
    public int damage;
    public List<StatusEffect> effects;
    public LayerMask environmentalHazard;
    private BoxCollider2D boxCollider;

    void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>(); // for collision
    }
}
