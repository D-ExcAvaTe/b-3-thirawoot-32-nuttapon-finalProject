using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : HealthBar
{
    [SerializeField] private Slider attackTimerSlider;

    public override void InitAtkSpd(float attackTimer, float attackSpeed)
    {
        attackTimerSlider.value = attackTimer;
        attackTimerSlider.maxValue = attackSpeed;
    }
}
