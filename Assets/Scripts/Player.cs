using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField]
    private GameManager gameManager;
    [SerializeField]
    private int speed;
    [SerializeField]
    private int jumpPower;

    [SerializeField]
    private GameObject bulletObj;

    [SerializeField]
    private int maxJumpCount = 2;
    [SerializeField]
    private int jumpCount = 0;

    [SerializeField]
    private float maxShotDelay;
    [SerializeField]
    private float curShotDelay;

    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator anim;
    BoxCollider2D boxCollider;

    public bool isDamaged = false;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    void Update()
    {
        if (Input.GetButtonDown("Jump") && jumpCount < maxJumpCount)
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            jumpCount++;
            anim.SetBool("isJumping", true);
        }

        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.linearVelocity = new Vector2(rigid.linearVelocity.normalized.x * 0.5f, rigid.linearVelocity.y);
        }

        if (Input.GetButtonDown("Horizontal"))
        {
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
        }

        if (Mathf.Abs(rigid.linearVelocity.x) < 2.5)
        {
            anim.SetBool("isWalking", false);
        }
        else
            anim.SetBool("isWalking", true);

        rigid.angularDamping = 5f;

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }

        curShotDelay += Time.deltaTime;
    }

    void FixedUpdate()
    {
        float h = Input.GetAxisRaw("Horizontal");
        rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        if (rigid.linearVelocity.x > speed)
            rigid.linearVelocity = new Vector2(speed, rigid.linearVelocity.y);
        else if (rigid.linearVelocity.x < speed * (-1))
            rigid.linearVelocity = new Vector2(speed * (-1), rigid.linearVelocity.y);

        Debug.DrawRay(rigid.position, Vector3.down * 1f, Color.green);
        RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1f, LayerMask.GetMask("Platform"));

        if (rayHit.collider != null)
        {
            if (rayHit.distance < 0.7f)
            {
                anim.SetBool("isJumping", false);
                jumpCount = 0;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            OnDamaged(collision.transform.position);
        }
    }

    void OnDamaged(Vector2 targetPos)
    {
        if (isDamaged) return; // 이미 무적이면 실행 안됨
        isDamaged = true;

        gameManager.HealthDown();

        gameObject.layer = 11;
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        int dirc = transform.position.x - targetPos.x > 0 ? 1 : -1;
        rigid.AddForce(new Vector2(dirc, 1) * 5, ForceMode2D.Impulse);

        anim.SetTrigger("doDamaged");
        Invoke("OffDamaged", 3);
    }

    void OffDamaged()
    {
        isDamaged = false;
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Finish")
        {
            gameManager.NextStage();
        }

        if (collision.gameObject.tag == "Fall")
            OnDamaged(transform.position);
    }


    void Attack()
    {
        if (!Input.GetButton("Fire1"))
            return;

        if (curShotDelay < maxShotDelay)
            return;
        GameObject bullet = Instantiate(bulletObj, transform.position, transform.rotation);
        Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
        Vector2 dir = spriteRenderer.flipX ? Vector2.left : Vector2.right;
        rigid.AddForce(dir * 10, ForceMode2D.Impulse);

        curShotDelay = 0;
    }

    public void OnDie()
    {
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);
        spriteRenderer.flipY = true;
        boxCollider.enabled = false;
        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);
    }

    public void VelocityZero()
    {
        rigid.linearVelocity = Vector2.zero;
    }
}
