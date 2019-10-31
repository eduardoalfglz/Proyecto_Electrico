using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *@class FollowOwner
 *@brief Clase que hereda de Cbehavior comportamiento seguir el movimiento del jugador que tiene el balon
 **/
public class FollowOwner : Cbehavior
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.6f;
    bool slowDown = false;
    /**
    *@funtion CalculateMove
    *@brief Calcula un vector de velocidad dependiendo del comportamiento
    *@param Player jugador al que le corresponde el comportamiento
    *@param context Lista de transforms del equipo
    *@param team Equipo al que pertenece
    *@param Oteam Equipo oponente   
    *@param Ball Balon del juego
    *@param Goal Cancha objetivo
    **/
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team, STeam Oteam, Sphere Ball, GameObject Goal)
    {
        slowDown = false;
        Vector3 centerOffset;
        centerOffset.x = 0f;    //La posicion en x realmente no importa
        if (Ball.owner==null)
        {
            return Vector3.zero;
        }
        centerOffset.y = 0f;
        centerOffset.z = Ball.owner.transform.position.z - Player.transform.position.z;
        //
        bool bFirstHalf;

        if (Player.TeamName == "Local")
        {
            bFirstHalf = Player.bFirstHalf;
        }
        else
        {
            bFirstHalf = !Player.bFirstHalf;
        }
        if (Player.type==SPlayer.TypePlayer.DEFENDER)//Si es defensor empezar a seguir cuando la distancia llega a 40
        {
            if (bFirstHalf)
            {
                if (Player.transform.position.z>0)
                {
                    return Vector3.zero;
                }
            }
            else
            {
                if (Player.transform.position.z < 0)
                {
                    return Vector3.zero;
                }
            }
            if (centerOffset.magnitude < 40f)
            {
                slowDown = true;
            }

        }

        if (Player.type == SPlayer.TypePlayer.MIDDLER && centerOffset.magnitude < 15f)//Si es mediocampista empezar a seguir cuando la distancia llega a 15
        {
            slowDown = true;
        }

        if (Player.type == SPlayer.TypePlayer.ATTACKER && centerOffset.magnitude < 2f )
        {
            slowDown = true;
        }
        if (slowDown)
        {
            centerOffset /= 3;
            agentSmoothTime = 0.9f;
            centerOffset = Vector3.Lerp(centerOffset, Vector3.zero, agentSmoothTime);
            return centerOffset.normalized;
        }


        Debug.Log("Error de delantero");

        //centerOffset = centerOffset.normalized*Player.MAXSPEED;
        //centerOffset = Vector3.SmoothDamp(Player.transform.forward, centerOffset, ref currentVelocity, agentSmoothTime);
        //Debug.Log(centerOffset.normalized,this);
        return centerOffset.normalized;
    }

}
