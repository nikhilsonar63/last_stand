using System.Collections;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    [Header("Public assing varibles")]
    public Transform[] randomPoint;
    public Transform randomTarget;
    public LayerMask mask;          // Player detect karne wali ray ke liye (purana)
    public Rigidbody2D rb;
    public Animator anim;
    public GameObject Bullet;
    public GameObject coins;
    public SpriteRenderer spriteRenderer;
    public PlayerAi playerAi;

    [Header("Jump settings (NAYA)")]
    public LayerMask groundMask;           // Inspector mein "Ground" layer select karo
    public float obstacleCheckDist = 1f;   // enemy kitna aage dekhe
    public float obstacleCheckHeight = 0.3f; // ray kitni oopar se jaye (zameen se thoda upar)
    public float jumpCooldown = 0.4f;      // jump ke baad itni der dobara jump nahi
    public float normalGravity = 2f;       // upar jaate time
    public float fallGravity = 3.5f;       // girte time (jump tight lagta hai)

    [Header("Numbers velus")]
    public float minWaitTime = 1;
    public float maxWiatTime = 5;
    public float attackReng = 2;
    public float Reng = 10;
    public float health = 25;
    public float visionReng = 5;
    public float walkSpeed = 5;
    public float runSpeed = 7;
    public float currentSpeed;
    public int knifeDamage = 3;
    public int jumpHight = 8;

    [Header("Bool varibles")]
    public bool wiating = false;
    public bool movingToB = false;
    public bool isDeath = false;
    public bool attackDecided = false;
    public bool isGraoundd = true;

    Transform player;
    Collider2D col;
    float lastJumpTime = -10f;

    public enum stat
    {
        patrol,
        knifeAttack,
        ditection,
        gunFireAttack,
        crouchGunAttack,
    }
    stat currentStat;

    private void Start()
    {
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;

        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        rb.freezeRotation = true;
        rb.gravityScale = normalGravity;

        currentStat = stat.patrol;
        health = Mathf.Clamp(health, 0, 25);
    }

    // ---------------- UPDATE: decisions + animation ----------------
    private void Update()
    {
        if (isDeath) return;
        if (player == null || !player.gameObject.CompareTag("Player")) return;

        float distance = Vector2.Distance(transform.position, player.position);

        switch (currentStat)
        {
            case stat.ditection:
                Ditect();
                break;
            case stat.knifeAttack:
                knifeAttack();
                break;
            case stat.gunFireAttack:
                gunFireAttack();
                break;
            case stat.crouchGunAttack:
                crouchGunAttack();
                break;
        }

        // State change: player vision mein aaya ya gaya
        if (distance <= visionReng && currentStat == stat.patrol)
        {
            StopAllCoroutines();
            wiating = false;
            currentStat = stat.ditection;
        }
        else if (distance > visionReng && currentStat != stat.patrol)
        {
            currentStat = stat.patrol;
            attackDecided = false;
            wiating = false;
        }

        // Animator
        anim.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));
        anim.SetBool("jump", !isGraoundd);
    }

    // ---------------- FIXEDUPDATE: saari physics/movement yahan ----------------
    private void FixedUpdate()
    {
        if (isDeath || player == null) return;

        UpdateGrounded();

        // Girte time gravity badhao, jump floaty nahi lagega
        rb.gravityScale = rb.linearVelocity.y < 0 ? fallGravity : normalGravity;

        switch (currentStat)
        {
            case stat.patrol:
                PatrolMove();
                CheckAndJump();
                break;
            case stat.knifeAttack:
                KnifeMove();
                CheckAndJump();
                break;
            default:
                // gun / crouch / ditection mein enemy ruka rahe
                StopX();
                break;
        }
    }

    // ---------------- GROUND CHECK ----------------
    void UpdateGrounded()
    {
        if (col == null) return;
        // Collider ke bilkul neeche se chhoti ray niche ki taraf
        Vector2 origin = col.bounds.center;
        float dist = col.bounds.extents.y + 0.1f;
        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, dist, groundMask);
        isGraoundd = hit.collider != null;
    }

    // ---------------- AUTO JUMP ----------------
    void CheckAndJump()
    {
        if (!isGraoundd) return;
        if (Time.time < lastJumpTime + jumpCooldown) return;

        float dir = spriteRenderer.flipX ? -1f : 1f;
        Vector2 origin = new Vector2(col.bounds.center.x, col.bounds.min.y + obstacleCheckHeight);

        RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.right * dir, obstacleCheckDist, groundMask);
        Debug.DrawRay(origin, Vector2.right * dir * obstacleCheckDist, Color.green);

        if (hit.collider != null)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpHight);
            isGraoundd = false;
            lastJumpTime = Time.time;
        }
    }

    // ---------------- MOVEMENT ----------------
    void StopX()
    {
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }

    void Face(float dirX)
    {
        if (dirX < 0) spriteRenderer.flipX = true;
        else if (dirX > 0) spriteRenderer.flipX = false;
    }

    void PatrolMove()
    {
        currentSpeed = walkSpeed;

        if (wiating)
        {
            StopX();
            return;
        }

        if (randomTarget == null)
        {
            if (randomPoint.Length == 0)
            {
                StopX();
                return;
            }
            randomTarget = randomPoint[Random.Range(0, randomPoint.Length)];
        }

        // Sirf X ka difference dekho (height ka farq reach hone se nahi rokega)
        float diff = randomTarget.position.x - rb.position.x;

        if (Mathf.Abs(diff) < 0.5f)
        {
            randomTarget = null;
            StopX();
            StartCoroutine(wiatAtPoint());
            return;
        }

        float dir = Mathf.Sign(diff);
        Face(dir);
        rb.linearVelocity = new Vector2(dir * currentSpeed, rb.linearVelocity.y);
    }

    void KnifeMove()
    {
        currentSpeed = runSpeed;

        float dx = player.position.x - transform.position.x;
        float distance = Vector2.Distance(transform.position, player.position);
        Face(dx);

        if (distance <= attackReng)
        {
            StopX();
        }
        else
        {
            rb.linearVelocity = new Vector2(Mathf.Sign(dx) * currentSpeed, rb.linearVelocity.y);
        }
    }

    // ---------------- STATES ----------------
    public void Ditect()
    {
        Vector2 dirToPlayer = (player.position - transform.position).normalized;
        RaycastHit2D hit2D = Physics2D.Raycast(transform.position, dirToPlayer, Reng, mask);

        if (hit2D.collider != null && hit2D.transform.CompareTag("Player"))
        {
            if (!attackDecided)
            {
                attackDecided = true;
                Face(player.position.x - transform.position.x);

                int rendomAnim = Random.Range(1, 4); // 1, 2 ya 3
                if (rendomAnim == 1)
                {
                    wiating = true;
                    currentStat = stat.gunFireAttack;
                }
                else if (rendomAnim == 2)
                {
                    wiating = false;
                    currentStat = stat.knifeAttack;
                }
                else
                {
                    wiating = true;
                    currentStat = stat.crouchGunAttack;
                }
                Debug.Log(rendomAnim);
            }
        }

        Debug.DrawRay(transform.position, dirToPlayer * Reng, Color.red);
    }

    public void gunFireAttack()
    {
        anim.SetTrigger("gunFireAttack");
    }

    public void knifeAttack()
    {
        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= attackReng)
        {
            anim.SetTrigger("knifeAttack");
        }
    }

    public void crouchGunAttack()
    {
        anim.SetTrigger("crouchGunAttack");
    }

    // Animation event se call hota hai
    public void Fire()
    {
        GameObject bulletPrefeb = Instantiate(Bullet, transform.position, Quaternion.identity);
        enemyBulletSp bulletScript = bulletPrefeb.GetComponent<enemyBulletSp>();
        if (bulletScript != null && spriteRenderer != null)
        {
            float pDirection = spriteRenderer.flipX ? -1f : 1f;
            bulletScript.bulletSetDerection(pDirection);
        }
    }

    IEnumerator wiatAtPoint()
    {
        wiating = true;
        float wiatTime = Random.Range(minWaitTime, maxWiatTime);
        yield return new WaitForSeconds(wiatTime);
        wiating = false;
    }

    // ---------------- DAMAGE ----------------
    public void enemyTekDamage(float damage)
    {
        health -= damage;
        if (health <= 0 && !isDeath)
        {
            isDeath = true;
            rb.linearVelocity = Vector2.zero;
            playerAi.killEnemy += 1;
            Destroy(gameObject, 0.5f);

            int randomCoins = Random.Range(1, 5);
            for (int i = 0; i < randomCoins; i++)
            {
                Instantiate(coins, transform.position, Quaternion.identity);
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerAi.playerTekDamage(knifeDamage);
        }
    }
}