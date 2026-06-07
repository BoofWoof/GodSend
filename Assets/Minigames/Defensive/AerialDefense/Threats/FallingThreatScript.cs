using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FallingThreatScript : MonoBehaviour
{
    public static List<FallingThreatScript> FallingThreatScripts = new List<FallingThreatScript>();
    private Rigidbody2D thisRB2D;
    private Image thisImage;

    public AudioClip[] DestructionNoises;

    private bool FallTriggered = false;

    [Header("Health and Damage")]
    public int Health = 1;
    public bool CanBeHurt = true;
    public bool CanHurtCity = true;

    [Header("DeathSettings")]
    public float FinalRippleValue = 1f;
    public float DeathPeriod = 0.5f;

    [Header("Personal Speeds")]
    public float DropSpeed = 20f;
    public float WindSpeed = 0f;

    [Header("External Speeds")]
    public float FormationSpeedModifier = 1f;
    public float WaveSpeedModifier = 1f;

    [Header("Prefabs")]
    public GameObject DestructionPingPrefab;
    public GameObject DetectionPingPrefab;

    [Header("Scanner Data")]
    public bool RadarNeeded = false;
    public float FadeInPeriod = 1.0f;
    public float FadeOutPeriod = 1.0f;
    public float MaintainPeriod = 0.5f;

    public static bool isEnemiesRemaining()
    {
        return FallingThreatScripts.Count > 0;
    }

    public void Awake()
    {
        FallingThreatScripts.Add(this);

        transform.SetParent(ThreatSpawnerScript.Instance.transform);
        transform.rotation = Quaternion.identity;

        thisRB2D = GetComponent<Rigidbody2D>();
        thisImage = GetComponent<Image>();
        thisImage.maskable = true;

        Vector2 DownSpeed = Vector2.down * transform.parent.lossyScale * 75f * WaveSpeedModifier;
        Vector2 TotalSpeed = DownSpeed;
        thisRB2D.linearVelocity = TotalSpeed;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FallTrigger()
    {
        if (FallTriggered) return;
        FallTriggered = true;

        thisRB2D = GetComponent<Rigidbody2D>();
        Vector2 DownSpeed = Vector2.down * DropSpeed * transform.parent.lossyScale * FormationSpeedModifier * WaveSpeedModifier;
        Vector2 HorizontalSpeed = Vector2.right * WindSpeed * transform.lossyScale;
        Vector2 TotalSpeed = DownSpeed + HorizontalSpeed;
        thisRB2D.linearVelocity = TotalSpeed;

        float angle = Mathf.Atan2(TotalSpeed.x, TotalSpeed.y) * Mathf.Rad2Deg;
        thisRB2D.MoveRotation(-angle - 180f);
        //transform.localRotation = Quaternion.Euler(0, 0, -targetAngle);
    }

    public static void DestroyAllThreats()
    {
        foreach(FallingThreatScript fallingThreatScript in FallingThreatScripts)
        {
            fallingThreatScript.SpawnExplosionPing();
            Destroy(fallingThreatScript.gameObject);
        }
    }

    private void OnDestroy()
    {
        FallingThreatScripts.Remove(this);
        AerialDefenseScript.ThreatDestroyed();
    }

    public void SpawnExplosionPing()
    {
        GameObject newPing = Instantiate(DestructionPingPrefab, transform.position, Quaternion.identity, AerialDefenseScript.StaticGameCanvas);
        AudioSource pingAudio = newPing.GetComponent<AudioSource>();
        if (pingAudio != null && DestructionNoises.Length > 0)
        {
            int randIdx = Random.Range(0, DestructionNoises.Length);
            pingAudio.clip = DestructionNoises[randIdx];
            pingAudio.Play();
        }
    }

    public void TakeDamage(int damageValue, Transform source)
    {
        Health -= damageValue;
        if(Health <= 0)
        {
            //SpawnExplosionPing();
            if (source == null) Destroy(gameObject);
            else
            {
                StartCoroutine(BurnAnimation(source.localPosition));
            }
        }
    }

    public IEnumerator BurnAnimation(Vector2 explosionPoint)
    {
        Debug.Log("STARTING THE BURN---------------------");

        thisRB2D.simulated = false;

        Material renderMaterial = Instantiate(thisImage.material);
        thisImage.canvasRenderer.SetMaterial(renderMaterial, 0);
        renderMaterial.SetVector("_ImpactPoint", explosionPoint);

        float timePassed = 0f;
        while (timePassed < DeathPeriod)
        {
            yield return null;
            timePassed += Time.deltaTime;
            Debug.Log(timePassed);
            float progress = timePassed / DeathPeriod;
            Debug.Log(progress);
            float rippleValue = Mathf.Lerp(0.1f * FinalRippleValue, FinalRippleValue, progress);
            Debug.Log(rippleValue);
            renderMaterial.SetFloat("_RippleProgress", rippleValue);
        }
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "SpawnSource")
        {
            if (FallTriggered) return;
            FallTrigger();
            return;
        }
        if (collision.gameObject.name == "DangerLine")
        {
            if (CanHurtCity)
            {
                AerialDefenseScript.TakeDamage();
            }
            TakeDamage(9999999, null);
            return;
        }
    }
}
