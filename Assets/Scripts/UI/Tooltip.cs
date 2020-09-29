﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    private RectTransform background;
    private Text tooltipText;
    private Ability ability;

    void Awake()
    {
        background = transform.Find("Background").GetComponent<RectTransform>();
        tooltipText = transform.Find("Text").GetComponent<Text>();
    }

    public void ShowTooltip(Ability ability)
    {
        gameObject.SetActive(true);
        this.ability = ability;
        tooltipText.text = ability.Description();
        float padding = 8f;
        Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + padding, tooltipText.preferredHeight + padding);
        background.sizeDelta = backgroundSize;
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }

    public void Update()
    {
        if (isActiveAndEnabled)
        {
            tooltipText.text = ability.Description();
            float padding = 8f;
            Vector2 backgroundSize = new Vector2(tooltipText.preferredWidth + padding, tooltipText.preferredHeight + padding);
            background.sizeDelta = backgroundSize;
        }
    }
}
