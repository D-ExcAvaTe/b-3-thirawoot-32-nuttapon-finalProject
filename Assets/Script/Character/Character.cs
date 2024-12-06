using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    [SerializeField] private DamageIndicator damageIndicatorPrefab;
    [SerializeField] public HealthBar healthBar;
    [SerializeField] protected float healthbarOffset;
    [SerializeField] protected GameObject hitParticle;
    [SerializeField] protected GameObject deathParticle;
    
    private int level;
    private float maxExp;
    private float exp;
    private float maxHealth;
    private float health;
    private float attack;
    private float attackSpeed;
    private float critRate;
    private float critDam;
    private float movementSpeed;
    private float cooldownReduction;
    
    //base stat
    public int Level 
    {
        get => level;
        set  
        { 
            if (value < 0)
            {
                return;
            }
            else level = value;
        } 
    }
    public float Exp { get => exp; set => exp = value; }
    public float MaxExp { get => maxExp; set => maxExp = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public float Health
    {
        get => health;
        set
        {
            if (value < 0)
            {
                health = 0;
            }else if (value > MaxHealth)
            {
                health = MaxHealth;
            }
            else
            {
                health = value;
            }
            healthBar.Init(Health, MaxHealth, Level);
            
        }
    }
    public float AttackDamage { get => attack; set => attack = value; }
    public float AttackSpeed { get => attackSpeed; set => attackSpeed = value; }
    public float MovementSpeed
    {
        get => movementSpeed;
        set => movementSpeed = value;
    }

    //bonus stat
    public float attackDamageBuff;
    public float attackSpeedBuff;
    public float movementSpeedBuff;
    public float maxHealthBuff;
    public virtual void Start()
    {
        healthBar = Instantiate(healthBar,
            new Vector2(this.transform.position.x, this.transform.position.y + healthbarOffset), Quaternion.identity);
    }

    public virtual void Update()
    {
        if (healthBar != null) 
            healthBar.transform.position = new Vector2(this.transform.position.x, this.transform.position.y + healthbarOffset);
        
    }

    public void Init(int _level, bool updateHp = true, float baseHp = 10, float baseAtk = 1)
    {
        Level = _level;
        maxExp = 10 +  ( Level * 10f );
        MaxHealth = (baseHp + (Level * 5)) * (1 + (maxHealthBuff / 100));
        AttackDamage = (baseAtk + (Level * 2)) * (1 + (attackDamageBuff / 100));
        MovementSpeed = 1 + movementSpeedBuff;
        AttackSpeed = 1.5f + attackSpeedBuff;

        if (updateHp) Health = maxHealth;
        
        Debug.Log($"{this.name} level = {Level} , Hp = {Health}/{MaxHealth}, Damage = {AttackDamage}, Max EXP = {maxExp}, exp = {Exp} ");
        
        healthBar.Init(Health, MaxHealth, Level);
        
        if (IsExpOverload()) LevelUp();

    }

    public virtual void TakeDamage(float _damage)
    {
        Health -= _damage;
        if (IsDead())
        {
            OnDead();
            if (healthBar != null) 
                Destroy(healthBar.gameObject);
        } 
        
        Debug.Log($"{this.name} took {_damage} damage. HP = {Health}");
        
        Instantiate(hitParticle, this.transform.position, Quaternion.identity);

        AudioManager.instance.PlaySFX(2);

        DamageIndicator dmgIndicator = Instantiate(damageIndicatorPrefab, this.transform.position, Quaternion.identity);
        if (_damage > 0) dmgIndicator.Initialize(_damage);
        else dmgIndicator.Initialize(_damage, true);
    }
    public bool IsDead()
    {
        if (Health <= 0) 
        {
            Health = 0;
            //Debug.Log($"{this.name} is Dead!!");
            return true;
        }
        else
        {
//            Debug.Log($"{this.name} yang mai tai.");
            return false;
        }
    }

    public abstract void OnHitWithCharacter(Character target);
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Character>() != null)
        {
            OnHitWithCharacter(other.GetComponent<Character>());
        }
    }

    public abstract void OnDead();
    public void LevelUp() 
    {
        Level++;
        Exp -= maxExp;
        
        float previousHpPercent = Health / MaxHealth;
        Init(Level,  false);

        Health = MaxHealth * previousHpPercent;
        
        Debug.Log("Level UP!!!");
        AudioManager.instance.PlaySFX(7);
        
        UI_LevelUp.instance.buffToGetAmount++;
        UI_LevelUp.instance.ShowLevelUpPanel();
    }
    public void GiveExp(float _exp) 
    {
        Exp += _exp;
        Debug.Log($"exp + {_exp}");
        Debug.Log($"Current Exp = {Exp}/{maxExp}");
        if (IsExpOverload())
        {
            LevelUp();
        }
    }
    public bool IsExpOverload()
    {
        if( Exp >= maxExp)
        {
            return true;
        }
        else return false;
    }
}
