using UnityEngine;

public class enemyBulletSp : MonoBehaviour
{
    public int speed = 25;
    public int damage = 5;
    public float moveDirection = 1;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
      Destroy(gameObject,5);
    }
      
    // Update is called once per frame
    void Update()
    {

        transform.position += Vector3.right * moveDirection * speed * Time.deltaTime;
    }
    public void bulletSetDerection(float direction)
    {
        moveDirection = direction;
        if (moveDirection < 0)
        {
            transform.eulerAngles = new Vector3(0, 180, 0);
        }
        else
        {
            transform.eulerAngles = new Vector3(0, 0, 0);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Destroy(gameObject);
            PlayerAi player = collision.gameObject.GetComponent<PlayerAi>();
            Transform impect = player.transform.Find("impectEf");
             player.playerTekDamage(damage);
            if (impect != null)
            {
                ParticleSystem impectEffect = impect.GetComponent<ParticleSystem>();
                if (impectEffect != null)
                {
                    impect.position = collision.ClosestPoint(transform.position);
                    impectEffect.Play();
                  
                }
            }
             
        }
    }
}
