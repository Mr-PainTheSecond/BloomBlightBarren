using UnityEngine;

public class TrapObstacle : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public BoxCollider2D initialTrap;
    public CapsuleCollider2D effectRadius;
    private float damagerTimer = 0f;
    private GameObject player;
    private float criticalTime = 8f;
    private LennyHealth lennyHealth;
    private bool justActivated = false;

    bool active = false;

    void Start()
    {
        effectRadius.enabled = false;
    }

    void OnTrigerEnter2D(Collider2D collider)
    {
        Debug.Log("We got here!");
        if (collider.tag == "Player")
        {
            if (initialTrap.enabled)
            {
                player = collider.gameObject;
                lennyHealth = collider.gameObject.GetComponent<LennyHealth>();
                Debug.Log("Trap Activated");
                initialTrap.enabled = false;
                effectRadius.enabled = true;
                active = true;
                justActivated = true;
            } else if (!active)
            {
                active = true;
            }

        }
    }

    void OnTriggerStay2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            if (initialTrap.enabled)
            {
                player = collider.gameObject;
                lennyHealth = collider.gameObject.GetComponent<LennyHealth>();
                Debug.Log("Trap Activated");
                initialTrap.enabled = false;
                effectRadius.enabled = true;
                active = true;
                justActivated = true;
            }
            else if (!active)
            {
                active = true;
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        Debug.Log("Left a trigger!");

        if (collider.tag == "Player" && active && !justActivated)
        {
            Debug.Log("Left area!");
            damagerTimer = 0; ;
            active = false;
        } else if (collider.tag == "Player" && active)
        {
            justActivated = false;
        }
    }

    // Update is called once per frame
    void Update()
    {


        if (active)
        {
            

            damagerTimer += Time.deltaTime;

            if (damagerTimer > criticalTime)
            {
                lennyHealth.health--;
                lennyHealth.healthSlider.value = lennyHealth.health;
                Debug.Log("Taking Damamge!");
                damagerTimer = 0;
            }
        }
    }
}
