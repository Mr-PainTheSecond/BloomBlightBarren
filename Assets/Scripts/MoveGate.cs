using System;
using UnityEngine;

public class MoveGate : MonoBehaviour
{
    public GameObject Lgate, Rgate;
    public SpawnEnemy spawnEnemy;

    private void Start()
    {
        Lgate.SetActive(false);
        Rgate.SetActive(false);
        //spawnEnemy = GetComponent<SpawnEnemy>();

    }

    private void Update()
    {
        if (spawnEnemy.waveComplete)
        {
            spawnEnemy.isSpawn = false;
            if(Lgate) Lgate.SetActive(false);
            if(Rgate) Rgate.SetActive(false);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("PLayer Passed gate");
            Lgate.SetActive(true);
            Rgate.SetActive(true);
            spawnEnemy.isSpawn = true;
        }
    }
}
