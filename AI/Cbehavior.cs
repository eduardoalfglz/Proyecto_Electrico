using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *@class Cbehavior
 *@brief Clase abstracta base para los comportamientos complejos
 **/
public abstract class Cbehavior : ScriptableObject
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
    public abstract Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team, STeam Oteam, Sphere Ball, GameObject Goal);
}
