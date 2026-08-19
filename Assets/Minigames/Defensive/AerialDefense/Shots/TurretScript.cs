using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class TurretScript : MonoBehaviour
{
    public RectTransform canvasSpace;
    public RectTransform target;
    public RectTransform gun;

    public GameObject blastObject;

    public Image chargeMeter;
    public float ChargeRate = 1f;
    public float CurrentCharge = 0f;
    public float MaxCharge = 1f;
    public float OnHitRechargePercent = 0.5f;

    public float ShotCost = 0.25f;

    public DiageticTurretScript diageticTurretScript;

    public static bool autoFire = false;
    public delegate void TurretFiredDelegate(TurretScript turretScript);
    public static TurretFiredDelegate TurretFiredEvent;

    public GameObject AimBeam;

    public UnityEvent OnShoot;

    public void Start()
    {
        //AimBeam.SetActive(false);
    }

    public bool IsTurretCharged()
    {
        float chargePercentage = CurrentCharge / MaxCharge;
        return chargePercentage >= ShotCost;
    }

    public bool FireBeam()
    {
        if (!IsTurretCharged()) return false;

        OnShoot?.Invoke();

        GameObject newBlast = Instantiate(blastObject);
        newBlast.transform.SetParent(AerialDefenseScript.Instance.BlastSpawn);
        RectTransform newRectTransform = newBlast.GetComponent<RectTransform>();
        newRectTransform.position = gun.position;
        newRectTransform.rotation = gun.rotation;
        newRectTransform.localScale = Vector3.one;

        newBlast.GetComponent<Image>().color = AimBeam.GetComponent<Image>().color;


        newBlast.GetComponent<ADBlastScript>().SetSource(this);

        if (diageticTurretScript != null) diageticTurretScript.Fire();
        CurrentCharge -= ShotCost;
        TurretFiredEvent?.Invoke(this);

        return true;
    }

    // Update is called once per frame
    void Update()
    {
        CurrentCharge += Time.deltaTime * ChargeRate;
        if (CurrentCharge > MaxCharge) CurrentCharge = MaxCharge;
        float chargePercentage = CurrentCharge / MaxCharge;
        chargeMeter.fillAmount = chargePercentage;

        // Get direction from turret to mouse (in local canvas space)
        Vector2 canvasGunPos = (Vector2)canvasSpace.InverseTransformPoint(gun.position);

        Vector2 dir = (Vector2)target.localPosition - canvasGunPos;

        // Calculate angle (atan2 gives radians, convert to degrees)
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        // Apply rotation (z-axis since it’s 2D UI element)
        gun.localRotation = Quaternion.Euler(0, 0, angle - 90f);
    }

    public void HitRegenerate()
    {
        CurrentCharge += ShotCost * OnHitRechargePercent;
        if (CurrentCharge > MaxCharge) CurrentCharge = MaxCharge;
    }
}
