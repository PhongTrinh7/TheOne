using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    [SerializeField] private Canvas battleUI;

    [SerializeField] private Image abilityPanel;

    public Image turnOrderPanel;
    public List<Image> turnOrderPortraits;

    //Battle UI

    //Player info
    private MovingObject activeUnit;
    public HealthBar healthBar;

    //Buttons
    [SerializeField] public Button b1;
    [SerializeField] public Button b2;
    [SerializeField] public Button b3;
    [SerializeField] public Button b4;
    public Button endTurn;

    public bool abilitiesOn;

    public Text health;

    public EventTrigger trigger;

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
        this.activeUnit = activeUnit;

        if (activeUnit.abilities[0].cooldownFill != 1)
        {
            b1.interactable = false;
        }
        else
        {
            b1.interactable = true;
        }
        b1.GetComponent<Image>().fillAmount = activeUnit.abilities[0].cooldownFill;

        if (activeUnit.abilities[1].cooldownFill != 1)
        {
            b2.interactable = false;
        }
        else
        {
            b2.interactable = true;
        }
        b2.GetComponent<Image>().fillAmount = activeUnit.abilities[1].cooldownFill;

        if (activeUnit.abilities[2].cooldownFill != 1)
        {
            b3.interactable = false;
        }
        else
        {
            b3.interactable = true;
        }
        b3.GetComponent<Image>().fillAmount = activeUnit.abilities[2].cooldownFill;

        if (activeUnit.abilities[3].cooldownFill != 1)
        {
            b4.interactable = false;
        }
        else
        {
            b4.interactable = true;
        }
        b4.GetComponent<Image>().fillAmount = activeUnit.abilities[3].cooldownFill;
    }

    public void Tooltip0()
    {
        b1.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[0].Description());
    }

    public void Tooltip1()
    {
        b2.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[1].Description());
    }

    public void Tooltip2()
    {
        b3.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[2].Description());
    }

    public void Tooltip3()
    {
        b4.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[3].Description());
    }

    public void ShowRange1()
    {
        activeUnit.abilities[0].ShowRange(activeUnit);
    }

    public void ShowRange2()
    {
        activeUnit.abilities[1].ShowRange(activeUnit);
    }

    public void ShowRange3()
    {
        activeUnit.abilities[2].ShowRange(activeUnit);
    }

    public void ShowRange4()
    {
        activeUnit.abilities[3].ShowRange(activeUnit);
    }

    public void HideRange1()
    {
        activeUnit.abilities[0].HideRange(activeUnit);
    }

    public void HideRange2()
    {
        activeUnit.abilities[1].HideRange(activeUnit);
    }

    public void HideRange3()
    {
        activeUnit.abilities[2].HideRange(activeUnit);
    }

    public void HideRange4()
    {
        activeUnit.abilities[3].HideRange(activeUnit);
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

        turnOrderPortraits[0].rectTransform.sizeDelta = new Vector2(72, 72);
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

        turnOrderPortraits[0].rectTransform.sizeDelta = new Vector2(72, 72);
    }
}
