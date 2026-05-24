using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class CartScript : MonoBehaviour
{
    public static CartScript instance;

    public float Speed = 0.1f;

    private Transform ReturnPoint;

    private List<Transform> PickupTargets = new List<Transform>();

    bool Searching = false;

    public void Awake()
    {
        instance = this;
        ReturnPoint = transform.parent;
    }


    public static void AddTarget(Transform newTarget)
    {
        instance.PickupTargets.Add(newTarget);
        instance.StartCoroutine(instance.Pickup());
    }

    public IEnumerator Pickup()
    {
        if (Searching) yield break;

        Searching = true;

        while(PickupTargets.Count > 0)
        {
            transform.position = Vector3.MoveTowards(transform.position, PickupTargets[0].position, Speed * Time.deltaTime);

            if(Vector3.Distance(transform.position, PickupTargets[0].position) < 0.0001f)
            {
                transform.position = PickupTargets[0].position;
                Destroy(PickupTargets[0].gameObject);
                PickupTargets.Remove(PickupTargets[0]);
            }

            yield return null;
        }

        while (Vector3.Distance(transform.position, ReturnPoint.position) > 0.0001f)
        {
            transform.position = Vector3.MoveTowards(transform.position, ReturnPoint.position, Speed * Time.deltaTime);
            yield return null;
        }

        transform.localPosition = Vector3.zero;

        Searching = false;

        if(PickupTargets.Count > 0)
        {
            StartCoroutine(Pickup());
        }
    }
}
