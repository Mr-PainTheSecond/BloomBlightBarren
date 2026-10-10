using UnityEngine;
using UnityEngine.UI;
using  UnityEngine.SceneManagement;

public class LennyHealth : MonoBehaviour
{
    public Slider healthSlider;
    public Lenny lenny;
    public int health = 3;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lenny = gameObject.GetComponent<Lenny>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health == 0)
        {
            SceneManager.LoadScene("MainScene");
            Debug.Log("Player died :[");
        }
            
        
    }
    
    void OnCollisionEnter2D(Collision2D collision)
    {
        switch (collision.gameObject.tag)   
        {
            case "Deathzone":
                health--;
                healthSlider.value = health;
                gameObject.transform.position = lenny.lastGroundPosition;
                break;
            case "Enemies":
                if (!lenny.attacks[(int)lenny.direction].enabled)
                {
                    health--;
                    healthSlider.value = health;
                    gameObject.transform.Translate(-(int)lenny.direction, 0, 0);
                }
                break;
            default:
                return;
        }
    }
}
