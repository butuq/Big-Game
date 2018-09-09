using UnityEngine;

public class PlayerController2D : MonoBehaviour {

    // C# variables
        // Public
        public bool isAlive;
        public float normalSpeed; // eventually should be private
        public float jumpSpeed; // eventually should be private
        public float currentHealth; // eventually should be private
        public float totalHealth;
        public float knockbackTimer;
        public enum CurrentState // eventually should be private
        {
            Idle,
            Running,
            Jumping,
            Falling,
            LookingUp,
            LookingDown,
            Dead,
            Knockback
        }
        public CurrentState currentState; // eventually should be private

        // Private

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
        FlipSprite();
        StateMachine();
	}

    private void Run()
    {
        float playerInput = Input.GetAxisRaw("Horizontal");
        Debug.Log(playerInput);
        Vector2 playerVelocity = new Vector2(playerInput * normalSpeed, myRigidBody.velocity.y);
        myRigidBody.velocity = playerVelocity;
    }

    private void Jump()
    {
        if (Input.GetButtonUp("Jump") && currentState == CurrentState.Jumping)
        {
            myRigidBody.velocity = new Vector2(myRigidBody.velocity.x, 0f);
        }

        // Change how this code works if you want double jump or more.
        bool isGrounded = myFeetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!isGrounded)
        {
            return;
        }
        
        if (Input.GetButtonDown("Jump"))
        {
            Vector2 jumpVelocity = new Vector2(0f, jumpSpeed);
            myRigidBody.velocity += jumpVelocity;
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
