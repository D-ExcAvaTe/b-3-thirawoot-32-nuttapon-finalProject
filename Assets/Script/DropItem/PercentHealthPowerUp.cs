using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PercentHealthPowerUp : PowerUp
{
    [SerializeField] float healPercent;

    public override void ApplyBuff(Player player)
    {
        player.PowerUp(healPercent, true);
    }
}
