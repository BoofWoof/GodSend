using UnityEngine;
using UnityEngine.Audio;

public class AddThreatSpawner : MonoBehaviour
{
    public GameObject[] ThreatPrefabs;

    public AudioClip SpawnNoise;
    [SerializeField] private AudioMixerGroup TargetGroup;

    [Header("Release Stats")]
    public Vector2Int BurstSizeRange = new Vector2Int(1, 1);
    public bool StartImmediately = false;
    public bool ReleasePeriodically = false;
    public float[] PeriodicWaitLoop;

    public Vector2 AngleRange;
    public Vector2 StartingSpeedRange;


    public void SpawnThreats()
    {
        if (SpawnNoise) AudioExtensions.PlayClipAtPointWithMixer(SpawnNoise, transform.position, TargetGroup);

        int burstSize = Random.Range(BurstSizeRange.x, BurstSizeRange.y + 1);
        for (int i = 0; i < burstSize; i++)
        {
            int targetIdx = Random.Range(0, ThreatPrefabs.Length);
            GameObject newThreat = Instantiate(ThreatPrefabs[targetIdx]);
            newThreat.transform.SetParent(AerialDefenseScript.Instance.CombatSpawn);
            newThreat.transform.position = transform.position;
            newThreat.transform.rotation = transform.rotation;
            newThreat.transform.localScale = Vector3.one;

            FallingThreatScript fallingScript = newThreat.GetComponent<FallingThreatScript>();
            fallingScript.KeepParent = true;

            float angle = Random.Range(AngleRange.x, AngleRange.y) * Mathf.Deg2Rad;
            float speed = Random.Range(StartingSpeedRange.x, StartingSpeedRange.y);

            fallingScript.InitialSpeedBoost = new Vector2 (speed * Mathf.Cos(angle), speed * Mathf.Sin(angle)/2f);
        }
    }
}
