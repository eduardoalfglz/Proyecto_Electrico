using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Obsoleto
[CreateAssetMenu(menuName = "Movement/Filter/Same Flock")]
public class SameFlockFilter : ContextFilter
{
    public override List<Transform> Filter(SPlayer agent, List<Transform> original)
    {
    //    List<Transform> filtered = new List<Transform>();
    //    foreach (Transform item in original)
    //    {
    //        SPlayer itemAgent = item.GetComponent<SPlayer>();
    //        if (itemAgent != null && itemAgent.PlayerTeam == agent.PlayerTeam)
    //        {
    //            filtered.Add(item);
    //        }
    //    }
        return null;
    }
}
