using System.Collections;
using TMPro;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class PlayerAi : MonoBehaviour
{
    [Header("PUBLIC NUMBRING VELUS")]
    public int coin;
    public int damage = 5;
    public int killEnemy;
    public float Horizontal;
    public float jumpForce=5;
    public float currentHealth;
    public float playerSpeed=5;
    public float playerHealth = 50;

    [Header("PUBLIC ASSING OBJECTS")]
    public Animator cameraAnim;

    public GameObject winSCreen;
    public GameObject uiScreenOff;
    public GameObject gameOverScreen;
    public GameObject heading;
    public GameObject Flag;
    public GameObject bulletPrefeb;

    public Slider healthBar;
    public Slider winHealthSlider;

    public TMP_Text coinTx;
    public TMP_Text winCoinTx;

    public SpriteRenderer sr;
    public AudioSource AudioSource;
    public CameraFollw cameraFollw;
    public PlayableDirector finalCutScene;
    public AudioClip npcVoice, boomBlast,Die,Hit,shot,coinSound;

    [Header("PUBLIC BOOL ")]
    public bool ABC = true;
    public bool isGroundd=true;
    public bool complit = false; 
    public bool playerDeath = false;
    bool isHealing = false;

    [Header("PRIVETS")]
    Animator anim;
   Rigidbody2D RB;
   Vector2 move; 
   enum State 
    {
        Move,Attack,Jump 
    }
    State CurrentState=State.Move; 

    // Start is called before the first frame update
    void Start()
    {
        RB=GetComponent<Rigidbody2D>();
        currentHealth = playerHealth;
        anim=GetComponent<Animator>();  
        sr = GetComponent<SpriteRenderer>();
        PlayerPrefs.SetInt("coin", coin);
        InvokeRepeating(nameof(RandomVoice), 1, 8);
       

    }
    void Update()
    {
        heading.SetActive(false);
        if (playerDeath) return;
        healthBar.maxValue = playerHealth;
        winHealthSlider.maxValue = playerHealth;
        healthBar.value = currentHealth;
        coinTx.text = coin.ToString();
        switch (CurrentState)  
        {
            case State.Move:
            PlayerMove();
            break;
            case State.Attack:
            Attack();
            break;
            case State.Jump:
             Jump();
             break;
            
        }

        if (ABC)
        {
            CurrentState = State.Move;
            ABC = true;
        }
        if (killEnemy == 7&&!complit)
        {
            complit = true;
            heading.SetActive(true);
            cameraFollw.enabled = false;
            Flag.SetActive(true);
            Invoke("timeOutHeading", 5);
            uiScreenOff.SetActive(false);
            Invoke("playerScriptOff", 0.5f);
            finalCutScene.stopped += cutSceneEnd;
            sr.flipX = false;
            finalCutScene.Play();
            winHealthSlider.value = currentHealth;
            winCoinTx.text = coin.ToString();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
    }
    void timeOutHeading()
    {
        heading.SetActive(false); 
    }
   
   public void RandomVoice()
    {
       int randomVoiceGanaret = Random.Range(0, 5);
        Debug.Log(randomVoiceGanaret);
        if (randomVoiceGanaret == 2)
        {
            if (AudioSource && npcVoice != null)
            {
                AudioSource.PlayOneShot(npcVoice);
            }
            
        }
       else if (randomVoiceGanaret == 4)
        {
            if (AudioSource && boomBlast != null)
            {
                AudioSource.PlayOneShot(boomBlast,0.5f);
                cameraAnim.SetTrigger("shek");
               

            }
        }
        Debug.Log(randomVoiceGanaret);
    }
    void PlayerMove()   
    {
        
        move=new Vector2(Horizontal,0).normalized; 
        RB.linearVelocity = new Vector2(move.x * playerSpeed, RB.linearVelocity.y);
        float speed=Mathf.Abs(RB.linearVelocity.x);
        anim.SetFloat("Speed",speed);
        if (Horizontal<0)
        {

            sr.flipX = true;
        }
        else if(Horizontal>0)
        {
            sr.flipX = false;
        }
      
        
    }
    public void onGunShot()
    {
        CurrentState = State.Attack;
    }
    public void noShot()
    {
        CurrentState = State.Move;
    }
    public void rodAttck()
    {
        anim.SetTrigger("Attack2");
    }
    public void leftMove()
    {
        Horizontal = -1;
    }
    public void rightMove()
    {
        Horizontal = 1;
    }
    public void stopMove()
    {
        Horizontal = 0;
    }
    public void onJump()
    {
        if (isGroundd)
        {
            Jump();
        }
    }
    void Attack()
    {

     anim.SetTrigger("Attack");
        if (AudioSource != null && shot != null)
        {
            AudioSource.PlayOneShot(shot);
        }
      
    }
    public void Fire()
    {
        GameObject newBullet = Instantiate(bulletPrefeb, transform.position, Quaternion.identity);
        bulletShot bulletShot = newBullet.GetComponent<bulletShot>();
        SpriteRenderer playerSprite = GetComponent<SpriteRenderer>();
        if (bulletShot != null && playerSprite != null)
        {
            float pDirection = playerSprite.flipX ? -1f : 1f;
            bulletShot.bulletSetDerection(pDirection);
        }
    }
    void Jump()
    {
        if (playerDeath) return;
        anim.SetTrigger("jump");
        RB.linearVelocity = new Vector2(RB.linearVelocity.x, jumpForce);
        isGroundd = false;
        ABC = false;

    }
    public void playerTekDamage(int Damage)
    {
        if (playerDeath) return;
        healthBar.value = currentHealth;
        currentHealth-=Damage;
        if (AudioSource != null && Hit != null)
        {
            AudioSource.PlayOneShot(Hit);
        }
        currentHealth = Mathf.Clamp(currentHealth, 0, playerHealth);
          if (currentHealth<=0)
          {
            playerDeath = true;
            gameOverScreen.SetActive(true);
            playerSpeed = 0;
            anim.SetTrigger("Die");
            cameraAnim.SetTrigger("cameraDie");
            cameraFollw.enabled = false;
            gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
            gameObject.tag = "Dead";
            if (AudioSource != null && Die != null)
            {
                AudioSource.PlayOneShot(Die);
            }
            Invoke("off", 0.3f);
          }
    }
    void off()
    {
        cameraAnim.enabled = false;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGroundd=true;
            
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("coin"))
        {
            coin++;
            if (AudioSource != null && coinSound != null)
            {
                AudioSource.PlayOneShot(coinSound);
            }
            Destroy(collision.gameObject);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
           EnemyAi ai= collision.GetComponent<EnemyAi>();
            ai.enemyTekDamage(damage);

        }
        if (coin >= 5 && currentHealth < playerHealth && !isHealing)
        {
            StartCoroutine(HealPlayer());
        }
    }
    IEnumerator HealPlayer()
    {
        while (coin >= 5 && currentHealth < playerHealth)
        {
            coin -= 5;
            float targetHealth = Mathf.Min(currentHealth + 10, playerHealth);
            while (currentHealth < targetHealth)
            {
                currentHealth += 1;
                currentHealth = Mathf.Min(currentHealth, targetHealth);
                healthBar.value = currentHealth;
                yield return new WaitForSeconds(0.05f);
            }
        }
        isHealing = false;
    }
    void playerScriptOff()
    {
            GetComponent<PlayerAi>().enabled = false;

    }
    void cutSceneEnd(PlayableDirector director)
    {
        winSCreen.SetActive(true);
    }
    public void menuButton()
    {
        SceneManager.LoadScene(0);
    }
   public void Exit()
    {
        Application.Quit();
    }
    public void renumeOn()
    {
        Time.timeScale = 0;
    }
    public void resumeOff()
    {
        Time.timeScale = 1;
    }
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
