using System.Collections;
using UnityEngine;

public class FallingThreatScript : ThreatScript
{
    private bool FallTriggered = false;

    public bool LockRotation = false;

    [Header("Personal Speeds")]
    public bool StartImmediately = false;
    public Vector2 InitialSpeedBoost;
    public float DecayRate = 0.5f;
    private float StartFallTime = 0f;
    public float DropSpeed = 20f;
    public float WindSpeed = 0f;
    public float HorizontalWave = 0f;
    public float HorizontalWaveFrequency = 0f;

    private float StartSwayTime;

    [Header("External Speeds")]
    public float FormationSpeedModifier = 1f;
    public static float WaveSpeedModifier = 1f;

    public static bool isEnemiesRemaining()
    {
        return FallingThreatScripts.Count > 0;
    }

    public void Awake()
    {
        base.Awake();

        Vector2 DownSpeed = Vector2.down * transform.parent.lossyScale * 75f;
        Vector2 TotalSpeed = DownSpeed * WaveSpeedModifier;
        thisRB2D.linearVelocity = TotalSpeed;
        CanBeHurt = false;

        if (StartImmediately) FallTrigger();
    }

    private void OnDestroy()
    {
        base.OnDestroy();
        WindScript.OnWindChanged -= SingleUpdatePath;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void FallTrigger()
    {
        if (FallTriggered) return;
        FallTriggered = true;

        StartCoroutine(DamageWait());

        OnStageEnter?.Invoke();

        StartFallTime = Time.time;

        if (InitialSpeedBoost.magnitude > 0 || HorizontalWave > 0)
        {
            StartCoroutine(ContinuousUpdatePath());
        } else
        {
            SingleUpdatePath();
            WindScript.OnWindChanged += SingleUpdatePath;
        }

        SetupMaterial();
    }

    public void SingleUpdatePath()
    {
        Vector2 DownSpeed = Vector2.down * DropSpeed * transform.parent.lossyScale;
        Vector2 HorizontalSpeed = Vector2.right * (WindSpeed + WindScript.StageWind) * transform.lossyScale;
        Vector2 TotalSpeed = (DownSpeed + HorizontalSpeed) * WaveSpeedModifier * FormationSpeedModifier;
        thisRB2D.linearVelocity = TotalSpeed;

        float angle = Mathf.Atan2(TotalSpeed.x, TotalSpeed.y) * Mathf.Rad2Deg;
        thisRB2D.MoveRotation(-angle - 180f);
    }
    public IEnumerator ContinuousUpdatePath()
    {
        StartSwayTime = Time.time;
        while (true)
        {
            yield return new WaitForFixedUpdate();

            float timePassed = Time.time - StartFallTime;
            float decay = Mathf.Exp(-DecayRate * timePassed);
            float dropRate = DropSpeed + (decay * InitialSpeedBoost.y);
            float horzRate = decay * InitialSpeedBoost.x;

            float swayTimePassed = Time.time - StartSwayTime;
            float horzWaveSpeed = HorizontalWave * Mathf.Cos(swayTimePassed * 2f * Mathf.PI * HorizontalWaveFrequency);

            Vector2 DownSpeed = Vector2.down * dropRate * transform.parent.lossyScale;
            Vector2 HorizontalSpeed = Vector2.right * (WindSpeed + WindScript.StageWind + horzRate + horzWaveSpeed) * transform.lossyScale;
            Vector2 TotalSpeed = (DownSpeed + HorizontalSpeed) * WaveSpeedModifier * FormationSpeedModifier;
            thisRB2D.linearVelocity = TotalSpeed;

            if (!LockRotation)
            {
                float angle = Mathf.Atan2(TotalSpeed.x, TotalSpeed.y) * Mathf.Rad2Deg;
                thisRB2D.MoveRotation(-angle - 180f);
            }
        }
    }

    public void SetSwayAmplitude(float amplitude)
    {

        StartSwayTime = Time.time;
        HorizontalWave = amplitude;
    }
    public void SetSwayFrequency(float frequency)
    {

        StartSwayTime = Time.time;
        HorizontalWaveFrequency = frequency;
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
