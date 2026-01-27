using System.Collections;
using System.Collections.Generic;
using MoreMountains.Feedbacks;
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
    [SerializeField] private MMF_Player coneSfxFeedback, lineSfxFeedback;
    public static DamageType CurrentDamageType { get; private set; }
    public static System.Action<DamageType> OnPlayerDamageTypeChanged;
    
    [Header("Weapon Visuals")]
    [SerializeField] private SpriteRenderer weaponRenderer; 
    [SerializeField] private Sprite weaponConeSprite;    
    [SerializeField] private Sprite weaponLineSprite;

    [Header("External Dependencies")] [SerializeField]
    private PlayerController playerController;

    [SerializeField] private BackgroundPatternManager backgroundManager;

    [Header("Attack Pattern")] [SerializeField]
    private PlayerAttackPattern playerAttackPattern;

    [SerializeField] private AreaOfAttack areaOfAttackLine, areaOfAttackCone;
    [SerializeField] private float aaLineDmgMultiplier, aaConeDmgMultiplier;

    [Header("Switch Mechanics")] [SerializeField]
    private float switchCooldown = 1.0f;

    private float lastSwitchTime = -999f;

    [Header("Damage Type Color")] [SerializeField]
    private Color32 dtNoneColor, dtPastColor, dtFutureColor;

    [Space]

    //private SpriteRenderer areaOfAttackRenderer;
    private float attackTimer = 0f;

    private Coroutine attackDelayCoroutine;


    private Collider2D colCone, colLine;
    public override void Start()
    {
        OnPlayerDamageTypeChanged = null;
        
        base.Start();
        Init(1, true);
        playerController = GetComponent<PlayerController>();

        healthBar.HealthBarColor(new Color32(0, 255, 0, 255));
        if(backgroundManager != null)
        {
            backgroundManager.ForceSetBackground(playerAttackPattern);
        }
        ApplyAttackState(playerAttackPattern, true); // Added 'true' to skip anim on start

        colCone = areaOfAttackCone.GetComponent<Collider2D>();
        colLine = areaOfAttackLine.GetComponent<Collider2D>();
    }

    public override void Init(int _level, bool updateHp = true)
    {
        //base.Init(_level, updateHp, baseHp, baseAtk);

        Level = _level;
        MaxExp = 10 + (Level * 15f);
        MaxHealth = (baseHp + (Level * 5)) * (1 + (maxHealthBuffPerLv / 100));
        AttackDamage = (baseAtk + (Level * 2)) * (1 + (attackDamageBuffPerLv / 100));
        MovementSpeed = 1 + movementSpeedBuffPerLv;
        AttackSpeed = 1.5f + attackSpeedBuffPerLv;

        if (updateHp) Health = MaxHealth;

        //Debug.Log($"{this.name} level = {Level} , Hp = {Health}/{MaxHealth}, Damage = {AttackDamage}, Max EXP = {MaxExp}, exp = {Exp} ");

        healthBar.Init(Health, MaxHealth, Level, CurrentOverHeal, MaxOverheal);

        if (IsExpOverload()) LevelUp();

        // ToggleSwitchAttackPattern();
        // ToggleSwitchDamageType();
    }

    public override void TakeDamage(float _damage, DamageType type)
    {
        if (playerController.isDashing) return;
        base.TakeDamage(_damage, type);
        playerController.TriggerHurt();

        if (IsDead()) playerController.TriggerDead();
    }

    public override void OnHitWithCharacter(Character target, DamageType type)
    {

    }

    public void Attack()
    {
        StartCoroutine(AttackDelay());
    }

    [SerializeField] private ParticleSystem[] attackParticleCone;
    [SerializeField] private ParticleSystem[] attackParticleLine;
    private IEnumerator AttackDelay()
    {
        if (playerAttackPattern == PlayerAttackPattern.Cone)
        {
            foreach (var p in attackParticleCone) if (p) p.Play();
            coneSfxFeedback.PlayFeedbacks();
        }
        else
        {
            foreach (var pp in attackParticleLine) if (pp) pp.Play();
            lineSfxFeedback.PlayFeedbacks();
        }
        
        float finalDamage = AttackDamage * (aaLineDmgMultiplier + aaConeDmgMultiplier);
        
        Collider2D col = GetAttackPattern().GetComponent<Collider2D>();
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
        //Debug.Log($"Heal {_heal} nuay");
    }

    public void PowerUp(float _heal, bool isPercent)
    {
        Health += (_heal / 100) * MaxHealth;
        //Debug.Log($"Heal {_heal} percent");
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
            case DamageType.None:
                color = dtNoneColor;
                break;
            case DamageType.Future:
                color = dtFutureColor;
                break;
            case DamageType.Past:
                color = dtPastColor;
                break;
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
        // Cooldown Check
        if (Time.time < lastSwitchTime + switchCooldown) return;

        lastSwitchTime = Time.time;


        colCone.enabled = false;
        colLine.enabled = false;
        
        // Toggle Enum
        if (playerAttackPattern == PlayerAttackPattern.Cone)
            playerAttackPattern = PlayerAttackPattern.Line;
        else
            playerAttackPattern = PlayerAttackPattern.Cone;

        ApplyAttackState(playerAttackPattern, false);
    }

    private void ApplyAttackState(PlayerAttackPattern pattern, bool isStartup)
    {
        bool isCone = pattern == PlayerAttackPattern.Cone;
        
        areaOfAttackCone.gameObject.SetActive(isCone);
        areaOfAttackLine.gameObject.SetActive(!isCone);
        
        if (weaponRenderer != null)
            weaponRenderer.sprite = isCone ? weaponConeSprite : weaponLineSprite;
       
        if (isCone)
        {
            damageType = DamageType.Future ;
            aaLineDmgMultiplier = 1f;
            aaConeDmgMultiplier = 1f;
        }
        else
        {
            damageType = DamageType.Past;
            aaLineDmgMultiplier = 1f;
            aaConeDmgMultiplier = 0f;
        }

        CurrentDamageType = damageType;
        OnPlayerDamageTypeChanged?.Invoke(damageType);
        
        UpdateVisuals(pattern, damageType);
        
        if(backgroundManager != null && !isStartup)
            backgroundManager.SwitchBackground(pattern);
    }

    private void UpdateVisuals(PlayerAttackPattern pattern, DamageType type)
    {
        Color32 color = (type == DamageType.Past) ? dtPastColor : dtFutureColor;
        GetAttackPatternObj().sr.color = color;

        DamageIndicator textIndicator = Instantiate(damageIndicatorPrefab, transform.position, Quaternion.identity);
        string indicatorText = $"{pattern} ({type})";
        textIndicator.Initialize(indicatorText, false, true);
    }

    private AreaOfAttack GetAttackPatternObj()
    {
        return playerAttackPattern == PlayerAttackPattern.Cone ? areaOfAttackCone : areaOfAttackLine;
    }
}
