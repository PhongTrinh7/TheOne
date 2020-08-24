using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    [SerializeField] private Canvas battleUI;

    [SerializeField] private Image abilityPanel;

    public Image turnOrderPanel;
    public List<Image> turnOrderPortraits;

    //Battle UI

    //Player info
    public HealthBar healthBar;

    //Buttons
    [SerializeField] public Button b1;
    [SerializeField] public Button b2;
    [SerializeField] public Button b3;
    [SerializeField] public Button b4;
    public bool abilitiesOn;

    public Text health;

    void Start()
    {
        BattleUI();
    }

    public void BattleUI()
    {
        battleUI.gameObject.SetActive(true);
    }

    public void AbilityPanel()
    {
        abilityPanel.gameObject.SetActive(!abilityPanel.gameObject.activeInHierarchy);
    }

    public void SetSkillsUninteractable()
    {
        b1.interactable = false;
        b2.interactable = false;
        b3.interactable = false;
        b4.interactable = false;
        abilitiesOn = false;
    }


    public void SetSkillsInteractable()
    {
        b1.interactable = true;
        b2.interactable = true;
        b3.interactable = true;
        b4.interactable = true;
        abilitiesOn = true;

    }

    public void HealthBarMaxValue(int health)
    {
        healthBar.SetMaxHealth(health);
    }

    public void HealthBarCurrentValue(int health)
    {
        healthBar.SetCurrentHealth(health);
    }

    public void SetUpTurnOrderPortraits(Queue<MovingObject> currentUnits)
    {
        turnOrderPortraits = new List<Image>();

        foreach (MovingObject unit in currentUnits)
        {
            turnOrderPortraits.Add(Instantiate(unit.portrait, turnOrderPanel.transform));
        }
    }

    public void UpdateTurnOrderPortraits(Queue<MovingObject> currentUnits)
    {
        turnOrderPortraits.Clear();

        foreach (Transform child in turnOrderPanel.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (MovingObject unit in currentUnits)
        {
            turnOrderPortraits.Add(Instantiate(unit.portrait, turnOrderPanel.transform));
        }
    }

    public void AdvanceTurnOrderPortraits()
    {
        /*Image portrait = turnOrderPortraits[0];
        turnOrderPortraits.Remove(portrait);

        turnOrderPortraits.Add(Instantiate(portrait, turnOrderPanel.transform));
        Destroy(portrait.gameObject);
    }

    public void RemoveTurnOrderPortrait(MovingObject unit)
    {
        Image portrait = turnOrderPortraits.Find(x => x.sprite == unit.portrait.sprite);
        turnOrderPortraits.Remove(portrait);
        Destroy(portrait.gameObject);

        /*Image portrait = turnOrderPortraits[currentTurn];
        turnOrderPortraits.Remove(portrait);
        Destroy(portrait.gameObject);*/
    }
}
