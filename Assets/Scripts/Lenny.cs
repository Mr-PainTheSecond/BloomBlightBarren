using System.Collections.Generic;
using System.Net.NetworkInformation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public enum GlidingStates
{
    NO_GLIDE,
    PREPARING_GLIDE,
    GLIDING
}

public enum Direction
{
    LEFT,
    RIGHT,
    UP,
    DOWN
}

public class Lenny : MonoBehaviour
{

    private Rigidbody2D rb;
    public float speed = 650f;
    private float frameSinceJump = 0;
    public float maxJumpTime = 0.25f;
    private int jumpCount = 0;
    private bool canJump = false;
    private bool jumpKeyPressed = false;
    private bool climbingMode = false;
    private bool jumping = false;
    private float climbingTimer = 0;
    public float climbingSpeed = 60f;
    private float maxClimbingTime = 10;
    private bool isGrounded = false;
    private bool isHittingWall = false;
    public float baseJumpPower = 300f;
    private float jumpPower;
    private GlidingStates glideState = GlidingStates.NO_GLIDE;
    private float glidingTimer = 0f;
    public float glidingWait = 0.75f;
    public Vector2 lastGroundPosition;

    private Dictionary<Collider2D, Vector2> lastNormals;

    public BoxCollider2D leftAttack;
    public BoxCollider2D rightAttack;
    public BoxCollider2D upAttack;
    public BoxCollider2D downAttack;
    public BoxCollider2D[] attacks;
    public BoxCollider2D activeColliders = null;

    public Direction direction;


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

        leftAttack.enabled = false;
        rightAttack.enabled = false;
        upAttack.enabled = false;
        downAttack.enabled = false;

        direction = Direction.RIGHT;

        attacks = new BoxCollider2D[4];

        attacks[(int)Direction.LEFT] = leftAttack;
        attacks[(int)Direction.RIGHT] = rightAttack;
        attacks[(int)Direction.UP] = upAttack;
        attacks[(int)Direction.DOWN] = downAttack;

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
            jumping = false;
            jumping = false;
            Debug.Log("Climbing...");
            canJump = true;
            resetGlide();
            jumpCount = 0;
        }


        // If u are holding down the climb button, hitting a wall, haven't run out of a stamina, and trying to climb
        if (canClimb())
        {
            Debug.Log("Trying to climb...");

            if (ySpeed == 0)
            {
                climbingTimer += Time.deltaTime / 4;
            } else
            {
                climbingTimer += Time.deltaTime;
            }


            rb.linearVelocity = new Vector2(xSpeed, ySpeed);

            return true;
        } else
        {
            return false;
        }
    }

    public bool canClimb()
    {
        return climbingMode && isHittingWall && (climbingTimer < maxClimbingTime);
    }

    void handleMovement(float xSpeed)
    {

        xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpKeyPressed = true;
            // just in case
            rb.gravityScale = 1f;
        }

        if (canJump && Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Jump Counter Incremented!");

            jumping = true;
            jumpCount++;
            resetGlide();
        }

        if (isHittingWall && !climbingMode && !isGrounded) xSpeed = 0;

        if (canJump && Input.GetKey(KeyCode.Space))
        {
            Debug.Log($"Jump Counter {jumpCount}");

            float deltaJump = jumpPower * Time.deltaTime;

            if (rb.gravityScale != 1) rb.gravityScale = 1;

            rb.linearVelocity = new Vector2(xSpeed, deltaJump);
            frameSinceJump += Time.deltaTime;

            Debug.Log(frameSinceJump);

            if (frameSinceJump > maxJumpTime)
            {
                jumpPower = baseJumpPower;
                frameSinceJump = 0;
                isGrounded = false;
                canJump = false;
                glideState = GlidingStates.PREPARING_GLIDE;
            }
        }
        else if (canJump && Input.GetKeyUp(KeyCode.Space))
        {

            Debug.Log($"Jump Counter {jumpCount}");

            frameSinceJump = 0;
            canJump = false;
            isGrounded = false;
            jumpKeyPressed = false;

            resetGlide();

        }
        else
        {
            if (!handleClimb(xSpeed))
            {
                rb.linearVelocity = new Vector2(xSpeed, rb.linearVelocity.y);
            }

        }



        if ((Input.GetKeyUp(KeyCode.Space) || !jumpKeyPressed) && !canJump && jumpCount < 2)
        {
            jumpKeyPressed = false;
            canJump = true;
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

            else if (Vector2.Dot(normalVector, Vector2.up) > 0.7f)
            {
                Debug.Log("Leaving floor");
                lastGroundPosition = gameObject.transform.position;
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
            Debug.Log("Hit ground");
            if (!jumpKeyPressed)
            {
                canJump = true;
            }
            climbingTimer = 0;
            jumpCount = 0;

            jumping = false;

            resetGlide();

        }
        
        if (collision.gameObject.CompareTag("Deathzone"))
        {
            Debug.Log("Player fell");
            gameObject.transform.position = lastGroundPosition;
        }

    }

    public void resetGlide()
    {
        glideState = GlidingStates.NO_GLIDE;
        glidingTimer = 0;
        rb.gravityScale = 1;

    }

    void handleGliding(float xSpeed)
    {

        if (Input.GetKeyUp(KeyCode.Space))
        {
            resetGlide();
        }

        if (glideState == GlidingStates.PREPARING_GLIDE)
        {
            glidingTimer += Time.deltaTime;

            if (glidingTimer > glidingWait)
            {
                Debug.Log("Gliding!");
                glideState = GlidingStates.GLIDING;
                glidingTimer = 0;

                canJump = false;
                jumpCount++;

                rb.gravityScale = 0.1f;
            }
        }
    }

    bool canDownAttack()
    {
        return jumping || (canClimb());
    }

    void handleAttacks()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (activeColliders == null && direction != Direction.DOWN || canDownAttack())
            {
                attacks[(int)direction].enabled = true;
            }

            activeColliders = attacks[(int)direction];
            Debug.Log("Enabling attack!");
        }


    }

    void getDirection(float x, float y)
    {
        // Since up/down is rarer than left/right, they take attack priority
       if (y > 0)
       {
            direction = Direction.UP;
       } else if (y < 0)
       {
            direction = Direction.DOWN;
       }
       else if (x > 0)
       {
            direction = Direction.RIGHT;
       } else if (x < 0)
       {
            direction = Direction.LEFT;
       }
       else
       {
            direction = Direction.RIGHT;
       }
    }

    // Update is called once per frame
    void Update()
    {

        float xSpeed = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        float ySpeed = Input.GetAxisRaw("Vertical");

        handleMovement(xSpeed);

        getDirection(xSpeed, ySpeed);

        // Sometimes, velocity can be 0 in practice, but not the number stored in memory
        if (rb.linearVelocityY < 0 && !(Utilities.approxEqual(rb.linearVelocityY, 0, 1e-5f) && !jumping && glideState == GlidingStates.NO_GLIDE))
        {
            if (jumpCount < 1) jumpCount = 1;
            jumping = true;
            glideState = GlidingStates.PREPARING_GLIDE;
        }

        handleGliding(xSpeed);

        handleAttacks();

        if (xSpeed < 0)
        {
            this.transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
