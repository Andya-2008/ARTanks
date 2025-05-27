using UnityEngine;
using Unity.Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class cineScript : MonoBehaviour
{
    public CinemachineTargetGroup targetGroup;

    public void AddTarget(Transform target)
    {
        CinemachineTargetGroup.Target newTarget = new CinemachineTargetGroup.Target { Object = target, Weight = 1, Radius = 0.5f };
        targetGroup.Targets.Add(newTarget);
    }

    public void RemoveTarget(Transform target)
    {
        foreach (CinemachineTargetGroup.Target t in targetGroup.Targets) {
            if (t.Object.transform == target) {
                targetGroup.Targets.Remove(t);
            }
        }

    }
}