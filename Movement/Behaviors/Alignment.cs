using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Movement/Behavior/Alignment")]
public class Alignment : Sbehavior
{
   
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team)
    {
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
            return Player.transform.up;

        //add all points together and average
        Vector3 alignmentMove = Vector3.zero;
        //List<Transform> filteredContext = (filter == null) ? context : filter.Filter(Player, context);
        foreach (Transform item in context)
        {
            alignmentMove += item.transform.up;
        }
        alignmentMove /= context.Count;

        return alignmentMove;
    }
}

