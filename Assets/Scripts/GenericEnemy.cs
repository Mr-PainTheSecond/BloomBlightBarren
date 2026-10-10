using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Lenny lenny = other.gameObject.GetComponent<Lenny>();
        
        if (lenny != null && lenny.attacks[(int)lenny.direction].enabled)
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
