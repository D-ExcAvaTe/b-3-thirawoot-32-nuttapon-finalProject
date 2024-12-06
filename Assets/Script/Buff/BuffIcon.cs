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

    public Buff currentBuff;
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

        if (currentBuff.rarity == BuffRarity.Rare)
        {
            textBuffName.color = new Color32(148, 255, 155, 255);
            e = 1;
        }
        else if (currentBuff.rarity == BuffRarity.Epic)
        {
            textBuffName.color = new Color32(230, 148, 255, 255);
            e = 2;
        }
        else if (currentBuff.rarity == BuffRarity.Legend)
        {
            textBuffName.color = new Color32(255, 221, 148, 255);
            e = 3;
        }

        for (int i = 0; i < e; i++)
            stars[i].SetActive(true);
    }
}
