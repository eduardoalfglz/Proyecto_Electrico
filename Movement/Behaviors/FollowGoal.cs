using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *@class Cbehavior
 *@brief Clase que hereda de Cbehavior comportamiento de seguir la cancha objetivo
 **/
public class FollowGoal : Cbehavior
{
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

        Vector3 centerOffset;
        centerOffset.x = Goal.transform.position.x - Player.transform.position.x;
        centerOffset.z = Goal.transform.position.z - Player.transform.position.z;
        centerOffset.y = 0f;





        return centerOffset.normalized;
    }

}
