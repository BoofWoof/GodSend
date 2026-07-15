using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class FallingThreatScript : MonoBehaviour
{
    public static List<FallingThreatScript> FallingThreatScripts = new List<FallingThreatScript>();
    private Rigidbody2D thisRB2D;
    public Image thisImage;
    private BoxCollider2D thisBoxCollider;

    public AudioClip[] DestructionNoises;

    private bool FallTriggered = false;

    [Header("Health and Damage")]
    public int Health = 1;
    public bool CanBeHurt = true;
    public bool CanHurtCity = true;
    public bool ScaleWithHealth = false;
    private Vector2 BaseScale;

    [Header("DeathSettings")]
    public float FinalRippleValue = 1f;
    public float DeathPeriod = 0.5f;
    public float DamagePeriod = 0.1f;

    [Header("Personal Speeds")]
    public float DropSpeed = 20f;
    public float WindSpeed = 0f;

    [Header("External Speeds")]
    public float FormationSpeedModifier = 1f;
    public static float WaveSpeedModifier = 1f;

    [Header("Prefabs")]
    public GameObject DestructionPingPrefab;
    public GameObject DetectionPingPrefab;

    [Header("Scanner Data")]
    public bool RadarNeeded = false;
    public float FadeInPeriod = 1.0f;
    public float FadeOutPeriod = 1.0f;
    public float MaintainPeriod = 0.5f;

    private Material RenderMaterial;
    private Coroutine DamageCoroutine;

    public UnityEvent OnStageEnter;
    public UnityEvent OnObjectDestroy;

    private bool Dying = false;

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
        thisBoxCollider = GetComponent<BoxCollider2D>();
        if(thisImage == null)
        {
            thisImage = GetComponent<Image>();
        }

        BaseScale = thisBoxCollider.size;

        if (thisImage != null) thisImage.maskable = true;

        Vector2 DownSpeed = Vector2.down * transform.parent.lossyScale * 75f;
        Vector2 TotalSpeed = DownSpeed * WaveSpeedModifier;
        thisRB2D.linearVelocity = TotalSpeed;

        if (ScaleWithHealth && Health > 1f) SetScale(1f + (Health - 1f) / 2f);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FallTrigger()
    {
        if (FallTriggered) return;
        FallTriggered = true;

        OnStageEnter?.Invoke();

        UpdatePath();

        WindScript.OnWindChanged += UpdatePath;

        if (thisImage == null) return;
        RenderMaterial = Instantiate(thisImage.material);
        thisImage.material = RenderMaterial;
        thisImage.SetMaterialDirty();
    }

    public void SetScale(float scale)
    {
        thisRB2D.simulated = false;
        thisBoxCollider.size = BaseScale * scale;
        thisRB2D.simulated = true;
        thisImage.transform.localScale = Vector3.one * scale;
    }

    public void UpdatePath()
    {
        Debug.Log($"Update Path: {WindScript.StageWind}");

        Vector2 DownSpeed = Vector2.down * DropSpeed * transform.parent.lossyScale;
        Vector2 HorizontalSpeed = Vector2.right * (WindSpeed + WindScript.StageWind) * transform.lossyScale;
        Vector2 TotalSpeed = (DownSpeed + HorizontalSpeed) * WaveSpeedModifier * FormationSpeedModifier;
        thisRB2D.linearVelocity = TotalSpeed;

        float angle = Mathf.Atan2(TotalSpeed.x, TotalSpeed.y) * Mathf.Rad2Deg;
        thisRB2D.MoveRotation(-angle - 180f);
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
        if (RenderMaterial != null)
            DestroyImmediate(RenderMaterial);

        WindScript.OnWindChanged -= UpdatePath;
        OnObjectDestroy?.Invoke();

        FallingThreatScripts.Remove(this);
    }

    public void SpawnExplosionPing()
    {
        if (DestructionPingPrefab == null) return;

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
        if (thisImage == null) return;
        if (!FallTriggered) return;
        if (Dying) return;

        Health -= damageValue;
        if(Health <= 0)
        {
            if (DamageCoroutine != null) StopCoroutine(DamageCoroutine);
            SpawnExplosionPing();
            if (source == null) Destroy(gameObject);
            else
            {
                DamageCoroutine = StartCoroutine(BurnAnimation(source.localPosition));
            }
        } else
        {
            DamageCoroutine = StartCoroutine(DamageAnimation(source.localPosition));
            if (ScaleWithHealth) SetScale(1f + (Health - 1f) / 2f);
        }
    }

    public IEnumerator DamageAnimation(Vector2 explosionPoint)
    {
        RenderMaterial = thisImage.canvasRenderer.GetMaterial();

        RenderMaterial.SetVector("_ImpactPoint", explosionPoint);

        RenderMaterial.SetFloat("_DamageGlow", 1f);

        float timePassed = 0f;

        Vector2 lastMovement = Vector2.zero;

        while(timePassed < DamagePeriod)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = timePassed / DamagePeriod;
            RenderMaterial.SetFloat("_DamageGlow", Mathf.Lerp(1f, 0f, progress));

            Vector2 newRandomMovement = Random.insideUnitCircle * 0.02f * (1 - progress);
            thisRB2D.position += newRandomMovement - lastMovement;
            lastMovement = newRandomMovement;
        }
        thisRB2D.position -= lastMovement;

        RenderMaterial.SetVector("_ImpactPoint", new Vector2(0, -2000f));
    }

    public IEnumerator BurnAnimation(Vector2 explosionPoint)
    {
        Dying = true;

        Debug.Log("STARTING THE BURN---------------------");
        RenderMaterial = thisImage.canvasRenderer.GetMaterial();

        thisRB2D.simulated = false;

        RenderMaterial.SetVector("_ImpactPoint", explosionPoint);
        RenderMaterial.SetFloat("_DamageGlow", 0f);

        float timePassed = 0f;
        while (timePassed < DeathPeriod)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = timePassed / DeathPeriod;
            float rippleValue = Mathf.Lerp(0.1f * FinalRippleValue, FinalRippleValue, progress);
            RenderMaterial.SetFloat("_RippleProgress", rippleValue);
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
