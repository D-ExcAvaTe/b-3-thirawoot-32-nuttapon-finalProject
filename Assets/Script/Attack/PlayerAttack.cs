using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : AreaOfAttack
{
    public override void OnHitWithCharacter(Character target)
    {
        if (target.GetComponent<Enemy>() != null)
        {
            target.TakeDamage(damage);
            if( target.IsDead())
            {
                owner.GiveExp(target.Level * 5);
            }
            //Destroy(this.gameObject);
        }
    }
}
