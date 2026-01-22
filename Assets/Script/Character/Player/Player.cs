using System.Collections;
using System.Collections.Generic;
using NUnit.Framework.Constraints;
using Unity.Mathematics;
using UnityEngine;

public enum PlayerAttackPattern
{
    Line,
    Cone
}

public class Player : Character
{
    [Header("Attack Pattern")]
    
    [SerializeField] private PlayerAttackPattern playerAttackPattern;
    [SerializeField] private AreaOfAttack areaOfAttackLine,areaOfAttackCone;
    [SerializeField] private float aaLineDmgMultiplier, aaConeDmgMultiplier;
    
    [Header("Damage Type Color")]
    [SerializeField] private Color32 dtNoneColor,dtPastColor,dtFutureColor;
    [Space]
    

    //private SpriteRenderer areaOfAttackRenderer;
    private float attackTimer = 0f;
    
    private PlayerController playerController;
    private Coroutine attackDelayCoroutine;
    
    public override void Start()
    {
        base.Start();
        Init(1,true);
        playerController = GetComponent<PlayerController>();
        
        healthBar.HealthBarColor(new Color32(0, 255, 0, 255));
        //areaOfAttackRenderer = areaOfAttack.GetComponent<SpriteRenderer>();
        
        //update all
        UpdateAttackAreaAttack();
        UpdateAttackAreaAttackColor();
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

        if (updateHp) Health = MaxHealth;

        Debug.Log(
            $"{this.name} level = {Level} , Hp = {Health}/{MaxHealth}, Damage = {AttackDamage}, Max EXP = {MaxExp}, exp = {Exp} ");

        healthBar.Init(Health, MaxHealth, Level);

        if (IsExpOverload()) LevelUp();

    }

    public override void TakeDamage(float _damage, DamageType type)
    {
        if (playerController.isDashing) return;
        base.TakeDamage(_damage, type);
        playerController.TriggerHurt();
        
        if(IsDead()) playerController.TriggerDead();
    }

    public override void OnHitWithCharacter(Character target, DamageType type)
    {
       
    }

    public void Attack()
    {
        StartCoroutine(AttackDelay());
    }

    private IEnumerator AttackDelay()
    {
        float finalDamage = AttackDamage * (aaLineDmgMultiplier + aaConeDmgMultiplier);
        
;        Collider2D col = GetAttackPattern().GetComponent<Collider2D>();
        GetAttackPattern().Init(finalDamage, this, damageType);

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

    void UpdateAttackAreaAttack()
    {
        bool isCone = GetAttackPattern() == areaOfAttackCone;
        areaOfAttackCone.gameObject.SetActive(isCone);
        areaOfAttackLine.gameObject.SetActive(!isCone);
        
        //dmg bonus
        if (!isCone)
        {
            aaLineDmgMultiplier = 0f;
            aaConeDmgMultiplier = 1f;
        }
        else
        {
            aaLineDmgMultiplier = 1f;
            aaConeDmgMultiplier = 1f;
        }
        
        GetAttackPattern().GetComponent<Collider2D>().enabled = false;
        
        DamageIndicator textIndicator = Instantiate(damageIndicatorPrefab,
            transform.position, Quaternion.identity);
        textIndicator.Initialize(playerAttackPattern.ToString(), false, true);
    }

    void UpdateAttackAreaAttackColor(bool showIndicator = true)
    {
        Color32 color = Color.white;
        switch (damageType)
        {
            case DamageType.None: color = dtNoneColor; break;
            case DamageType.Future: color = dtFutureColor; break;
            case DamageType.Past: color = dtPastColor; break;
        }

        GetAttackPattern().sr.color = color;

        if (!showIndicator) return;
        DamageIndicator textIndicator = Instantiate(damageIndicatorPrefab,
            transform.position, Quaternion.identity);
        textIndicator.Initialize(damageType.ToString(), false, true);
    }

    AreaOfAttack GetAttackPattern()
    {
        if (playerAttackPattern == PlayerAttackPattern.Cone) return areaOfAttackCone;
        else return areaOfAttackLine;
    }

    public void ToggleSwitchAttackPattern()
    {
        if (playerAttackPattern == PlayerAttackPattern.Cone) playerAttackPattern = PlayerAttackPattern.Line;
        else playerAttackPattern = PlayerAttackPattern.Cone;

        UpdateAttackAreaAttack();
        UpdateAttackAreaAttackColor(false);
    }

    public void ToggleSwitchDamageType()
    {
        if (damageType == DamageType.Future) damageType = DamageType.Past;
        else if (damageType == DamageType.Past) damageType = DamageType.Future;
        
        UpdateAttackAreaAttackColor();
    }


}
