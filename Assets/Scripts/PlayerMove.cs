using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager gameManager;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    CapsuleCollider2D capsuleCollider;
    Animator animator;
    public float maxSpeed;
    public float jumpPower;
    public int HP;
    // Start is called before the first frame update
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        capsuleCollider = GetComponent<CapsuleCollider2D>();
        animator = GetComponent<Animator>();
        gameManager = GameObject.FindWithTag("GM").GetComponent<GameManager>();
    }

    private void Update()
    {
        //Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("isJumping")) {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isJumping", true);
        }

        // Stop To Move
        if (Input.GetButtonUp("Horizontal")) {
            rigid.velocity = new Vector2(0.5f * rigid.velocity.normalized.x , rigid.velocity.y);
        }
        // Sprite Direction
        if (Input.GetButton("Horizontal")){
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        //Walk Animation
        if (Mathf.Abs(rigid.velocity.x) < 0.5f)
        {
            animator.SetBool("isWalking", false);
        }
        else {
            animator.SetBool("isWalking", true);
        }
    }

    // Update is called once per frames
    void FixedUpdate()
    {
        //Move By Key Control
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.velocity.x > maxSpeed)
        {
            rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
        }
        else if (rigid.velocity.x < maxSpeed * (-1)) {
            rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // Check Landing Platform
        if (rigid.velocity.y < 0) {
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1.0f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null && rayHit.distance < 0.7f)
            {
                animator.SetBool("isJumping", false);
            }
        }

    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) {
                //Attack
                OnAttack(collision.transform);
            }
            else
            {
                //Damaged
                OnDamaged(collision.transform.position);
            }
            
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag) {
            case "Item":
                gameManager.GetScore(collision.gameObject);
                collision.gameObject.SetActive(false);
                break;
            case "Finish":
                //Next Stage
                gameManager.NextStage();
                break;
        }
    }

    void OnDamaged(Vector2 targetPos) {
        //HP Down
        HPDown();
        // Change Layer (Immortal Active)
        gameObject.layer = 11;

        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int direct = (transform.position.x - targetPos.x) > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(direct, 1)* 7, ForceMode2D.Impulse);

        animator.SetTrigger("doDamaged");

        Invoke("OffDamaged", 3);
    }

    void OnAttack(Transform enemy) {
        EnemyMove enemyMove = enemy.GetComponent<EnemyMove>();

        //React Player
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);

        //Kill Enemy
        enemyMove.OnDamaged();

    }

    void OffDamaged() {
        gameObject.layer = 10;

        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    public void ReSpawn() {
        if (HP <= 0) {
            return;
        }
        transform.position = new Vector3(-12, 4, -1);
        rigid.velocity = Vector2.zero;

    }

    public void HPDown() {
        HP--;
        gameManager.HPDown();
        if (HP <= 0) {
            OnDie();
            Debug.Log("Player Die");
        }
    }
    void OnDie() {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        //Sprite Flip Y
        spriteRenderer.flipY = true;
        //Collider Disable
        capsuleCollider.enabled = false;
        //Die Effect Jump
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
        gameManager.OnDie();
    }
}