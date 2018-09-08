using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    public float attackCooldown;
    public float knockbackDuration;
    public int damage;
    public float verticalKnockbackIntensity;
    public float horizontalKnockbackIntensity;

    public GameObject attackColliderObject;
    private BoxCollider2D myAttackCollider;
    private Animator myAnimator;
    private PlayerController2D myController;

    private float attackCount;
    private List<GameObject> enemiesInRange;

    private void Start()
    {
        enemiesInRange = new List<GameObject>();
        myAttackCollider = attackColliderObject.GetComponent<BoxCollider2D>();
        myAnimator = this.GetComponent<Animator>();
        myController = this.GetComponent<PlayerController2D>();
    }

    void Update ()
    {
        if (Input.GetButtonDown("Attack1"))
            Attack();

        attackCount -= Time.deltaTime;
	}

    private void Attack()
    {
        if (attackCount <= 0f)
        {
            // myAnimator.SetTrigger("Attack");
            attackCount = attackCooldown;
            foreach (GameObject enemy in enemiesInRange)
            {
                EnemyMovement enemyScript = enemy.GetComponent<EnemyMovement>();
                enemyScript.knockbackTimerSelf = knockbackDuration;
                float horizontalKnockback = horizontalKnockbackIntensity * this.transform.localScale.x;
                enemy.GetComponent<Rigidbody2D>().velocity = new Vector2(horizontalKnockback, verticalKnockbackIntensity);
                enemyScript.TakeDamage(damage);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        GameObject enemy = collision.transform.parent.gameObject;
        Physics2D.IgnoreCollision(collision.gameObject.GetComponent<Collider2D>(), myController.bodyColliderObject.GetComponent<Collider2D>());
        if (enemy.tag == "Enemy" && !enemiesInRange.Contains(enemy))
        {
            enemiesInRange.Add(enemy);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        GameObject enemy = collision.transform.parent.gameObject;
        if (enemy.tag == "Enemy")
        {
            enemiesInRange.Remove(enemy);
        }
    }
}
