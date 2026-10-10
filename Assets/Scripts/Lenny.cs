using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Lenny : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 200f;
    private int frameSinceJump = 0;
    public int maxJumpFrames = 60;
    private bool isGrounded = false;
    public float baseJumpPower = 350f;
    private float jumpPower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        jumpPower = baseJumpPower;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void handleJump()
    {
        float xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        float ySpeed = -9.81f;

        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            float deltaJump = jumpPower * Time.deltaTime;

            rb.linearVelocity = new Vector2(xSpeed, deltaJump);
            frameSinceJump++;

            if (frameSinceJump > maxJumpFrames)
            {
                jumpPower = baseJumpPower;
                frameSinceJump = 0;
                isGrounded = false;
            }
        }
        else
        {
            rb.linearVelocity = new Vector2(xSpeed, rb.linearVelocity.y);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.gameObject.transform.rotation.z < 90 && collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleJump();
    }
}
