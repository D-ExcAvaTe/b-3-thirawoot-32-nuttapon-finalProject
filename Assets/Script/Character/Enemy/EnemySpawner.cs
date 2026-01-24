using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Transform> spawnPoints; 
    [SerializeField] private List<Enemy> enemies;
    [SerializeField] private float spawnDelay;
    [SerializeField] private int enemyLevel;
    
    public int maxEnemy;
    public int currentEnemies;
    public int wave;
    public float waitTime = 20f;
    public float waitTimer;

    private void Start()
    {
        StartCoroutine(WaveLoop());
    }
    private void Update()
    {
        if(BuffInventory.instance.isGamePause) return;
        if (waitTimer < 0) waitTimer = 0;
        else if (waitTimer > 0 && currentEnemies <= 0) waitTimer -= Time.deltaTime;
    }
    private void SpawnEnemy()
    {
        Enemy newEnemy = Instantiate(enemies[Random.Range(0,enemies.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
        //Enemy newEnemy = Instantiate(enemies[Random.Range(0,enemies.Count)], spawnPoints[Random.Range(0,spawnPoints.Count)].position, Quaternion.identity);
        newEnemy.Init(Random.Range( enemyLevel - 3, enemyLevel ));
    }
    private IEnumerator SpawnDelay()
    {
        /*while (BuffInventory.instance.isGamePause)
            yield return null;*/
        
        if (currentEnemies > 0)
        {
            for (int i = 0; i < maxEnemy; i++)
            {
                // รอ pause
                while (BuffInventory.instance.isGamePause)
                    yield return null;

                Debug.Log("new summon");
                SpawnEnemy();
                currentEnemies--;
                yield return new WaitForSeconds(spawnDelay);
            }
        }
    }
    private void NewWave()
    {
        waitTimer = waitTime;
        Debug.Log("New wave");
        wave++;
        maxEnemy = (wave * 4) + 5;
        currentEnemies = maxEnemy;
        enemyLevel = ( wave + 1 ) * 2; 
        StartCoroutine(SpawnDelay());
    }
    private IEnumerator WaveLoop()
    {
        while (true)
        {
            // รอ pause
            while (BuffInventory.instance.isGamePause)
                yield return null;

            NewWave();
            /*yield return new WaitForSeconds(maxEnemy * spawnDelay); -->1
            yield return new WaitForSeconds(waitTime);                -->2 */
            // รอ wave ก้็ต้อง check pause
            float waveTime = maxEnemy * spawnDelay;
            float timer = 0f;

            while(timer < waveTime) // -->1
        {
                if (!BuffInventory.instance.isGamePause)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }

            timer = 0f; // -->2
            while (timer < waitTime)
            {
                if (!BuffInventory.instance.isGamePause)
                {
                    timer += Time.deltaTime;
                }
                yield return null;
            }
        }
    }
}
