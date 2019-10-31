using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/**
 *@class TackleMove
 *@brief Clase que hereda de Sbehavior, mantener velocidad constante en una direccion
 **/
public class TackleMove : Sbehavior
{
    /**
    *@funtion CalculateMove
    *@brief Calcula un vector de velocidad dependiendo del comportamiento
    *@param Player jugador al que le corresponde el comportamiento
    *@param context Lista de transforms del equipo
    *@param team Equipo al que pertenece
    **/
    public override Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team)
    {
        return Player.transform.forward.normalized;
    }

    
}
