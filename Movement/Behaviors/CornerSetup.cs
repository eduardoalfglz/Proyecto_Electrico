using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerSetup : Cbehavior
{
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team, STeam Oteam, Sphere Ball, GameObject Goal)
    {
        Vector3 centerOffset;
        centerOffset.y = 0f;
        if (Player.bFirstHalf)//ataque en derecha
        {
            if (Ball.OutPosition.z<0)   //Defensa de corner
            {
                centerOffset.x = -team.defend_corner_position[Player.PlayerId].x - Player.transform.position.x; //negativa porque se define en la derecha
                centerOffset.z = -team.defend_corner_position[Player.PlayerId].z - Player.transform.position.z;
            }
            else
            {
                if (Ball.OutPosition.x<0 && Player.PlayerId==8)
                {
                    centerOffset.x = -50 - Player.transform.position.x; 
                    centerOffset.z = 57 - Player.transform.position.z;
                }
                else if (Ball.OutPosition.x > 0 && Player.PlayerId == 9)
                {
                    centerOffset.x = 38.5f - Player.transform.position.x; 
                    centerOffset.z = 57 - Player.transform.position.z;
                }
                else
                {
                    centerOffset.x = team.attack_corner_position[Player.PlayerId].x - Player.transform.position.x; 
                    centerOffset.z = team.attack_corner_position[Player.PlayerId].z - Player.transform.position.z;
                }
                
            }
        }
        else
        {
            if (Ball.OutPosition.z < 0)   //ataque en corner
            {
                if (Ball.OutPosition.x < 0 && Player.PlayerId == 8)
                {
                    centerOffset.x = -38.5f - Player.transform.position.x;
                    centerOffset.z = -57 - Player.transform.position.z;
                }
                else if (Ball.OutPosition.x > 0 && Player.PlayerId == 9)
                {
                    centerOffset.x = 38.5f - Player.transform.position.x;
                    centerOffset.z = -57 - Player.transform.position.z;
                }
                else
                {
                    centerOffset.x = -team.attack_corner_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = -team.attack_corner_position[Player.PlayerId].z - Player.transform.position.z; //Negativa por que se define a la derecha
                }
            }
            else
            {

                centerOffset.x = team.defend_corner_position[Player.PlayerId].x - Player.transform.position.x; 
                centerOffset.z = team.defend_corner_position[Player.PlayerId].z - Player.transform.position.z;
            }
        }
        
        if (centerOffset.magnitude < 2f)
        {
            centerOffset = Vector3.zero;
        }


        return centerOffset.normalized;
    }
}
