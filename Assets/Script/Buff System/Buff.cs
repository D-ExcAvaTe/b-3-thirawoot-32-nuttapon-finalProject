using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class Buff : ScriptableObject
{
    public string buffName;
    public string description;
    public Sprite buffIcon;
    public BuffRarity rarity;
    public abstract void Apply();
    public abstract void Remove();

    public abstract void GetDescription();
}

public enum BuffRarity
{
    Rare,
    Epic,
    Legend
}
