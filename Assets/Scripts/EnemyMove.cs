using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    Rigidbody2D rigid;
    Animator animator;
    SpriteRenderer spriteRenderer;
    public int nextMove;

    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        Think();
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Move
        rigid.velocity = new Vector2(nextMove, rigid.velocity.y);

        //Platform Check
        Vector2 postPos = new Vector2(rigid.position.x + nextMove * 0.5f, rigid.position.y);
        Debug.DrawRay(postPos, Vector3.down, new Color(0, 1, 0));
        RaycastHit2D rayHit = Physics2D.Raycast(postPos, Vector3.down, 1);
        if (rayHit.collider == null) {
            nextMove *= -1;
            CheckFlip();
            CancelInvoke();
            Invoke("Think", 5);
        }
    }

    void Think()
    {
        nextMove = Random.Range(-1, 2);
        //Sprite Animation
        animator.SetInteger("WalkSpeed", nextMove);
        //Flip Sprite
        CheckFlip();


        Invoke("Think", 5);
    }

    void CheckFlip() {
        if (nextMove != 0)
        {
            spriteRenderer.flipX = nextMove == 1;
        }
    }
}
