using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Lenny : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 650f;
    private int frameSinceJump = 0;
    public int maxJumpFrames = 60;
    private bool isGrounded = false;
    private bool isHittingWall = false;
    public float baseJumpPower = 400f;
    private float jumpPower;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        jumpPower = baseJumpPower;

        rb.bodyType = RigidbodyType2D.Dynamic;
    }

    void handleJump(float xSpeed)
    {

        if (Input.GetKeyUp(KeyCode.Space))
        {
            jumpPower = baseJumpPower;
            frameSinceJump = 0;
            isGrounded = false;
        }

        if (isHittingWall) xSpeed = 0;

        else if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            Debug.Log("Here!");
            float deltaJump = jumpPower * Time.deltaTime;

            jumpPower /= 1.01f;
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

        Vector2 normalVector = collision.GetContact(0).normal;

        // Collided from the side, don't ground player
        if (Vector2.Dot(normalVector, Vector2.left) > 0.7f || Vector2.Dot(normalVector, Vector2.right) > 0.7f)
        {
            transform.position.Set(transform.position.x - speed, transform.position.y, transform.position.z);
            isHittingWall = true;
            return;
        }

        isHittingWall = false;

        if (collision.gameObject.transform.rotation.z < 90 && collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
        }


    }

    // Update is called once per frame
    void Update()
    {
        float xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        handleJump(xSpeed);

        if(xSpeed < 0)
        {
            this.transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
