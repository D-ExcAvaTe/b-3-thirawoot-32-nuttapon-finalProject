using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Player : Character
{
    [SerializeField] private AreaOfAttack areaOfAttack;
    private SpriteRenderer areaOfAttackRenderer;
    private float attackTimer = 0f;
    
    private PlayerController playerController;
    private Coroutine attackDelayCoroutine;
    
    public override void Start()
    {
        base.Start();
        Init(1,true,90,5);
        playerController = GetComponent<PlayerController>();
        
        healthBar.HealthBarColor(new Color32(0, 255, 0, 255));
        areaOfAttackRenderer = areaOfAttack.GetComponent<SpriteRenderer>();
    }

    public override void TakeDamage(float _damage)
    {
        if (playerController.isDashing) return;
        base.TakeDamage(_damage);
        playerController.TriggerHurt();
        
        if(IsDead()) playerController.TriggerDead();
    }

    public override void OnHitWithCharacter(Character target)
    {
       
    }

    public void Attack()
    {
        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        Collider2D col = areaOfAttack.GetComponent<Collider2D>();
        areaOfAttack.Init(AttackDamage, this);

        col.enabled = true;
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
    }

    public override void OnDead()
    {
        MovementSpeed = 0;
        AudioManager.instance.PlaySFX(0);
    }

    public void PowerUp(float _heal)
    {
        Health += _heal;
        Debug.Log($"Heal {_heal} nuay");
    }
    public void PowerUp(float _heal, bool isPercent)
    {
        Health += (_heal / 100 ) * MaxHealth;
        Debug.Log($"Heal {_heal} percent");
    }
}
