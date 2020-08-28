using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//[CreateAssetMenu(menuName = "Movement/Behavior/StayInStadium")]
/**
 *@class StayInStadium
 *@brief Clase que hereda de Sbehavior, mantener velocidad constante en una direccion
 **/
public class StayInStadium : Sbehavior
{
    private Vector3 center = new Vector3(0, 0, 0);
    private float Xlimit = 40;
    private float Zlimit = 60;
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
        centerOffset.x = center.x - Player.transform.position.x;
        centerOffset.z = center.z - Player.transform.position.z;
        centerOffset.y = 0f;

        float x = centerOffset.x / Xlimit;
        float z = centerOffset.z / Zlimit;
        if (x < 0.9f && z < 0.95f)
        {
            //Debug.Log("adentro de limites");
            return Vector3.zero;
        }else if (x < 0.95f && z > 0.95f)
        {
            //Debug.Log("fuera de limites");
            return centerOffset * z * z;
        }else if (x > 0.95f && z < 0.95f)
        {
            //Debug.Log("fuera de limites");
            return centerOffset * x * x;
        }
        //Debug.Log("fuera de limites");
        return centerOffset * x * z;
    }
}
