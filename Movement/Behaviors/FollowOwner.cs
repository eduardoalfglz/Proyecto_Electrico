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
    Vector3 centerOffset;
    Vector3 GoalMod;
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
        Player.Walking = false;
        GoalMod.z = Goal.transform.position.z;
        GoalMod.y = 0f;
        if (Player.playerTeam.name == "Local")
        {
            GoalMod.x = team.local_position[Player.PlayerId].x;
            
        }
        else
        {
            GoalMod.x = team.visit_position[Player.PlayerId].x;
            
        }
        
        slowDown = false;
        
        centerOffset.x = 0f;    //La posicion en x realmente no importa
        if (Ball.owner==null)
        {
            return Vector3.zero;
        }
        centerOffset.y = 0f;
        centerOffset.z = Player.transform.InverseTransformPoint(Ball.owner.transform.position).z;
        //
        
        if (!Player.bFirstHalf)
        {
            centerOffset = -centerOffset;
        }
        if (Player.type==SPlayer.TypePlayer.DEFENDER)//Si es defensor empezar a seguir cuando la distancia llega a 40
        {
            
            if (centerOffset.magnitude < 40f)
            {
                slowDown = true;
            }

        }

        if (Player.type == SPlayer.TypePlayer.MIDDLER && centerOffset.magnitude < 20f)//Si es mediocampista empezar a seguir cuando la distancia llega a 15
        {
            slowDown = true;
        }

        if (Player.type == SPlayer.TypePlayer.ATTACKER && centerOffset.magnitude < 10f )
        {
            slowDown = true;
        }
        if (slowDown)
        {
            Player.Walking = true;
            centerOffset.x = GoalMod.x-Player.transform.position.x;
            centerOffset.z = GoalMod.z-Player.transform.position.z;
            centerOffset /= 3;
            return centerOffset.normalized;
        }


        

        //centerOffset = centerOffset.normalized*Player.MAXSPEED;
        //centerOffset = Vector3.SmoothDamp(Player.transform.forward, centerOffset, ref currentVelocity, agentSmoothTime);
        //Debug.Log(centerOffset.normalized,this);
        return centerOffset.normalized;
    }

}
