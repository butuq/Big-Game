using UnityEngine;

public class EnemyMovement : MonoBehaviour {

    public float normalSpeed;
    public int currentHealth;
    public int totalHealth;
    public bool canDropFromPlatforms;
    public float knockbackTimerSelf;
    public bool playerDetected;

    [Header("Damage to Player")]
    public int damage;
    public float horizontalKnockBackIntensity;
    public float verticalKnockBackIntensity;
    public float knockbackDuration;

    public GameObject bodyColliderObject;
    public GameObject wallColliderObject;
    public GameObject attackColliderObject;
    public GameObject falldownColliderObject;
    public Rigidbody2D myRigidBody;
    private BoxCollider2D myBodyCollider;
    private BoxCollider2D myWallCollider;
    private BoxCollider2D myAttackCollider;
    private BoxCollider2D falldownCollider;

    public int currentDirection;

    private void Start()
    {
        currentHealth = totalHealth;
        myBodyCollider = bodyColliderObject.GetComponent<BoxCollider2D>();
        myWallCollider = wallColliderObject.GetComponent<BoxCollider2D>();
        myAttackCollider = attackColliderObject.GetComponent<BoxCollider2D>();
        falldownCollider = falldownColliderObject.GetComponent<BoxCollider2D>();
        myRigidBody = this.GetComponent<Rigidbody2D>();
        currentDirection = 1;
        //Any Extra thing that needs to be started
        XStart();
    }

    private void Update()
    {


        if (knockbackTimerSelf > 0)
        {
            knockbackTimerSelf -= Time.deltaTime;
            return;
        }

        if (myWallCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            playerDetected = false;
            Flip();
        }
            

        bool isGrounded = myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        bool isGonnaFall = !falldownCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
        if (!canDropFromPlatforms && isGrounded && isGonnaFall)
        {
            playerDetected = false;
            Flip();
        }
        

        
        if (!playerDetected)
        {
            float horizontalSpeed = (normalSpeed) * currentDirection;
            myRigidBody.velocity = new Vector2(horizontalSpeed, myRigidBody.velocity.y);
        }
            

    }

    protected virtual void XCollisionEnter(Collider2D collision)
    {

    }
    protected virtual void XStart()
    {

    }
    private void Flip()
    {
        currentDirection = -currentDirection;
        Vector3 scale = this.transform.localScale;
        scale.x *= -1;
        this.transform.localScale = scale;
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        if (currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (myAttackCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && collision.gameObject.tag == "EnemyDetectsPlayer")
        {
            
            GameObject player = collision.transform.parent.gameObject;
            PlayerController2D playerScript = player.GetComponent<PlayerController2D>();
            if (playerScript.knockbackTimer <= 0f && playerScript.isAlive)
            {
                playerScript.knockbackTimer = knockbackDuration;
                Vector2 dir = (player.transform.position - this.transform.position);
                float horizontalKnockback = horizontalKnockBackIntensity * dir.normalized.x;              
                player.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalKnockback, verticalKnockBackIntensity);
                playerScript.TakeDamage(damage);
            }
        }
        XCollisionEnter(collision);
    }

}
