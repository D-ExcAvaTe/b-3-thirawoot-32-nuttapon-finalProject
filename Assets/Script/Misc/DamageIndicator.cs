using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDamage;

    [Space]
    [SerializeField] private Color32 damageColor = Color.white; 
    [SerializeField] private Color32 healColor = Color.lawnGreen;

    public void Initialize(string _damage, bool isHealing = false, bool isText = false)
    {
        if (isText)
        {
            textDamage.text = _damage;
            return;
        }
        
        if (isHealing) textDamage.text = $"+{_damage}";
        else textDamage.text = $"-{_damage}";

        if (isHealing) textDamage.color = healColor;
        else textDamage.color = damageColor;
    }
}
