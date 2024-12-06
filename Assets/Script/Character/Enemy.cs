using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Enemy : Character
{
    [SerializeField] private Animator anim;
    [SerializeField] private float enemyAttackSpeed = 1f;
    private float enemyAttackTimer;
    private bool canAttack;

    [Space]
    [SerializeField] private List<PowerUp> powerUps;
    [SerializeField] private float powerUpDropChance;
    public override void Start()
    {
        base.Start();
        healthBar.HealthBarColor(new Color32(255, 0, 0, 255));
        //anim.GetComponent<Animator>();
    }

    public override void Update()
    {
        base.Update();
        canAttack = enemyAttackTimer >= enemyAttackSpeed;
        if (!canAttack) enemyAttackTimer += Time.deltaTime;
    }
    public override void TakeDamage(float _damage)
    {
        base.TakeDamage(_damage);
        anim.SetTrigger("HurtTrigger");

        if (IsDead()) Destroy(this.gameObject);
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<Character>() != null && canAttack)
        {
            OnHitWithCharacter(other.GetComponent<Character>());
        }
    }
    public override void OnHitWithCharacter(Character target)
    {
        if (target.GetComponent<Player>() != null)
        {
            enemyAttackTimer = 0;
            target.TakeDamage(10);
        }
    }
    public override void OnDead()
    {
        Debug.Log("OnDead used");
        Destroy(this.gameObject);
        
        Instantiate(deathParticle, this.transform.position, Quaternion.identity);
        
        AudioManager.instance.PlaySFX(1);

        if (powerUpDropChance >= Random.Range(0, 100))
            Instantiate(powerUps[Random.Range(0, powerUps.Count)], this.transform.position, Quaternion.identity);
        
    }
}
