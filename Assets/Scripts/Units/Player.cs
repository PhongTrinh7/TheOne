using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MovingObject
{
    // Start is called before the first frame update
    protected override void Awake()
    {
        isNpc = false;
        base.Awake();
        ChangeFacingDirection(new Vector2(1, 0));
    }
}
