using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class Enemy : Character
{
    [SerializeField] private GameObject normalModeVisuals;
    [SerializeField] private GameObject inactiveModeVisuals;
    
    [SerializeField] private float enemyAttackSpeed = 1f;
    
    private float enemyAttackTimer;
    private bool canAttack;
    
    [Space]
    [SerializeField] private List<PowerUp> powerUps;
    [SerializeField] private float powerUpDropChance;

    [SerializeField] private EnemyMovement enemyMovement;

    [Header("Color")]
    [SerializeField] Color32 normalColor;
    [SerializeField] Color32 glitchColor;
    public override void Start()
    {
        base.Start();
        
        if(spawnParticle) Instantiate(spawnParticle, this.transform.position, Quaternion.identity);
        //anim.GetComponent<Animator>();
        
        UpdateEnemyVisuals(Player.CurrentDamageType);
        healthBar.HealthBarColor(normalColor);
    }

    private void OnEnable()
    {
        Player.OnPlayerDamageTypeChanged += UpdateEnemyVisuals;
    }
    private void UpdateEnemyVisuals(DamageType playerCurrentType)
    {
        bool isVulnerable = (playerCurrentType == weaknessType);

        if(isVulnerable) healthBar.HealthBarColor(normalColor);
        else healthBar.HealthBarColor(glitchColor);
        
        if (normalModeVisuals)
            normalModeVisuals.SetActive(isVulnerable);
        

        if (inactiveModeVisuals)
            inactiveModeVisuals.SetActive(!isVulnerable);
        

        enemyMovement.UpdateMovementSpeed( playerCurrentType,  weaknessType);
    }
    public override void Update()
    {
        base.Update();
        canAttack = enemyAttackTimer >= enemyAttackSpeed;
        if (!canAttack) enemyAttackTimer += Time.deltaTime;
    }
    
    public override void Init(int _level, bool updateHp = true)
    {
        //base.Init(_level, updateHp, baseHp, baseAtk);
        
        Level = _level;
        MaxExp = 10 + (Level * 10f);
        MaxHealth = (baseHp + (Level * 5)) * (1 + (maxHealthBuffPerLv / 100));
        AttackDamage = (baseAtk + (Level * 2)) * (1 + (attackDamageBuffPerLv / 100));
        MovementSpeed = 1 + movementSpeedBuffPerLv;
        AttackSpeed = 1.5f + attackSpeedBuffPerLv;

        if (updateHp)
            Health = MaxHealth;

        //Debug.Log($"{this.name} level = {Level} , Hp = {Health}/{MaxHealth}, Damage = {AttackDamage}, Max EXP = {MaxExp}, exp = {Exp} ");

        healthBar.Init(Health, MaxHealth, Level, CurrentOverHeal, MaxOverheal);

        if (IsExpOverload()) LevelUp();

    }
    
    public override void TakeDamage(float _damage, DamageType type)
    {
        base.TakeDamage(_damage, type);

        //anim.SetTrigger("HurtTrigger");

        enemyMovement.UpdateOverhealSpeed(CurrentOverHeal > 0f);
        Debug.Log($"Updated overhealspeed current:{CurrentOverHeal} result{CurrentOverHeal>0}");
        
        if (IsDead())
        {
            deathFeedback.PlayFeedbacks();
            Destroy(this.gameObject);
        }
        
    }
    public void OnTriggerStay2D(Collider2D other)
    {
        if (other.GetComponent<Character>() != null && canAttack)
        {
            OnHitWithCharacter(other.GetComponent<Character>(), damageType);
        }
    }
    public override void OnHitWithCharacter(Character target, DamageType type)
    {
        if (target.GetComponent<Player>() != null)
        {
            enemyAttackTimer = 0;
            target.TakeDamage(10, type);
        }
    }
    public override void OnDead()
    {
        Player.OnPlayerDamageTypeChanged -= UpdateEnemyVisuals;
        
        Instantiate(deathParticle, this.transform.position, Quaternion.identity);
        
        AudioManager.instance.PlaySFX(1);

        if (powerUpDropChance >= Random.Range(0, 100))
            Instantiate(powerUps[Random.Range(0, powerUps.Count)], this.transform.position, Quaternion.identity);
        
        if (ParticleDropAttractor.instance != null)
            ParticleDropAttractor.instance.SpawnLoot(this.transform.position);
        //Debug.Log("OnDead used");
        Destroy(this.gameObject);
    }
}
