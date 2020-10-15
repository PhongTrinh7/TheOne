using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Manager<UIManager>
{
    [SerializeField] private Canvas battleUI;
    public bool showBattleUI;

    [SerializeField] private Image abilityPanel;

    public Image turnOrderPanel;
    public List<Image> turnOrderPortraits;

    //Battle UI

    //Player info
    private MovingObject activeUnit;
    public GameObject activeCharacterInfoPanel;

    //Buttons
    [SerializeField] public Button b1;
    [SerializeField] public Button b2;
    [SerializeField] public Button b3;
    [SerializeField] public Button b4;
    [SerializeField] public Button b5;
    public Button endTurn;

    public bool abilitiesOn;

    void Start()
    {
        showBattleUI = false;
        BattleUI();
    }

    public void BattleUI()
    {

    }

    public void AbilityPanel()
    {
        abilityPanel.gameObject.SetActive(!abilityPanel.gameObject.activeInHierarchy);
    }

    public void UpdateActiveUnitAbilities(MovingObject activeUnit)
    {
        this.activeUnit = activeUnit;

        if (activeUnit.abilities[0].cooldownFill != 1 || activeUnit.energy < activeUnit.abilities[0].cost)
        {
            b1.interactable = false;
        }
        else
        {
            b1.interactable = true;
        }
        b1.GetComponent<Image>().fillAmount = activeUnit.abilities[0].cooldownFill;

        if (activeUnit.abilities[1].cooldownFill != 1 || activeUnit.energy < activeUnit.abilities[1].cost)
        {
            b2.interactable = false;
        }
        else
        {
            b2.interactable = true;
        }
        b2.GetComponent<Image>().fillAmount = activeUnit.abilities[1].cooldownFill;

        if (activeUnit.abilities[2].cooldownFill != 1 || activeUnit.energy < activeUnit.abilities[2].cost)
        {
            b3.interactable = false;
        }
        else
        {
            b3.interactable = true;
        }
        b3.GetComponent<Image>().fillAmount = activeUnit.abilities[2].cooldownFill;

        if (activeUnit.abilities[3].cooldownFill != 1 || activeUnit.energy < activeUnit.abilities[3].cost)
        {
            b4.interactable = false;
        }
        else
        {
            b4.interactable = true;
        }
        b4.GetComponent<Image>().fillAmount = activeUnit.abilities[3].cooldownFill;

        if (activeUnit.abilities[4].cooldownFill != 1 || activeUnit.energy < activeUnit.abilities[4].cost)
        {
            b5.interactable = false;
        }
        else
        {
            b5.interactable = true;
        }
        b5.GetComponent<Image>().fillAmount = activeUnit.abilities[4].cooldownFill;
    }

    public void Tooltip0()
    {
        b1.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[0]);
    }

    public void Tooltip1()
    {
        b2.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[1]);
    }

    public void Tooltip2()
    {
        b3.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[2]);
    }

    public void Tooltip3()
    {
        b4.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[3]);
    }
    
    public void Tooltip4()
    {
        b5.transform.GetChild(1).GetComponent<Tooltip>().ShowTooltip(activeUnit.abilities[4]);
    }

    public void HideTooltips()
    {
        b1.transform.GetChild(1).GetComponent<Tooltip>().HideTooltip();
        b2.transform.GetChild(1).GetComponent<Tooltip>().HideTooltip();
        b3.transform.GetChild(1).GetComponent<Tooltip>().HideTooltip();
        b4.transform.GetChild(1).GetComponent<Tooltip>().HideTooltip();
        b5.transform.GetChild(1).GetComponent<Tooltip>().HideTooltip();
    }

    public void SetSkillsUninteractable()
    {
        b1.interactable = false;
        b2.interactable = false;
        b3.interactable = false;
        b4.interactable = false;
        b5.interactable = false;
        abilitiesOn = false;
    }


    public void SetSkillsInteractable()
    {
        b1.interactable = true;
        b2.interactable = true;
        b3.interactable = true;
        b4.interactable = true;
        b5.interactable = true;
        abilitiesOn = true;

    }

    public void setEndTurn(bool b)
    {
        endTurn.interactable = b;
    }

    public void SetUpTurnOrderPortraits(Queue<MovingObject> currentUnits)
    {
        turnOrderPortraits = new List<Image>();

        foreach (MovingObject unit in currentUnits)
        {
            turnOrderPortraits.Add(Instantiate(unit.portrait, turnOrderPanel.transform));
        }

        //turnOrderPortraits[0].rectTransform.sizeDelta = new Vector2(72, 72);
        turnOrderPortraits[0].rectTransform.localScale = new Vector2(1.5f, 1.5f);
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

        //turnOrderPortraits[0].rectTransform.sizeDelta = new Vector2(72, 72);
        turnOrderPortraits[0].rectTransform.localScale = new Vector2(1.5f, 1.5f);
    }
}
