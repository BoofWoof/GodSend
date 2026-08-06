using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ThreatScript : MonoBehaviour
{
    public static List<ThreatScript> FallingThreatScripts = new List<ThreatScript>();

    public bool KeepParent = false;

    public AudioClip[] DestructionNoises;

    protected Rigidbody2D thisRB2D;
    public Image thisImage;
    protected BoxCollider2D thisBoxCollider;

    [Header("DeathSettings")]
    public float FinalRippleValue = 1f;
    public float DeathPeriod = 0.5f;
    public float DamagePeriod = 0.1f;
    protected bool Dying = false;

    [Header("Health and Damage")]
    public int Health = 1;
    public bool CanBeHurt = true;
    public bool CanHurtCity = true;
    public bool ScaleWithHealth = false;
    protected Vector2 BaseScale;

    [Header("Prefabs")]
    public GameObject DestructionPingPrefab;

    protected Material RenderMaterial;
    protected Coroutine DamageCoroutine;

    public UnityEvent OnStageEnter;
    public UnityEvent OnObjectDestroy;

    public void Awake()
    {
        FallingThreatScripts.Add(this);

        if(!KeepParent) transform.SetParent(ThreatSpawnerScript.Instance.transform);
        transform.rotation = Quaternion.identity;

        thisRB2D = GetComponent<Rigidbody2D>();
        thisBoxCollider = GetComponent<BoxCollider2D>();
        if (thisImage == null)
        {
            thisImage = GetComponent<Image>();
        }

        if(thisBoxCollider) BaseScale = thisBoxCollider.size;

        if (thisImage != null) thisImage.maskable = true;

        if (ScaleWithHealth && Health > 1f) SetScale(1f + (Health - 1f) / 2f);
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

        float timePassed = 0f;

        Vector2 lastMovement = Vector2.zero;

        while (timePassed < DamagePeriod)
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

    protected void OnDestroy()
    {
        AerialDefenseScript.TotalProjectilesDestroyed += 1;

        if (RenderMaterial != null)
            DestroyImmediate(RenderMaterial);

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
        if (!CanBeHurt) return;
        if (Dying) return;

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
