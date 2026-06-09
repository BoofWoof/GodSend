using UnityEngine;

public class ADBlastScript : MonoBehaviour
{
    private RectTransform thisRect;
    private Rigidbody2D thisRB2D;

    [Header("Particles")]
    public GameObject TrailObject;
    public ParticleSystem Trail;
    public GameObject BurstObject;
    public ParticleSystem Burst;

    [Header("Stats")]
    public int DamageAmount = 1;
    public float Speed = 20f;
    public float LifeTime = 3f;

    public void Start()
    {
        thisRect = GetComponent<RectTransform>();

        Destroy(gameObject, LifeTime);

        thisRB2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        thisRB2D.linearVelocity = thisRect.up * Speed * transform.lossyScale.x;
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "ADThreat")
        {
            collision.gameObject.GetComponent<FallingThreatScript>().TakeDamage(DamageAmount, transform);

            Trail.Stop();
            TrailObject.transform.SetParent(transform.parent);
            Destroy(TrailObject, 1);
            Burst.Play();
            BurstObject.transform.SetParent(transform.parent);
            Destroy(BurstObject, 1);

            Destroy(gameObject);
        }
    }
}
