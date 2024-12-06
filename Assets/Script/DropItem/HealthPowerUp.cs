using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPowerUp : PowerUp
{
    [SerializeField] float healAmount;

    public override void ApplyBuff(Player player)
    {
        player.PowerUp(healAmount);
    }
}
