using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "Movement/Behavior/Cohesion")]
/**
 *@class Cohesion
 *@brief Clase que hereda de Sbehavior, comportamiento de converger todos los jugadores a un punto central
 **/
public class Cohesion : Sbehavior
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
        Vector3 cohesionMove = Vector3.zero;
        

        //List<Transform> filteredContext = (filter == null) ? context : filter.Filter(Player, context);
        foreach (Transform item in context)
        {
            cohesionMove += item.position;
            
        }
        cohesionMove /= context.Count;
        cohesionMove -= Player.transform.position;
        cohesionMove = Vector3.SmoothDamp(Player.transform.up, cohesionMove, ref currentVelocity, agentSmoothTime);
        return cohesionMove;
    }
}

