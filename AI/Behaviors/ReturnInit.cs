using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnInit : Cbehavior
{
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team, STeam Oteam, Sphere Ball, GameObject Goal)
    {
        Vector3 centerOffset;
        centerOffset.y = 0f;
        if (Player.playerTeam.name == "Local")
        {
            centerOffset.x = team.local_init_position[Player.PlayerId].x - Player.transform.position.x;
            centerOffset.z = team.local_init_position[Player.PlayerId].z - Player.transform.position.z;
        }
        else
        {
            centerOffset.x = team.visit_init_position[Player.PlayerId].x - Player.transform.position.x;
            centerOffset.z = team.visit_init_position[Player.PlayerId].z - Player.transform.position.z;
        }
        
        if (centerOffset.magnitude < 0.4f)//FIXME: esto se puede ajustar a un mejor valor
        {
            return Vector3.zero;
        }
        return centerOffset.normalized;
    }

}
