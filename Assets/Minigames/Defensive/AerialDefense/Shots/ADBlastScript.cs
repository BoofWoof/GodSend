using UnityEngine;
using UnityEngine.Audio;

public class ADBlastScript : MonoBehaviour
{
    private RectTransform thisRect;
    private Rigidbody2D thisRB2D;

    public AudioClip HitNoise;
    public AudioClip BlockNoise;
    [SerializeField] private AudioMixerGroup TargetGroup;

    [Header("Particles")]
    public GameObject TrailObject;
    public ParticleSystem Trail;
    public GameObject BurstObject;
    public ParticleSystem Burst;

    [Header("Stats")]
    public int DamageAmount = 1;
    public float Speed = 20f;
    public float LifeTime = 3f;

    private bool Hit = false;

    private TurretScript Source;

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
            if (Hit) return;
            Hit = true;

            ThreatScript threatScript = collision.gameObject.GetComponent<ThreatScript>();

            if(threatScript != null)
            {
                threatScript.TakeDamage(DamageAmount, transform);
                Source.HitRegenerate();

                if (HitNoise) AudioExtensions.PlayClipAtPointWithMixer(HitNoise, transform.position, TargetGroup);
            } else
            {
                if (BlockNoise) AudioExtensions.PlayClipAtPointWithMixer(BlockNoise, transform.position, TargetGroup);
            }


            Trail.Stop();
            TrailObject.transform.SetParent(transform.parent);
            Destroy(TrailObject, 1);
            Burst.Play();
            BurstObject.transform.SetParent(transform.parent);
            Destroy(BurstObject, 1);

            Destroy(gameObject);
        }
    }

    public void SetSource(TurretScript source)
    {
        Source = source;
    }
}
