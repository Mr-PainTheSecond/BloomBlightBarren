using UnityEngine;

public class BlightShroom : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 200f;
    private int magnitude = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            Vector2 normalVector = collision.GetContact(0).normal;
            if (Vector2.Dot(normalVector, Vector2.left) > 0.7f || Vector2.Dot(normalVector, Vector2.right) > 0.7f)
            {

                Debug.Log("Wall Hit!");
                magnitude *= -1;
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Collision Event");
        if (collider.CompareTag("Wall"))
        {
            Debug.Log("COLLIDING!!!");
            magnitude *= -1;
        }
    }

    // Update is called once per frame
    void Update()
    {
        float speedX = speed * magnitude * Time.deltaTime;

        rb.linearVelocity = new Vector2(speedX, rb.linearVelocity.y);

        if (speedX < 0)
        {
            this.transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
