using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *@class AvoidOpponents
 *@brief Clase que hereda de Cbehavior comportamiento de evitar oponentes a la hora de atacar
 **/
public class AvoidOpponents : Cbehavior
{
    Vector3 currentVelocity;
    public float agentSmoothTime = 0.9f;
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
        
        //add all points together and average
        Vector3 AvoidOpponents = Vector3.zero;
        int nAvoid = 0;

        List<SPlayer> temp;
        if (Player.transform.parent.name == "Local")
            temp = Oteam.Visitors;
        else
            temp = Oteam.Locals;
        float SquareAvoidanceRadius = 25;
        foreach (SPlayer item in temp)
        {
            if (Vector3.SqrMagnitude(item.transform.position - Player.transform.position) < SquareAvoidanceRadius)
            {
                nAvoid++;
                AvoidOpponents += (Player.transform.position - item.transform.position);
            }
        }
        
        if (nAvoid > 0)
            AvoidOpponents /= nAvoid;
        else
            return Player.transform.up;
        float speedDivider=20;
        if (Player.transform.parent.name == "Local") //Reduce el movimiento hacia atras
        {
            if (Player.bFirstHalf)
            {
                if (AvoidOpponents.z<0)
                {
                    AvoidOpponents.z =0;
                }
            }
            else
            {
                if (AvoidOpponents.z > 0)
                {
                    AvoidOpponents.z =0;
                }
            }
        }
        else
        {
            if (!Player.bFirstHalf)
            {
                if (AvoidOpponents.z < 0)
                {
                    AvoidOpponents.z =0;
                }
            }
            else
            {
                if (AvoidOpponents.z > 0)
                {
                    AvoidOpponents.z =0;
                }
            }
        }
        
        AvoidOpponents.x *= 10;
        AvoidOpponents = Vector3.SmoothDamp(Player.transform.up, AvoidOpponents, ref currentVelocity, agentSmoothTime);
        return AvoidOpponents;
    }
    
}
