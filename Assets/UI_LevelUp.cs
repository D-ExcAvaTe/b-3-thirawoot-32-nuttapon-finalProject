using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UI_LevelUp : MonoBehaviour
{
    public GameObject panelLevelUp;
    [SerializeField] private Transform buffListContent;
    [SerializeField] private TextMeshProUGUI textBuffToGetAmount;
    [SerializeField] private List<Buff> buffToGive;
    [SerializeField] private int buffListAmount;
    public int buffToGetAmount;
    
    public static UI_LevelUp instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(instance.gameObject);
    }


    public void ShowLevelUpPanel()
    {
        foreach (Transform child in buffListContent)
            Destroy(child.gameObject);

        for (int i = 0; i < buffListAmount; i++)
        {
            Buff newBuffToGive = buffToGive[Random.Range(0, buffToGive.Count)];
            BuffIcon buffIcon = Instantiate(BuffInventory.instance.buffPrefabs,buffListContent);
            buffIcon.Init(newBuffToGive);

            Button buffButton = buffIcon.GetComponent<Button>();
            buffButton.onClick.AddListener(() => SelectBuff(newBuffToGive));
        }

        textBuffToGetAmount.text = $"Select buff ({buffToGetAmount})";
        
        panelLevelUp.SetActive(true);
        BuffInventory.instance.isLevelUpOpen = true;
    }

    private void SelectBuff(Buff newBuffToGive)
    {
        BuffInventory.instance.GiveBuff(newBuffToGive);
        
        panelLevelUp.SetActive(false);
        BuffInventory.instance.isLevelUpOpen = false;

        buffToGetAmount--;
        if (buffToGetAmount >= 1) ShowLevelUpPanel();
        else buffToGetAmount = 0;
    }
}
