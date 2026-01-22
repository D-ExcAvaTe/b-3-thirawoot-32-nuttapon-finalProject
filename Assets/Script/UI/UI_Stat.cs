using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Stat : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textWave, textNewWave, textStat, textLevel, textExp;
    [SerializeField] private Slider expSlider;
    
    private EnemySpawner enemySpawner;
    private Player player;

    private void Start()
    {
        player = FindObjectOfType<Player>();
        enemySpawner = FindObjectOfType<EnemySpawner>();
    }
    private void Update()
    {
        textLevel.text = $"Lv. {Fn(player.Level)}";
        textExp.text = $"Exp:{Fn(player.Exp)}/{Fn(player.MaxExp)}";

        expSlider.value = player.Exp;
        expSlider.maxValue = player.MaxExp;
        
        textStat.text = $"{Fn(player.Health)}/{Fn(player.MaxHealth)} (+{Fn(player.maxHealthBuffPerLv)}%)" +
                            $"\n{Fn(player.AttackDamage)} (+{Fn(player.attackDamageBuffPerLv)}%)" +
                            $"\n{Fn(player.AttackSpeed)} (+{Fn(player.attackSpeedBuffPerLv)})" +
                            $"\n{Fn(player.MovementSpeed)} (+{Fn(player.movementSpeedBuffPerLv)})";
        textWave.text = $"Wave {enemySpawner.wave}";

        if (enemySpawner.waitTimer >= enemySpawner.waitTime)
            textNewWave.text = $"Enemy spawned {-1*(enemySpawner.currentEnemies-enemySpawner.maxEnemy)}/{enemySpawner.maxEnemy}";
        else textNewWave.text = $"Next wave in {enemySpawner.waitTimer.ToString("#0")}";
    }

    private string Fn(float number)
    {
        return number.ToString("#0");
    }

}
