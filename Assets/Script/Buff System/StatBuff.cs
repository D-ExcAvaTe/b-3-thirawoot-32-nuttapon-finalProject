using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StatBuff", menuName = "Buffs/StatBuff")]
public class StatBuff : Buff
{
    public float attackDamageIncrease;
    public float attackSpeedIncrease;
    public float movementSpeedIncrease;
    public float healthIncrease;

    public override void Apply()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            player.attackDamageBuff += attackDamageIncrease;
            player.movementSpeedBuff += movementSpeedIncrease;
            player.attackSpeedBuff += attackSpeedIncrease;
            player.maxHealthBuff += healthIncrease;

            Debug.Log($"Buff applied: +{attackDamageIncrease} Attack, +{movementSpeedIncrease} Movement Speed, +{healthIncrease} Health.");

            player.Init(player.Level,false);
        }
    }

    public override void Remove()
    {
        Player player = FindObjectOfType<Player>();

        if (player != null)
        {
            player.attackDamageBuff -= attackDamageIncrease;
            player.movementSpeedBuff -= movementSpeedIncrease;
            player.attackSpeedBuff -= attackSpeedIncrease;
            player.maxHealthBuff -= healthIncrease;

            Debug.Log("Buff removed.");
        }
    }

    public override void GetDescription()
    {
        description = "";
        if (attackDamageIncrease > 0) description += $"Attack Damage + {attackDamageIncrease}% ";
        if (attackSpeedIncrease > 0) description += $"Attack Speed + {attackSpeedIncrease}% ";
        if (movementSpeedIncrease > 0) description += $"Movement Speed + {movementSpeedIncrease} ";
        if (healthIncrease > 0) description += $"Health + {healthIncrease}% ";

    }
}
