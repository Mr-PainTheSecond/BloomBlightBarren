using UnityEngine;
using UnityEngine.UI;

public class LennyHealth : MonoBehaviour
{
    public Slider healthSlider;

    public int health = 3;
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
        if (collision.gameObject.CompareTag("Deathzone") ||  collision.gameObject.CompareTag("Enemies"))
        {
            Debug.Log("Player hit!");
        }
    }
}
