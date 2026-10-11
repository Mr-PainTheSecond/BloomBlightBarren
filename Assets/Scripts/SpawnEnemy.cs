using Unity.VisualScripting;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    [SerializeField] private GameObject enemy; // add other later
    public int enemyCnt = 0;
    public int enemyMax = 0;
    
    public float minX, maxX, minY, maxY;

    public bool isSpawn = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCnt < enemyMax)
            SpawnsEnemy();

        for (int i = 0; i < enemyMax; i++)
        {
            if (enemy.IsDestroyed())
                enemyCnt = 0;
        }
    }

    void SpawnsEnemy()
    {
        if (isSpawn)
        {
            float x = Random.Range(minX, maxX);
            float y = Random.Range(minY, maxY);
        
            Vector2 pos = new Vector2(x, y);
            Instantiate(enemy, pos, Quaternion.identity);
            enemyCnt++;
        }
    }
}
