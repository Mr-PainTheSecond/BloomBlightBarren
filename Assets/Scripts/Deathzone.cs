using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Deathzone : MonoBehaviour
{
    //public int health = 3; //update w real health later
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Deathzone"))
        {
            Debug.Log("Player fell");
            SceneManager.LoadScene("MainScene");
        }
    }
}
