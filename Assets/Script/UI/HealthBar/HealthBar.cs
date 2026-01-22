using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class HealthBar : MonoBehaviour
{
    [SerializeField] private Image fillColor; 
    [SerializeField] private Slider healthBarSlider;
    [SerializeField] private Slider overHealSlider;
    [SerializeField] private TextMeshProUGUI textLevel;
    public void Init(float _health, float _maxHealth, int _level, float _overHeal, float _maxOverheal)
    {
        healthBarSlider.maxValue = _maxHealth;
        healthBarSlider.value = _health;

        overHealSlider.value = _overHeal / _maxOverheal;

        textLevel.text = $"Lvl.{_level}";
    }

    public void HealthBarColor(Color32 healthColor)
    {
        fillColor.color = healthColor;
    }

    public void UpdateLevel(int _level)
    {
        textLevel.text = $"Lvl.{_level}";
    }

    public abstract void InitAtkSpd(float attackTimer, float attackSpeed);
}
