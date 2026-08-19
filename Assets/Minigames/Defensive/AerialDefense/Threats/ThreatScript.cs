using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.UI;

[Serializable]
public class EnemyPhase
{
    public bool Triggered = false;
    public float HealthPercentageTrigger = 0f;
    public UnityEvent OnTriggerEvents;
}

public class ThreatScript : MonoBehaviour
{
    public static List<ThreatScript> FallingThreatScripts = new List<ThreatScript>();

    public bool KeepParent = false;

    [SerializeField] private AudioMixerGroup sfxGroup;
    public AudioClip[] DestructionNoises;
    public AudioClip[] DamageNoises;

    protected Rigidbody2D thisRB2D;
    public Image thisImage;
    protected BoxCollider2D thisBoxCollider;

    [Header("DeathSettings")]
    public float FinalRippleValue = 1f;
    public float DeathPeriod = 0.5f;
    public float DamagePeriod = 0.1f;
    protected bool Dying = false;
    public ThreatScript[] WeakPoints;

    [Header("Health and Damage")]
    public float InvulnerabilityTime = 0f;
    public int Health = 1;
    private int MaxHealth = 1;
    public bool CanBeHurt = true;
    public bool CanHurtCity = true;
    public bool ScaleWithHealth = false;
    public bool IndirectDamageOnly = false;
    protected Vector2 BaseScale;

    [Header("Prefabs")]
    public GameObject DestructionPingPrefab;

    protected Material RenderMaterial;
    protected Coroutine DamageCoroutine;

    public UnityEvent OnStageEnter;
    public UnityEvent OnObjectDestroy;

    [Header("Phases")]
    [SerializeField] private List<EnemyPhase> EnemyPhases = new();
    private List<EnemyPhase> EnemyPhasesClone = new();
    private int CurrentPhase = 0;

    public void Awake()
    {
        EnemyPhasesClone = new List<EnemyPhase>(EnemyPhases);
        MaxHealth = Health;

        FallingThreatScripts.Add(this);

        if (!KeepParent) {
            transform.SetParent(AerialDefenseScript.Instance.CombatSpawn);
            transform.rotation = Quaternion.identity;
        } 

        thisRB2D = GetComponent<Rigidbody2D>();
        thisBoxCollider = GetComponent<BoxCollider2D>();
        if (thisImage == null)
        {
            thisImage = GetComponent<Image>();
        }

        if(thisBoxCollider) BaseScale = thisBoxCollider.size;

        if (thisImage != null) thisImage.maskable = true;

        if (ScaleWithHealth && Health > 1f) SetScale(1f + (Health - 1f) / 2f);

        if (WeakPoints.Length > 0) SetupWeakPoints();

    }

    public void SetupWeakPoints()
    {
        Health = 0;
        foreach(ThreatScript threat in WeakPoints)
        {
            threat.OnObjectDestroy.AddListener(() => OnWeakPointDestroyed(threat));
            Health++;
        }
        if(Health > MaxHealth) MaxHealth = Health;

        WeakPointDamageImmunityPeriod(InvulnerabilityTime);
    }
    public void OnWeakPointDestroyed(ThreatScript threatSource)
    {
        TakeDamage(1, threatSource.transform, true);
    }

    public void SetupMaterial()
    {
        if (thisImage == null) return;
        RenderMaterial = Instantiate(thisImage.material);
        thisImage.material = RenderMaterial;
        thisImage.SetMaterialDirty();
    }

    public void SetScale(float scale)
    {
        thisRB2D.simulated = false;
        if (thisBoxCollider) thisBoxCollider.size = BaseScale * scale;
        thisRB2D.simulated = true;
        thisImage.transform.localScale = Vector3.one * scale;
    }

    public static void DestroyAllThreats()
    {
        foreach (FallingThreatScript fallingThreatScript in FallingThreatScripts)
        {
            fallingThreatScript.SpawnExplosionPing();
            Destroy(fallingThreatScript.gameObject);
        }
    }

