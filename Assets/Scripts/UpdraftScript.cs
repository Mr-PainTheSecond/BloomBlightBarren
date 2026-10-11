using System;
using Unity.VisualScripting;
using UnityEngine;

enum DraftDirection
{
    UP,
    DOWN,
    LEFT,
    RIGHT
}
public class UpdraftScript : MonoBehaviour
{

    [SerializeField] private float updraftStrength;
    [SerializeField] private DraftDirection draftDirection;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
            if (other.GetComponent<Lenny>().glideState == GlidingStates.GLIDING)
            {
                Debug.Log("Updraft!\n");
                Updraft(updraftStrength, draftDirection, rb);
            }
        }
    }

    void Updraft(float updraftStrength, DraftDirection draftDirection, Rigidbody2D rb)
    {
        if (draftDirection == DraftDirection.UP)
        {
            rb.AddForce(Vector2.up * updraftStrength, ForceMode2D.Force);
        }
        if (draftDirection == DraftDirection.DOWN)
        {
            rb.AddForce(Vector2.down * updraftStrength, ForceMode2D.Force);
        }
        if (draftDirection == DraftDirection.LEFT)
        {
            rb.AddForce(Vector2.left * updraftStrength, ForceMode2D.Force);
        }
        if (draftDirection == DraftDirection.RIGHT)
        {
            rb.AddForce(Vector2.right * updraftStrength, ForceMode2D.Force);
        }
    }
}
