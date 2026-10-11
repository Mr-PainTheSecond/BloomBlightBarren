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

public enum FireStages
{
    REGULAR,
    PREPARING_FIRE,
    DRAWING_FIRE
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
    public float fireOffset = 200f;
    private float jumpPower;
    public GlidingStates glideState = GlidingStates.NO_GLIDE;
    private float glidingTimer = 0f;
    public float glidingWait = 0.75f;
    public Vector2 lastGroundPosition;
    public GameObject fireObj;
    private GameObject activeFire;

    private Dictionary<Collider2D, Vector2> lastNormals;

    public BoxCollider2D leftAttack;
    public BoxCollider2D rightAttack;
    public BoxCollider2D upAttack;
    public BoxCollider2D downAttack;
    public BoxCollider2D[] attacks;
    public float attackTimer = 0;
    public BoxCollider2D activeColliders = null;

    public FireStages fireStage = FireStages.REGULAR;
    public Direction direction;
    public Direction facingDirection;

    private float currentFireTimer = 0;
    public float[] fireTimers;

    public LennyChangeSprite lcSprite;
    
    public Animator anim;

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


        facingDirection = Direction.RIGHT;
        direction = Direction.RIGHT;
        rb.freezeRotation = true;

        attacks = new BoxCollider2D[4];

        attacks[(int)Direction.LEFT] = leftAttack;
        attacks[(int)Direction.RIGHT] = rightAttack;
        attacks[(int)Direction.UP] = upAttack;
        attacks[(int)Direction.DOWN] = downAttack;

        lastNormals = new Dictionary<Collider2D, Vector2>();

        fireTimers = new float [3] { 0, 0.5f, 1.5f};
        
        lcSprite = GetComponent<LennyChangeSprite>();
        
        anim = GetComponent<Animator>();
   
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
            Debug.Log("Climbing...");
            canJump = true;
            resetGlide();
            jumpCount = 0;
        }

        lcSprite.ReturnSprite();
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (activeColliders != null)
            {
                activeColliders.enabled = false;
                activeColliders = null;
                attackTimer = 0;
            }
            jumpKeyPressed = true;
            // just in case
            rb.gravityScale = 1f;
        }

        if (fireStage != FireStages.REGULAR)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocityY);
            return;
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
                if (glideState != GlidingStates.GLIDING) glideState = GlidingStates.PREPARING_GLIDE;
            }
            
            lcSprite.ChangeSprite(lcSprite.jump);
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
/*
        if (xSpeed != 0)
        {
            lcSprite.cycle = true;
            //lcSprite.ChangeSprite(lcSprite.walk1);
            lcSprite.CycleSprites(lcSprite.walk1, lcSprite.walk2);
        }
        else if (xSpeed == 0 && !canJump)
        {
            lcSprite.ReturnSprite();
            lcSprite.cycle = false;
        }
        */    
            
    if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
        anim.SetTrigger("isMove");
        

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
            lcSprite.ReturnSprite();
            rb.linearVelocityY = 0;
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

                if(!isGrounded) canJump = false;
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
            Direction attackingDirection;
            if (direction == Direction.UP || direction == Direction.DOWN)
            {
                attackingDirection = direction;
            }
            else
            {
                Debug.Log("From Facing Direction!");
                attackingDirection = facingDirection;
            }

            if ((attackingDirection != Direction.DOWN || canDownAttack()) && activeColliders == null)
            {
                Debug.Log(attackingDirection);
                attacks[(int)attackingDirection].enabled = true;
                activeColliders = attacks[(int)attackingDirection];
                Debug.Log("Enabling attack!");
                lcSprite.ChangeSprite(lcSprite.atkFront);
            }

           
        } else if (activeColliders != null)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer > glidingTimer)
            {
                activeColliders.enabled = false;
                activeColliders = null;
                attackTimer = 0;
                lcSprite.ReturnSprite();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && isGrounded && fireStage == FireStages.REGULAR)
        {
            if (activeColliders != null)
            {
                activeColliders.enabled = false;
                activeColliders = null;
            }
            attackTimer = 0;

            fireStage++;
        }

        if (fireStage == FireStages.PREPARING_FIRE)
        {
            currentFireTimer += Time.deltaTime;

            if (currentFireTimer >= fireTimers[(int)fireStage])
            {
                float localFireOffset = fireOffset;
                float rotationVariance = 90;

                if (direction == Direction.LEFT)
                {
                    localFireOffset = -fireOffset;
                    rotationVariance = 270;
                }

                Debug.Log("Finished preparing the fire");
                fireStage = FireStages.DRAWING_FIRE;
                activeFire = Instantiate(fireObj, new Vector3(transform.position.x + localFireOffset, transform.position.y, transform.position.z), new Quaternion(0, 0, 0, 0));

                activeFire.transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationVariance));

                currentFireTimer = 0;
                lcSprite.ChangeSprite(lcSprite.breathFire);
            }
        }
        else if (fireStage == FireStages.DRAWING_FIRE)
        {
            currentFireTimer += Time.deltaTime;

            if (currentFireTimer >= fireTimers[(int)fireStage])
            {
                Destroy(activeFire);
                currentFireTimer = 0;
                activeFire = null;
                fireStage = FireStages.REGULAR;
                
                lcSprite.ReturnSprite();
            }
        }


    }

    void getDirection(float x, float y)
    {
        if (fireStage == FireStages.DRAWING_FIRE)
        {
            return;
        }
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
            facingDirection = Direction.RIGHT;
       } else if (x < 0)
       {
            direction = Direction.LEFT;
            facingDirection = Direction.LEFT;
       }
       else
       {
            direction = facingDirection;
       }
    }

    // Update is called once per frame
    void FixedUpdate()
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
            //glideState = GlidingStates.PREPARING_GLIDE;
        }

        else if (rb.linearVelocityY == 0)
        {
            //Debug.Log("No linear Y Velocity");
            resetGlide();
        }

        handleGliding(xSpeed);

        handleAttacks();

        if (facingDirection == Direction.LEFT)
        {
            this.transform.rotation = new Quaternion(0, -1, 0, 0);
        }
        else
        {
            this.transform.rotation = new Quaternion(0, 0, 0, 0);
        }
    }
}
