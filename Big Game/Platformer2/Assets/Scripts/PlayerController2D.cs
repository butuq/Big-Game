using UnityEngine;

public class PlayerController2D : MonoBehaviour {

    // C# variables
        // Enums
        public enum CurrentMap
        {
            Neutral,
            Air,
            Fire,
            Ice,
            Earth,
            Metal
        }
        public enum DashState
        {
            Ready,
            Dashing,
            Cooldown
        }
        public enum CurrentState
        {
            Idle,
            Running,
            Jumping,
            Falling,
            LookingUp,
            LookingDown,
            Dead,
            Knockback,
            Dashing
        }

        // Public
        public bool canDoubleJump;
        public bool canWallJump;
        public bool canDash;
        public bool isAlive;
        public float normalSpeed; 
        public float jumpSpeed; 
        public float currentHealth; 
        public float totalHealth;
        public float knockbackTimer;

        // Private
        private CurrentMap currentMap;
        private CurrentState currentState;
            // Variables for dash
                private DashState dashState;
                private Vector2 savedVelocity;
                private float dashTimer;
                [SerializeField] private float dashDuration;
                [SerializeField] private float dashIntensity;
            // Variables for double jump
                private int jumpsAvailable;
                private bool isJumping;
                [SerializeField] private float jumpTime;
                private float jumpTimeCounter;

    // Unity variables 
        // Public 
        public GameObject bodyColliderObject;
        public GameObject feetColliderObject;
        public GameObject enemyDetectionColliderObject;


        // Private
        private Rigidbody2D myRigidBody;
        private BoxCollider2D myBodyCollider;
        private BoxCollider2D myFeetCollider;
        private Animator myAnimator;


	void Start ()
    {
        currentMap = CurrentMap.Neutral;

        currentHealth = totalHealth;
        isAlive = true;
        myFeetCollider = feetColliderObject.GetComponent<BoxCollider2D>();
        myBodyCollider = bodyColliderObject.GetComponent<BoxCollider2D>();
        myRigidBody = this.GetComponent<Rigidbody2D>();
        myAnimator = this.GetComponent<Animator>();
	}
	
	void Update ()
    {
        if (!isAlive)
        {
            myRigidBody.velocity = Vector2.zero;
            currentState = CurrentState.Dead;
            return;
        }

        if (knockbackTimer > 0f)
        {
            knockbackTimer -= Time.deltaTime;
            currentState = CurrentState.Knockback;
            return;
        }

        Run();
        Jump();
        Dash();
        FlipSprite();
        StateMachine();
	}

    private void Run()
    {
        if (currentState != CurrentState.Dashing)
        {
            float playerInput = Input.GetAxisRaw("Horizontal");
            Vector2 playerVelocity = new Vector2(playerInput * normalSpeed, myRigidBody.velocity.y);
            myRigidBody.velocity = playerVelocity;
        }
    }

    private void Jump()
    {        
        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        
        if (isGrounded)
            jumpsAvailable = 2;

        if (jumpsAvailable > 0 && Input.GetButtonDown("Jump"))
        {
            jumpsAvailable--;
            isJumping = true;
            jumpTimeCounter = jumpTime;
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);
        }

        if (Input.GetButton("Jump") && isJumping)
        {
            if (jumpTimeCounter > 0)
            {
                myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, jumpSpeed);
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        if (Input.GetButtonUp("Jump"))
        {
            isJumping = false;
        }
    }

    private void Dash()
    {
        if (canDash)
        {
            switch (currentMap)
            {
                case CurrentMap.Neutral:
                    switch (dashState)
                    {
                        case DashState.Ready:
                            bool isDashKeyDown = Input.GetButtonDown("Special1");
                            if (isDashKeyDown)
                            {
                                savedVelocity = new Vector2(myRigidBody.velocity.x, 0f);
                                myRigidBody.velocity = new Vector2(normalSpeed * this.transform.localScale.x * dashIntensity, 0f);
                                myRigidBody.gravityScale = 0f;
                                dashState = DashState.Dashing;
                            }
                            break;
                        case DashState.Dashing:
                            Debug.Log(myRigidBody.velocity);
                            dashTimer += Time.deltaTime;
                            if (dashTimer >= dashDuration)
                            {
                                dashTimer = dashDuration;
                                myRigidBody.velocity = savedVelocity;
                                myRigidBody.gravityScale = 1f;
                                dashState = DashState.Cooldown;
                            }
                            break;
                        case DashState.Cooldown:
                            dashTimer -= Time.deltaTime;
                            if (dashTimer <= 0)
                            {
                                dashTimer = 0;
                                dashState = DashState.Ready;
                            }
                            break;
                    }
                    break;
                case CurrentMap.Air:
                    break;
                case CurrentMap.Fire:
                    break;
                case CurrentMap.Ice:
                    break;
                case CurrentMap.Earth:
                    break;
                case CurrentMap.Metal:
                    break;
                default:
                    break;
            }
        }
    }

    private void FlipSprite()
    {
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > 0f;
        if (hasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(myRigidBody.velocity.x), 1f);
        }
    }

    private void StateMachine()
    {
        if (dashState == DashState.Dashing)
        {
            currentState = CurrentState.Dashing;
            //myAnimator.SetBool("Dash", true);
            return;
        }

        if (myRigidBody.velocity == Vector2.zero)
        {
            currentState = CurrentState.Idle;
            //myAnimator.SetBool("Idle", true);
            return;
        }

        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (isGrounded)
        {
            if (Mathf.Abs(myRigidBody.velocity.x) > 0f)
            {
                currentState = CurrentState.Running;
                //myAnimator.SetBool("Running", true);
            }
            return;
        }

        if (myRigidBody.velocity.y > 0f)
        {
            currentState = CurrentState.Jumping;
            //myAnimator.SetBool("Jumping", true);
            return;
        }
        else
        {
            currentState = CurrentState.Falling;
            //myAnimator.SetBool("Falling", true);
            return;
        }
    }

    private void SetAnimation()
    {
        // Running
        bool hasHorizontalSpeed = Mathf.Abs(myRigidBody.velocity.x) > 0f;
        // myAnimator.SetBool("Running", hasHorizontalSpeed);
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isAlive = false;
    }
}
