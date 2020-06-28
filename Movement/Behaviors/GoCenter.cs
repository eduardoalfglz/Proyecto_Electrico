using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *@class GoCenter
 *@brief Clase que hereda de Sbehavior, ir al centro de la cancha
 **/
public class GoCenter : Sbehavior
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
        Vector3 centerOffset;
        centerOffset.x = Player.center.transform.position.x - Player.transform.position.x;
        centerOffset.z = Player.center.transform.position.z - Player.transform.position.z;
        centerOffset.y = 0f;





        return centerOffset.normalized;
    }

    
}
