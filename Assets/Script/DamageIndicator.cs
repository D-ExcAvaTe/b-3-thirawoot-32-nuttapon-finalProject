using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textDamage;

    public void Initialize(float _damage, bool isHealing = false)
    {
        if (isHealing) textDamage.text = $"+{_damage.ToString("##")}";
        else textDamage.text = $"-{_damage.ToString("##")}";
    }
}
