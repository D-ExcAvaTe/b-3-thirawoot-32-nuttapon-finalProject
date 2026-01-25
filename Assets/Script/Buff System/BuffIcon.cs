using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BuffIcon : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textBuffName;
    [SerializeField] private TextMeshProUGUI textBuffDescription;
    [SerializeField] private Image iconBuff;
    [SerializeField] private GameObject[] stars;
    
    [Header("Rarity preferences")]
    [SerializeField] private Image buffRarityBackground;
    [SerializeField] private Color32 colorRare, colorEpic, colorLegend;
    [SerializeField] private GameObject glintObj;
    private Buff currentBuff;

    public void Init(Buff buffData)
    {
        buffData.GetDescription();

        currentBuff = buffData;

        textBuffName.text = currentBuff.buffName;
        textBuffDescription.text = currentBuff.description;
        iconBuff.sprite = currentBuff.buffIcon;

        foreach (var star in stars)
            star.SetActive(false);

        int e = 0;

        glintObj.SetActive(false);
        Color32 clr = colorRare;
        if (currentBuff.rarity == BuffRarity.Rare)
        {
            clr = colorRare;
            e = 1;
        }
        else if (currentBuff.rarity == BuffRarity.Epic)
        {
            clr = colorEpic;
            e = 2;
        }
        else if (currentBuff.rarity == BuffRarity.Legend)
        {
            clr = colorLegend;
            e = 3;
            glintObj.SetActive(true);
        }
        buffRarityBackground.color = clr;


        for (int i = 0; i < e; i++)
            stars[i].SetActive(true);
    }
}
