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

    public void UpdateActiveUnitAbilities(MovingObject activeUnit)
    {
        b1.GetComponent<Image>().fillAmount = activeUnit.abilities[0].cooldownFill;
        b2.GetComponent<Image>().fillAmount = activeUnit.abilities[1].cooldownFill;
        b3.GetComponent<Image>().fillAmount = activeUnit.abilities[2].cooldownFill;
        b4.GetComponent<Image>().fillAmount = activeUnit.abilities[3].cooldownFill;
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
}
