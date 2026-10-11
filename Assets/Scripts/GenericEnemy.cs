using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;

public class GenericEnemy : MonoBehaviour
{


    public int HP;
    private int slashDamage = 1;
    private Rigidbody2D rb;
    private int fireDamage = 3;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        

        Lenny lenny = other.gameObject.GetComponent<Lenny>();
        
        if (lenny != null && lenny.attacks[(int)lenny.direction].enabled)
        {

            HP -= slashDamage;
        }

        Fire fire = other.gameObject.GetComponent<Fire>();

        if (fire != null)
        {
            HP -= fireDamage;
        }
        
        if (HP <= 0)
        {
            Destroy(gameObject);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
