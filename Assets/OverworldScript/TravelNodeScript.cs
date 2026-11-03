using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct TravelNodeConnection
{
    public TravelNodeScript TravelNode;
    public bool ExitWithAnimation;
    public bool EnterWithAnimation;
    public float WaitBeforeStart;
    public UnityEvent AnimationCommand;
    public float WaitBeforeEnd;
    public UnityEvent AfterAnimationCommand;
    public bool ForceAngleOnArrival;
    public bool FlipAngle;
}

public class TravelNodeScript : MonoBehaviour
{
    public string NodeName;
    public int SceneID;
    public List<TravelNodeConnection> ConnectedNodes = new List<TravelNodeConnection>();

    public Color lineColor = Color.yellow;

    private void OnDrawGizmosSelected()
    {

        Gizmos.color = Color.red;
        Gizmos.DrawLine(transform.position, transform.forward + transform.position);

        if (ConnectedNodes == null || ConnectedNodes.Count == 0)
            return;

        Gizmos.color = lineColor;

        foreach (TravelNodeConnection target in ConnectedNodes)
        {
            if (target.TravelNode != null)
            {
                Gizmos.DrawLine(transform.position, target.TravelNode.transform.position);
            }
        }
    }
}
