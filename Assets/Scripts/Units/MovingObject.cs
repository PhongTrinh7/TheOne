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
        CHARGING,
        READYUP
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

    public string unitName;
    public Image portrait;
    public GameObject turnIndicator;
    public DamageNumber damageNumber;

    //Unit stats.
    public int maxHealth;
    public int health;
    public int speed;
    public int energy;
    public HealthBar healthBar;
    public EnergyBar energyBar;
    public int energyRegen;

    //Unit Abilities.
    public List<Ability> abilitiesReference;
    public List<Ability> abilities;
    public bool charging;
    public int loadedAbility;

    //Status effects.
    public Image statusEffectContainer;
    public List<StatusEffect> statuses;
    public bool bleed;
    public Image bleedIcon;
    public bool wet;
    public Image wetIcon;
    public bool shock;
    public Image shockIcon;
    public bool immobilize;
    public Image immobilizeIcon;
    public bool stun;
    public Image stunIcon;

    //Movement
    public float moveSpeed;
    protected float inverseMoveTime;
    public LayerMask blockingLayer;
    public Vector2 facingDirection;
    public int moveCost = 1;

    //Collision detection
    private BoxCollider2D boxCollider;
    private Rigidbody2D rb2D;

    //Animations
    private Animator anim;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        moveSpeed = 3f;

        health = maxHealth;

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
        energy = 0;
        energyRegen = speed;
        energyBar.SetMaxEnergy(12);

        //Get reference to the turn indicator.
        //turnIndicator = transform.GetChild(0).transform.GetChild(2).gameObject;
        turnIndicator.SetActive(false);

        //Get reference to the status effect container and status effects.
        statusEffectContainer = transform.GetChild(0).transform.GetChild(3).gameObject.GetComponent<Image>();
        bleedIcon = statusEffectContainer.transform.GetChild(0).gameObject.GetComponent<Image>();
        wetIcon = statusEffectContainer.transform.GetChild(1).gameObject.GetComponent<Image>();
        shockIcon = statusEffectContainer.transform.GetChild(2).gameObject.GetComponent<Image>();
        immobilizeIcon = statusEffectContainer.transform.GetChild(3).gameObject.GetComponent<Image>();
        stunIcon = statusEffectContainer.transform.GetChild(4).gameObject.GetComponent<Image>();

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
        if (stun || immobilize)
        {
            Debug.Log("Can't move");
            yield break;
        }

        priorState = state;
        state = UnitState.BUSY;

        Vector2 newFacingDirection = new Vector2(xDir, yDir);
        ChangeFacingDirection(newFacingDirection);

        //Change direction instead of move if facing direction is not the same as move direction.
        /*if (newFacingDirection != facingDirection)
        {
            //Change facing direction.
            ChangeFacingDirection(newFacingDirection);

            yield return null;
        }*/

        if (energy >= moveCost)
        {
            //Store start position to move from, based on objects current transform position.
            Vector2 start = transform.position;

            // Calculate end position based on the direction parameters passed in when calling Move.
            Vector2 end = start + facingDirection;

            RaycastHit2D hit;

            CastHitDetectBlockingSingle(end, end, out hit);

            //Check if anything was hit
            if (hit.transform == null)
            {
                energy -= moveCost;

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
            Vector3 newPosition = Vector3.MoveTowards(rb2D.position, end, speedMultiplier * moveSpeed * Time.fixedDeltaTime);
            rb2D.MovePosition(newPosition);
            sqrRemainingDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
    }

    public void ChangeFacingDirection(Vector2 direction)
    {
        //Sets facing direction.
        facingDirection = direction;
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
    }

    public bool ReadyAbility(int i)
    {
        priorState = state;
        state = UnitState.READYUP;

        if (energy >= abilities[i].cost)
        {
            if (abilities[i].Ready(this))
            {
                loadedAbility = i;
                return true;
            }
            else
            {
                Debug.Log(abilities[i] + " is on cooldown.");
                ReturnToPriorState();
                return false;
            }
        }
        else
        {
            Debug.Log("Not enough energy!");
            ReturnToPriorState();
            return false;
        }
    }

    public void CancelAbility()
    {
        abilities[loadedAbility].HideRange();
        loadedAbility = -1;
        state = UnitState.ACTIVE;
        Debug.Log("Ability canceled.");
    }

    public void CastAbility()
    {
        state = UnitState.BUSY;
        energy -= abilities[loadedAbility].cost;
        abilities[loadedAbility].Cast();
    }

    public void TriggerAnimation(string animationName)
    {
        anim.SetTrigger(animationName);
    }

    public void TriggerAbilityEffect()
    {
        abilities[loadedAbility].Effect();
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
        if (state == UnitState.CHARGING)
        {
            Debug.Log("Hide");
            abilities[loadedAbility].HideRange();
        }
        else
        {
            state = UnitState.BUSY;
        }

        //Store start position to move from, based on objects current transform position.
        Vector2 start = (Vector2) transform.position + direction;

        // Calculate end position based on the direction and displacement distance.
        Vector2 end = start + (direction * displacement);

        RaycastHit2D hit;

        CastHitDetectBlockingSingle(start, end, out hit);

        //Check if anything was hit
        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            yield return StartCoroutine(SmoothMovement(end, 2f));
        }

        else
        {
            //If something is hit, collide with obstacle.
            Vector3 offset = direction;
            yield return StartCoroutine(SmoothMovement(hit.transform.position - offset, 2f));
            TakeDamage(3);
        }

        if (state == UnitState.CHARGING)
        {
            Debug.Log("show");
            abilities[loadedAbility].ShowRange();
        }
        else
        {
            ReturnToPriorState();
        }
    }

    public void Dash(Vector2 direction, int distance, int damage)
    {
        StartCoroutine(DashCoroutine(direction, distance - 1, damage)); //-1 accounting for start vector being in front of unit.
    }

    public IEnumerator DashCoroutine(Vector2 direction, int distance, int damage)
    {
        state = UnitState.BUSY;
        anim.SetBool("Dashing", true);

        //Store start position to move from, based on objects current transform position.
        Vector2 start = (Vector2) transform.position + direction;

        // Calculate end position based on the direction and displacement distance.
        Vector2 end = start + (direction * distance);

        RaycastHit2D hit;

        CastHitDetectBlockingSingle(start, end, out hit);

        //Check if anything was hit
        if (hit.transform == null)
        {
            //If nothing was hit, start SmoothMovement co-routine passing in the Vector2 end as destination
            yield return StartCoroutine(SmoothMovement(end, 3f));
        }

        else
        {
            //If something is hit, collide with obstacle.
            Vector3 offset = facingDirection;
            yield return StartCoroutine(SmoothMovement(hit.transform.position - offset, 3f));
            if (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Player"))
            {
                abilities[loadedAbility].Effect();
                //hit.transform.gameObject.GetComponent<MovingObject>().TakeDamage(damage);
            }
        }

        loadedAbility = -1;
        anim.SetBool("Dashing", false);
        state = UnitState.ACTIVE;
    }

    public void Charge()
    {
        state = UnitState.CHARGING;
        charging = true;
        anim.SetBool("Charging", true);
        Debug.Log("Charging");
    }

    public void Discharge()
    {
        priorState = UnitState.ACTIVE;
        state = UnitState.BUSY;
        charging = false;
        anim.SetBool("Charging", false);
        abilities[loadedAbility].Discharge();
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
            yield return new WaitForSeconds(waveDelay);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (damage < 0)
        {
            Instantiate(damageNumber, transform.position, Quaternion.identity).SetDamageVisual(damage, true);
        }
        else
        {
            Instantiate(damageNumber, transform.position, Quaternion.identity).SetDamageVisual(damage, false);
        }

        if (!dead && health <= 0)
        {
            dead = true;
            return;
        }
        else if (dead && health > 0)
        {
            dead = false;
        }

        health = Mathf.Clamp(health, 0, maxHealth);
        healthBar.SetCurrentHealth(health);
    }

    public void CastMaskDetectSingle(Vector2 start, Vector2 end, LayerMask layerMask, out RaycastHit2D hit)
    {
        //Cast a line from start point to end point checking collision on a LayerMask.
        hit = Physics2D.Linecast(start, end, layerMask);
    }

    public void CastMaskDetectMulti(Vector2 start, Vector2 end, LayerMask layerMask, out RaycastHit2D[] hit)
    {
        //Cast a line from start point to end point checking collision on a LayerMask.
        hit = Physics2D.LinecastAll(start, end, layerMask);
    }

    public void CastHitDetectBlockingSingle(Vector2 start, Vector2 end, out RaycastHit2D hit)
    {
        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.Linecast(start, end, blockingLayer);
    }

    public void CastHitDetectBlockingMulti(Vector2 start, Vector2 end, out RaycastHit2D[] hit)
    {
        //Cast a line from start point to end point checking collision on blockingLayer.
        hit = Physics2D.LinecastAll(start, end, blockingLayer);
    }

    //Status handling.
    public void ApplyStatus(StatusEffect status)
    {
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
        energy = Mathf.Clamp(energy + energyRegen, 0, 12);

        ApplyEffects();
        HandleCooldowns();

        if (stun)
        {
            Debug.Log("stunned!");
            EndTurn();
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
        if (!charging)
        {
            state = UnitState.IDLE;
        }
        isTurn = false;
        turnIndicator.SetActive(isTurn);
        Debug.Log("Ending Turn");
    }

    public void Death()
    {
        if (loadedAbility != -1)
        {
            abilities[loadedAbility].HideRange();
        }
        Destroy(this.gameObject);
    }

    public void Update()
    {
        energyBar.SetCurrentEnergy(energy);

        bleedIcon.gameObject.SetActive(bleed);
        wetIcon.gameObject.SetActive(wet);
        shockIcon.gameObject.SetActive(shock);
        immobilizeIcon.gameObject.SetActive(immobilize);
        stunIcon.gameObject.SetActive(stun);
    }
}
