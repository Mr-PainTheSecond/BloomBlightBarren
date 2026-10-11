using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    public GameObject enemy; // add other later
    public int enemyCnt = 0;
    public int enemyMax = 0;
    
    public float minX, maxX, minY, maxY;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyCnt < enemyMax)
            SpawnsEnemy();
    }

    void SpawnsEnemy()
    {
        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        
        Vector2 pos = new Vector2(x, y);
        Instantiate(enemy, pos, Quaternion.identity);
        enemyCnt++;
    }
}
