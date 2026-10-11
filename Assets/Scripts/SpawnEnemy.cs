using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemy; // add other later
    public int enemyCnt = 0;
    public int enemyMax = 0;
    
    public int enemiesKilled = 0;
    
    public float minX, maxX, minY, maxY;

    public bool isSpawn = false;
    
    private bool enemiesSpawned = false;
    public bool waveComplete = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if (enemyCnt < enemyMax)
            SpawnsEnemy();
        */
        if (!enemiesSpawned && isSpawn)
        {
            SpawnEnemies(enemyCnt);
        }

        if (enemiesKilled == enemyCnt)
        {
            waveComplete = true;
        }
 
    }

    void SpawnsEnemy()
    {
        if (isSpawn)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
        
            Vector2 pos = new Vector2(x, y);
            GameObject newEnemy = Instantiate(enemy, pos, Quaternion.identity);
            GenericEnemy enemyScript = newEnemy.GetComponent<GenericEnemy>();
            enemyScript.spawn = this;
            enemyCnt++;
        }
    }

    void SpawnEnemies(int enemyCount)
    {
        for (int i = 0; i < enemyCnt; i++)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
        
            Vector2 pos = new Vector2(x, y);
            GameObject newEnemy = Instantiate(enemy, pos, Quaternion.identity);
            GenericEnemy enemyScript = newEnemy.GetComponent<GenericEnemy>();
            enemyScript.spawn = this;
        }
        enemiesSpawned = true;
    }
}