    public IEnumerator DamageAnimation(Vector2 explosionPoint)
    {
        RenderMaterial = thisImage.canvasRenderer.GetMaterial();

        RenderMaterial.SetVector("_ImpactPoint", explosionPoint);

        RenderMaterial.SetFloat("_DamageGlow", 1f);

        AudioClip DamageNoise = DamageNoises.RandomlySelectValue();
        if (DamageNoise != null) AudioExtensions.PlayClipAtPointWithMixer(DamageNoise, transform.position, sfxGroup);

        float timePassed = 0f;

        Vector2 lastMovement = Vector2.zero;

        while (timePassed < DamagePeriod)
        {
            yield return null;
            timePassed += Time.deltaTime;
            float progress = timePassed / DamagePeriod;
            RenderMaterial.SetFloat("_DamageGlow", Mathf.Lerp(1f, 0f, progress));

            Vector2 newRandomMovement = UnityEngine.Random.insideUnitCircle * 0.02f * (1 - progress);
            thisRB2D.position += newRandomMovement - lastMovement;
            lastMovement = newRandomMovement;
        }
        thisRB2D.position -= lastMovement;

        RenderMaterial.SetVector("_ImpactPoint", new Vector2(0, -2000f));
    }

    public IEnumerator BurnAnimation(Vector2 explosionPoint)
    {
        Dying = true;

        AudioClip DestructionNoise = DestructionNoises.RandomlySelectValue();
        if(DestructionNoise != null) AudioExtensions.PlayClipAtPointWithMixer(DestructionNoise, transform.position, sfxGroup);

        Debug.Log("STARTING THE BURN---------------------");
        RenderMaterial = thisImage.canvasRenderer.GetMaterial();

        thisRB2D.simulated = false;

        Debug.Log(explosionPoint);

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

    protected void OnDestroy()
    {
        AerialDefenseScript.TotalProjectilesDestroyed += 1;

        if (RenderMaterial != null)
            DestroyImmediate(RenderMaterial);

        OnObjectDestroy?.Invoke();

        FallingThreatScripts.Remove(this);
    }

    public void DamageImmunityPeriod(float invulnerabilityTime)
    {
        InvulnerabilityTime = invulnerabilityTime;
        StartCoroutine(DamageWait());
    }
    public void WeakPointDamageImmunityPeriod(float invulnerabilityTime)
    {
        InvulnerabilityTime = invulnerabilityTime;
        foreach(ThreatScript ts in WeakPoints)
        {
            StartCoroutine(ts.DamageWait());
        }
    }

    public IEnumerator DamageWait()
    {
        CanBeHurt = false;
        Color prevColor = thisImage.color;
        thisImage.color = Color.grey;
        yield return new WaitForSeconds(InvulnerabilityTime);
        thisImage.color = prevColor;
        CanBeHurt = true;
    }

    public void SpawnExplosionPing()
    {
        if (DestructionPingPrefab == null) return;

        GameObject newPing = Instantiate(DestructionPingPrefab, transform.position, Quaternion.identity, AerialDefenseScript.StaticGameCanvas);
        AudioSource pingAudio = newPing.GetComponent<AudioSource>();
        if (pingAudio != null && DestructionNoises.Length > 0)
        {
            int randIdx = UnityEngine.Random.Range(0, DestructionNoises.Length);
            pingAudio.clip = DestructionNoises[randIdx];
            pingAudio.Play();
        }
    }

    public void TakeDamage(int damageValue, Transform source, bool indirectDaamge = false)
    {
        if (thisImage == null) return;
        if (!CanBeHurt) return;
        if (Dying) return;
        if (IndirectDamageOnly && !indirectDaamge) return;

        foreach (EnemyPhase ep in EnemyPhasesClone)
        {
            if (!ep.Triggered && ep.HealthPercentageTrigger > (float)Health / MaxHealth)
            {
                ep.Triggered = true;
                ep.OnTriggerEvents?.Invoke();
                CurrentPhase++;
                GetComponent<Animator>().SetInteger("Phase", CurrentPhase);
            }
        }

        Health -= damageValue;
        if (Health <= 0)
        {
            if (DamageCoroutine != null) StopCoroutine(DamageCoroutine);
            SpawnExplosionPing();
            if (source == null) Destroy(gameObject);
            else
            {
                DamageCoroutine = StartCoroutine(BurnAnimation(source.localPosition));
            }
        }
        else
        {
            DamageCoroutine = StartCoroutine(DamageAnimation(source.localPosition));
            if (ScaleWithHealth) SetScale(1f + (Health - 1f) / 2f);
        }
    }
}
