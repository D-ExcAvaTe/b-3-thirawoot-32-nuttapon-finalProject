using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BuffInventory : MonoBehaviour
{
    public static BuffInventory instance;

    [SerializeField] private List<Buff> activeBuffs = new List<Buff>();

    [SerializeField] private Transform buffDisplayContent;
    public BuffIcon buffPrefabs;

    public bool isInventoryOpen = false;
    public bool isLevelUpOpen = false;
    public bool isGamePause = false;
    [SerializeField] private Animator buffDispalyAnim;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        isGamePause = isInventoryOpen || isLevelUpOpen;
    }

    public void GiveBuff(Buff newBuff)
    {
        activeBuffs.Add(newBuff);
        newBuff.Apply();
        Debug.Log($"Buff {newBuff.buffName} applied.");
    }

    public void RemoveBuff(Buff buff)
    {
        if (activeBuffs.Contains(buff))
        {
            buff.Remove();
            activeBuffs.Remove(buff);
            Debug.Log($"Buff {buff.buffName} removed.");
        }
    }

    public void ShowInventory()
    {
        isInventoryOpen = !isInventoryOpen;
        buffDispalyAnim.SetBool("isOpen", isInventoryOpen);
        
        activeBuffs.Sort((buff1, buff2) =>
        {
            int rarityComparison = buff2.rarity.CompareTo(buff1.rarity);
            if (rarityComparison != 0)
                return rarityComparison;

            return string.Compare(buff1.buffName, buff2.buffName, StringComparison.Ordinal);
        });
        
        foreach (Transform child in buffDisplayContent)
            Destroy(child.gameObject);

        foreach (var buff in activeBuffs)
        {
            BuffIcon buffIcon = Instantiate(buffPrefabs, buffDisplayContent);
            buffIcon.Init(buff);
        }
        
        RectTransform rectTransform = buffDisplayContent.GetComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, 250+((activeBuffs.Count/5)*250));
    }
}