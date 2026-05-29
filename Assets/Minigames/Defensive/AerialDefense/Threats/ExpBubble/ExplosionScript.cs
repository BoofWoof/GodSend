using UnityEngine;

public class ExplosionScript : MonoBehaviour
{

    public float EffectiveSeconds = 1f;
    private float TimePassed = 0f;

    public void Update()
    {
        TimePassed += Time.deltaTime;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (TimePassed > EffectiveSeconds) return;

        if (collision.gameObject.tag == "ADThreat")
        {
            collision.gameObject.GetComponent<FallingThreatScript>().SpawnExplosionPing();
            Destroy(collision.gameObject);

            AerialDefenseScript.ThreatDestroyed();
        }
    }
}
