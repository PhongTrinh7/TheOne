using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class MovingObject : MonoBehaviour
{
    //States
    public bool isTurn;
    public bool dead;

    public enum UnitState{
        IDLE,
        ACTIVE,
        BUSY,
        CHARGING
    }

    public UnitState state = UnitState.IDLE;
    public UnitState priorState = UnitState.IDLE;

    public UnitState State
    {
        get { return state; }
        set { state = value; }
    }

    //NPC check
    protected bool isNpc;

    public bool IsNpc
    {
        get { return isNpc; }
    }

    public string name;
    public Image portrait;
    public GameObject turnIndicator;

    //Unit stats.
    public int health;
    public int speed;
    public int energy;
    public HealthBar healthBar;
    public EnergyBar energyBar;
    public int energyRegen = 4;

    //Unit Abilities.
    public List<Ability> abilitiesReference;
    public List<Ability> abilities;
    public bool charging = false;
    public int loadedAbility;

    //Status effects.
    public List<StatusEffect> statuses;
    public bool stun;
    public bool bleed;
    public bool wet;
    public bool shock;

    //Movement
    protected float moveSpeed = 20f;
    protected float inverseMoveTime;
    public LayerMask blockingLayer;
    public Vector2 facingDirection;
    public int moveCost = 1;

    //Collision detection
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;
    private Vector3 start;

    //Animations
    private Animator anim;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        //Collision detection.
        boxCollider = GetComponent<BoxCollider2D>(); // for collision
        rb2D = GetComponent<Rigidbody2D>(); // for collision
        inverseMoveTime = 1f / moveSpeed; // for smooth movement
        anim = GetComponent<Animator>(); // get animator

        //Get reference to health bar.
        healthBar = transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<HealthBar>();
        healthBar.SetMaxHealth(health);

        //Get reference to energy bar.
        energyBar = transform.GetChild(0).transform.GetChild(1).gameObject.GetComponent<EnergyBar>();
        energy = speed;
        energyBar.SetMaxEnergy(energy);

        //Get reference to the turn indicator.
        turnIndicator = transform.GetChild(0).transform.GetChild(2).gameObject;
        turnIndicator.SetActive(false);

        //Refresh ability cooldowns each battle.
        foreach (Ability a in abilitiesReference)
        {
            abilities.Add(Object.Instantiate(a));
        }
    }

    //Move returns true if it is able to move and false if not. 
    //Move takes parameters for x direction, y direction and a RaycastHit2D to check collision.
    public IEnumerator Move(int xDir, int yDir)
    {
        if (stun)
        {
            Debug.Log("Can't move");
            yield break;
        }

        priorState = state;
        state = UnitState.BUSY;

        Vector2 newFacingDirection = new Vector2(xDir, yDir);

        //Change direction instead of move if facing direction is not the same as move direction.
        if (newFacingDirection != facingDirection)
        {
            //Change facing direction.
            ChangeFacingDirection(newFacingDirection);

            yield return null;
        }
        else
        {
            //Store start position to move from, based on objects current transform position.
            Vector2 start = transform.position;

            // Calculate end position based on the direction parameters passed in when calling Move.
            Vector2 end = start + facingDirection;

            RaycastHit2D hit;

            CastHitDetectBlocking(end, end, out hit);

            //Check if anything was hit
            if (hit.transform == null && energy > 0)
            {
                if (wet)
                {
                    energy--;
                }

                energy--;
                //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination

                //Movement animation.
                anim.SetBool("Moving", true);
                yield return StartCoroutine(SmoothMovement(end, 1f));
            }
        }

        anim.SetBool("Moving", false);
        ReturnToPriorState();
    }

    protected IEnumerator SmoothMovement(Vector3 end, float speedMultiplier)
    {
        float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

        while (sqrRemainingDistance > float.Epsilon)
        {
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, speedMultiplier * moveSpeed * Time.deltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    protected void ChangeFacingDirection(Vector2 direction)
    {
        //Sets facing direction.
        facingDirection = direction;
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
    }

    public void CastAbility(int i)
    {
        priorState = state;
        state = UnitState.BUSY;

        if (energy >= abilities[i].cost)
        {
            if (abilities[i].Cast(this))
            {
                energy -= abilities[i].cost;
            }
            else
            {
                Debug.Log("cast failed.");
                ReturnToPriorState();
            }
        }
        else
        {
            Debug.Log("cast failed.");
            ReturnToPriorState();
        }
    }

    public void TriggerAnimation(string animationName, int abilitySlot)
    {
        loadedAbility = abilitySlot;
        anim.SetTrigger(animationName);
    }

    public void TriggerAbilityEffect()
    {
        abilities[loadedAbility].Effect(this);
        loadedAbility = -1;
    }

    public void ReturnToPriorState()
    {
        state = priorState;
    }
  
    public void Launch(Vector2 direction, int displacement)
    {
        StartCoroutine(LaunchCoroutine(direction, displacement-1)); //-1 accounting for start vector being in front of unit.
    }

    public IEnumerator LaunchCoroutine(Vector2 direction, int displacement)
    {
        priorState = state;
        state = UnitState.BUSY;

        //Store start position to move from, based on objects current transform position.
        Vector2 start = (Vector2) transform.position + direction;

        // Calculate end position based on the direction and displacement distance.
        Vector2 end = start + (direction * displacement);

        RaycastHit2D hit;

        CastHitDetectBlocking(start, end, out hit);

        //Check if anything was hit
        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            yield return StartCoroutine(SmoothMovement(end, 3f));
        }

        else
        {
            //If something is hit, collide with obstacle.
            Vector3 offset = direction;
            yield return StartCoroutine(SmoothMovement(hit.transform.position - offset, 3f));
            TakeDamage(50);
        }

        ReturnToPriorState();
    }

    public void Dash(Vector2 direction, int distance, int damage)
    {
        StartCoroutine(DashCoroutine(direction, distance - 1, damage)); //-1 accounting for start vector being in front of unit.
    }

    public IEnumerator DashCoroutine(Vector2 direction, int distance, int damage)
    {
        priorState = state;
        state = UnitState.BUSY;

        //Store start position to move from, based on objects current transform position.
        Vector2 start = (Vector2) transform.position + direction;

        // Calculate end position based on the direction and displacement distance.
        Vector2 end = start + (direction * distance);

        RaycastHit2D hit;

        CastHitDetectBlocking(start, end, out hit);

        //Check if anything was hit
        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            yield return StartCoroutine(SmoothMovement(end, 2f));
        }

        else
        {
            //If something is hit, collide with obstacle.
            Vector3 offset = facingDirection;
            yield return StartCoroutine(SmoothMovement(hit.transform.position - offset, 2f));
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
            }
        }

        ReturnToPriorState();
    }

    public void Charge(int abilitySlot)
    {
        state = UnitState.CHARGING;
        charging = true;
        anim.SetBool("Charging", true);
        loadedAbility = abilitySlot;
        Debug.Log("Charging");
    }

    public void Discharge()
    {
        priorState = UnitState.ACTIVE;
        state = UnitState.BUSY;
        charging = false;
        anim.SetBool("Charging", false);
        abilities[loadedAbility].Discharge(this);
    }

    public void PlaceHazardWave(EnvironmentalHazard hazard, List<List<Vector3>> waves, float waveDelay)
    {
        StartCoroutine(PlaceHazardWaveCoroutine(hazard, waves, waveDelay));
    }

    IEnumerator PlaceHazardWaveCoroutine(EnvironmentalHazard hazard, List<List<Vector3>> waves, float waveDelay)
    {
        foreach (List<Vector3> wave in waves)
        {
            foreach (Vector3 position in wave)
            {
                Instantiate(hazard, position, Quaternion.identity);
            }

            Debug.Log("spawn wave");
            yield return new WaitForSeconds(waveDelay);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (!dead && health <= 0)
        {
            health = 0;
            dead = true;
        }
        else if (dead && health > 0)
        {
            dead = false;
        }

        healthBar.SetCurrentHealth(health);
    }

    public void CastMaskDetect(Vector2 start, Vector2 end, LayerMask layerMask, out RaycastHit2D hit)
    {
        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on a LayerMask.
        hit = Physics2D.Linecast(start, end, layerMask);

        //Re-enable boxCollider after linecast.
        boxCollider.enabled = true;
    }

    public void CastHitDetectBlocking(Vector2 start, Vector2 end, out RaycastHit2D hit)
    {
        //Disable the boxCollider so that linecast doesn't hit this object's own collider.
        //boxCollider.enabled = false;

        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);

        //Re-enable boxCollider after linecast.
        //boxCollider.enabled = true;
    }

    //Status handling.
    public void ApplyStatus(StatusEffect status)
    {
        statuses.Add(status);
        status.OnApply(this);
    }

    public void ApplyEffects()
    {
        foreach (StatusEffect status in statuses.ToArray())
        {
            status.Effect();
            status.CheckTimer();
        }
    }

    public void HandleCooldowns()
    {
        foreach (Ability a in abilities)
        {
            a.Cooldown();
        }
    }

    public void StartTurn()
    {
        Debug.Log(this + " starts turn.");
        isTurn = true;
        turnIndicator.SetActive(isTurn);
        energy = speed;
        ApplyEffects();
        HandleCooldowns();

        if (stun)
        {
            Debug.Log("stunned!");
        }
        else if (charging)
        {
            Discharge();
        }
        else
        {
            state = UnitState.ACTIVE;
        }
    }

    public void EndTurn()
    {
        state = UnitState.IDLE;
        isTurn = false;
        turnIndicator.SetActive(isTurn);
        Debug.Log("Ending Turn");
    }

    public void Death()
    {
        Destroy(this.gameObject);
    }

    public void Update()
    {
        energyBar.SetCurrentEnergy(energy);
    }
}
