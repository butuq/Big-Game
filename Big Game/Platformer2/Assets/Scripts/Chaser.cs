using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chaser : EnemyMovement
{
    [Header("Chaser Info")]
    public GameObject ChaserColliderObject;
    private BoxCollider2D myChaserCollider;
    public GameObject player;
    //public float chasingDistance;
    public float extraSpeed;
    protected override void XStart()
    {
        myChaserCollider = ChaserColliderObject.GetComponent<BoxCollider2D>();
    }
    /*
        protected override void Special()
        {
            //Follow is player is near
            float distanceToPlayer = Vector2.Distance(player.transform.position, transform.position);

            if (distanceToPlayer < chasingDistance && Mathf.Abs(player.transform.position.y - transform.position.y) < 1)
            {
                float buff;
               //Detect player
                 if ((transform.position.x < player.transform.position.x && currentDirection==1) || (transform.position.x > player.transform.position.x && currentDirection==-1))
                {
                    buff = normalSpeed + extraSpeed;

                }
                else
                {
                    buff = normalSpeed;

                }

                float horizontalSpeed = (buff) * currentDirection;
                myRigidBody.velocity = new Vector2(horizontalSpeed, myRigidBody.velocity.y);



            } */

    protected override void XCollisionEnter(Collider2D collision)
    {

        
        if (myChaserCollider.IsTouching(player.GetComponent<PlayerController2D>().enemyDetectionColliderObject.GetComponent<BoxCollider2D>())/* && collision.gameObject.tag == "EnemyDetectsPlayer"*/)
        {
            playerDetected = true;

            GameObject player = collision.transform.parent.gameObject;
            PlayerController2D playerScript = player.GetComponent<PlayerController2D>();
            if (playerScript.isAlive)
            {
                float horizontalSpeed = (normalSpeed + extraSpeed) * currentDirection;
                myRigidBody.velocity = new Vector2(horizontalSpeed, myRigidBody.velocity.y);

            }
        }
    }

   /* private void OnTriggerExit2D(Collider2D collision)
    {
        if (!myChaserCollider.IsTouchingLayers(LayerMask.GetMask("Player")) && collision.gameObject.tag == "EnemyDetectsPlayer")
        {
            playerDetected = false;
        }
    }*/
}