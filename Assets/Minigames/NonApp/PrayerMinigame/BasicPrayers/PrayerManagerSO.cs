using System.Collections.Generic;
using UnityEngine;
using static BasicPrayerGroupSO;

[CreateAssetMenu(fileName = "PrayerManagerSO", menuName = "Prayers/PrayerManagerSO")]
public class PrayerManagerSO : ScriptableObject
{
    public List<BasicPrayerGroupSO> PrayerGroupList = new();

    public void PrepareLists()
    {
        foreach (BasicPrayerGroupSO prayerGroup in PrayerGroupList)
        {
            prayerGroup.PrepareGroup();
        }
    }

    public PrayerData SamplePrayer()
    {
        return SampleGroup().SamplePrayer();
    }

    public BasicPrayerGroupSO SampleGroup()
    {
        float totalWeight = 0;
        foreach (BasicPrayerGroupSO prayerGroup in PrayerGroupList)
        {
            totalWeight += prayerGroup.GetPrayerCount();
        }

        if (totalWeight <= 0) return default;

        float randomPoint = Random.Range(0, totalWeight);

        foreach (BasicPrayerGroupSO prayerGroup in PrayerGroupList)
        {
            int weight = prayerGroup.GetPrayerCount();
            if (weight <= 0f) continue;

            if(randomPoint < weight)
            {
                return prayerGroup;
            }
            randomPoint -= weight;
        }


        return default;
    }
}
