using Unity.VisualScripting;
using UnityEngine;

public class bulletShot : MonoBehaviour
{
    public int speed = 25;
    public float damage = 10;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Destroy(gameObject);

            EnemyAi ai = collision.gameObject.GetComponent<EnemyAi>();
            Transform empect = ai.transform.Find("impect");
            Animator anim = collision.gameObject.GetComponent<Animator>();
            if (ai != null)
            {
                ParticleSystem impect = empect.GetComponent<ParticleSystem>();
                ai.enemyTekDamage(damage);
                anim.SetTrigger("hurt");
                impect.Play();
            }
        }
    }

}
