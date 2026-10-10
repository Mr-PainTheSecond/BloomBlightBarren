using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Rendering;

public class Lenny : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 650f;
    private float frameSinceJump = 0;
    public float maxJumpTime = 0.25f;
    private bool canJump = false;
    private bool climbingMode = false;
    private float climbingTimer = 0;
    public float climbingSpeed = 60f;
    private float maxClimbingTime = 10;
    private bool isGrounded = false;
    private bool isHittingWall = false;
    public float baseJumpPower = 400f;
    private float jumpPower;

    private Dictionary<Collider2D, Vector2> lastNormals;



    void Cache(Collision2D collision)
    {
        if (collision.contactCount > 0)
            lastNormals[collision.collider] = collision.GetContact(0).normal;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Application.targetFrameRate = 60;
        QualitySettings.vSyncCount = 0;

        rb = GetComponent<Rigidbody2D>();
        jumpPower = baseJumpPower;

        rb.bodyType = RigidbodyType2D.Dynamic;

        lastNormals = new Dictionary<Collider2D, Vector2>();
    }

    bool handleClimb(float xSpeed)
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            Debug.Log("Enterig Climb Mode!");
            Debug.Log($"{isHittingWall}");
            climbingMode = true;
        }

        else if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            climbingMode = false;
        }

        float ySpeed = Input.GetAxisRaw("Vertical") * climbingSpeed * Time.deltaTime;

        if (climbingMode && isHittingWall)
        {
            canJump = true;
        }


        // If u are holding down the climb button, hitting a wall, haven't run out of a stamina, and trying to climb
        if (climbingMode && isHittingWall && (climbingTimer < maxClimbingTime) && (ySpeed != 0))
        {
            Debug.Log("Trying to climb...");

            climbingTimer += Time.deltaTime;

            rb.linearVelocity = new Vector2(xSpeed, ySpeed);

            return true;
        } else
        {
            return false;
        }
    }

    void handleMovement(float xSpeed)
    {

        xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        if (isHittingWall && !climbingMode && !isGrounded) xSpeed = 0;

        if (isGrounded && Input.GetKey(KeyCode.Space))
        {
            float deltaJump = jumpPower * Time.deltaTime;

            rb.linearVelocity = new Vector2(xSpeed, deltaJump);
            frameSinceJump += Time.deltaTime;

            if (frameSinceJump > maxJumpTime)
            {
                jumpPower = baseJumpPower;
                frameSinceJump = 0;
                canJump = false;
                isGrounded = false;
            }
        }
        else if (isGrounded && Input.GetKeyUp(KeyCode.Space))
        {
            if (frameSinceJump > maxJumpTime)
            {
                jumpPower = baseJumpPower;
                frameSinceJump = 0;
                canJump = false;
                isGrounded = false;
            }
        }
        else
        {
            if (!handleClimb(xSpeed))
            {
                rb.linearVelocity = new Vector2(xSpeed, rb.linearVelocity.y);
            }

        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        Vector2 normalVector;

        if (lastNormals.TryGetValue(collision.collider, out normalVector))
        {
            // Exiting the collision of a wall
            if ((Vector2.Dot(normalVector, Vector2.left) > 0.7f || Vector2.Dot(normalVector, Vector2.right) > 0.7f))
            {
                isHittingWall = false;
                Debug.Log("Leaving wall");
                return;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        Vector2 normalVector = collision.GetContact(0).normal;

        Cache(collision);

        // Collided from the side, don't ground player
        if ((Vector2.Dot(normalVector, Vector2.left) > 0.7f || Vector2.Dot(normalVector, Vector2.right) > 0.7f))
        {
            isHittingWall = true;
            Debug.Log("Is Hitting Wall");
            return;
        }

        if (collision.gameObject.transform.rotation.z < 90 && collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            canJump = true;
            climbingTimer = 0;

        }

    }

    // Update is called once per frame
    void Update()
    {

        float xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;


        handleMovement(xSpeed);

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
