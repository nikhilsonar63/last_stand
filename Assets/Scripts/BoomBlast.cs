using UnityEngine;

public class BoomBlast : MonoBehaviour
{
    public PlayerAi PlayerAi;
    public AudioSource audioSource;
    public AudioClip boom;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Transform blast = transform.Find("boom");
            if (blast != null)
            {
                ParticleSystem Boom = blast.GetComponent<ParticleSystem>();
                if (Boom != null)
                {
                    int Damage;
                    Damage = Random.Range(5, 15);
                    PlayerAi.playerTekDamage(Damage);
                    Boom.Play();
                    if (audioSource != null && boom != null)
                    {
                        audioSource.PlayOneShot(boom, 0.8f);
                    }
                    Destroy(gameObject, 0.2f);
                }

            }
        }
    }
}
