using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : AreaOfAttack
{
    public override void OnHitWithCharacter(Character target)
    {
        if (target.GetComponent<Enemy>() != null)
        {
            target.TakeDamage(damage, damageType);
            if( target.IsDead())
            {
                owner.GiveExp(target.Level * 1.25f + 2);
            }
            //Destroy(this.gameObject);
        }
    }
}
