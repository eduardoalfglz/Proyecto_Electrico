using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *@class Sbehavior
 *@brief Clase abstracta base para los comportamientos simples
 **/
public abstract class Sbehavior : ScriptableObject
{
    /**
    *@funtion CalculateMove
    *@brief Calcula un vector de velocidad dependiendo del comportamiento
    *@param Player jugador al que le corresponde el comportamiento
    *@param context Lista de transforms del equipo
    *@param team Equipo al que pertenece
    **/ 
    public abstract Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team);
}
