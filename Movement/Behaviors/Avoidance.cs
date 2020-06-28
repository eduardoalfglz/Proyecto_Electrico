using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "Movement/Behavior/Avoidance")]
/**
 *@class Avoidance
 *@brief Clase que hereda de Sbehavior, comportamiento de evitar a otros jugadores del mismo equipo
 **/
public class Avoidance : Sbehavior
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.5f;
    /**
    *@funtion CalculateMove
    *@brief Calcula un vector de velocidad dependiendo del comportamiento
    *@param Player jugador al que le corresponde el comportamiento
    *@param context Lista de transforms del equipo
    *@param team Equipo al que pertenece
    **/
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team)
    {
        
        //if no neighbors, maintain current alignment
        if (context.Count == 0)
            return Player.transform.up;

        //add all points together and average
        Vector3 avoidanceMove = Vector3.zero;
        int nAvoid = 0;
        float SquareAvoidanceRadius = 4f;
        foreach (Transform item in context)
        {
            if (Vector3.SqrMagnitude(item.position - Player.transform.position) < SquareAvoidanceRadius)
            {
                //Debug.Log(context.Count);
                nAvoid++;
                avoidanceMove += (Player.transform.position - item.position);
            }
        }
        if (nAvoid > 0)
            avoidanceMove /= nAvoid;
        else
            return Vector3.zero;

        avoidanceMove = Vector3.SmoothDamp(Player.transform.up, avoidanceMove, ref currentVelocity, agentSmoothTime);
        avoidanceMove.x *= 10;
        avoidanceMove.z /= 3;
        return avoidanceMove;
    }
}

