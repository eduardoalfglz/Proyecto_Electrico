using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/**
 *@class Defend
 *@brief Clase que hereda de Cbehavior comportamiento Encontrar un jugador cerca y moverse hacia el
 **/
public class Defend : Cbehavior
{
    float Timeb4changeAtt=0;
    float offset=0;
    float TempValue;        //Variable temporal de la diferencia más pequeña
    bool AttackingSide;     //Bandera que indica si el jugadore esta en la mitad de ataque
    SPlayer attacker;
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
        //Debug.Log("Defending");
        Vector3 centerOffset=Vector3.zero;
        //attacker = null;
        List<SPlayer> opponents;
        if (Ball.owner!=null)
        {
            if (Player.type == SPlayer.TypePlayer.DEFENDER && Ball.owner.OnAreaCheck())//Fixme, no dejar a otros jugadores solos
            {
                centerOffset.y = 0f;
                centerOffset.x = Ball.owner.transform.position.x - Player.transform.position.x;
                centerOffset.z = Ball.owner.transform.position.z - Player.transform.position.z;
                attacker = Ball.owner;
                return centerOffset.normalized;
            }
        }
        
        if (Player.bFirstHalf)
        {

            if (Player.transform.position.z > 0f)
                AttackingSide = true;
            else
                AttackingSide = false;
            offset = -3f;
        }
        else
        {
            if (Player.transform.position.z < 0f)
                AttackingSide = true;
            else
                AttackingSide =false;
            offset = 3f;
        }
        if (Player.TeamName == "Local")
            opponents = Oteam.Visitors;            
        else
            opponents = Oteam.Locals;



        if (Timeb4changeAtt<=0)
        {
            Timeb4changeAtt = 2f;
            Vector3 tempOffset=new Vector3(0,10,0);
            
            if (!AttackingSide)
            {
                //Debug.Log("Defendiendo");
                for (int i = 0; i < opponents.Count; i++)
                {
                    //Debug.Log(opponents[i].CheckAround(3));
                    if (opponents[i].CheckAround(3) <= 2) //Checks for other defenders near the attacker
                    {
                        TempValue = (opponents[i].transform.position - Player.transform.position).magnitude;
                        if (opponents[i] == Ball.owner)//Prioridad al dueno del balon
                        {
                            TempValue /= 2;
                        }
                        if (opponents[i].PlayerId==Player.PlayerId)
                        {
                            TempValue /= 4;
                        }
                        if (TempValue < tempOffset.magnitude)
                        {
                            centerOffset.y = 0f;
                            centerOffset.x = opponents[i].transform.position.x - Player.transform.position.x;
                            centerOffset.z = opponents[i].transform.position.z - Player.transform.position.z;

                            tempOffset = centerOffset;
                            attacker = opponents[i];
                        }
                    }

                }
                
            }

            //Debug.Log(attacker);
            if (centerOffset.magnitude==0)
            {
                
                if (Player.playerTeam.name == "Local")
                {
                    centerOffset.x = team.local_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.local_position[Player.PlayerId].z - Player.transform.position.z;
                    if (centerOffset.magnitude < 2f)
                    {
                        centerOffset = Vector3.zero;
                    }
                }
                else
                {
                    centerOffset.x = team.visit_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.visit_position[Player.PlayerId].z - Player.transform.position.z;
                    if (centerOffset.magnitude < 2f)
                    {
                        centerOffset = Vector3.zero;
                    }
                }
            }

        }
        else
        {
            Timeb4changeAtt -= Time.deltaTime;
            if (attacker!=null)
            {
                centerOffset.y = 0f;
                centerOffset.x = attacker.transform.position.x - Player.transform.position.x;
                centerOffset.z = attacker.transform.position.z - Player.transform.position.z +offset;

            }
            else
            {
                if (Player.playerTeam.name == "Local")
                {
                    centerOffset.x = team.local_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.local_position[Player.PlayerId].z - Player.transform.position.z;
                    if (centerOffset.magnitude < 2f)
                    {
                        centerOffset = Vector3.zero;
                    }
                }
                else
                {
                    centerOffset.x = team.visit_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.visit_position[Player.PlayerId].z - Player.transform.position.z;
                    if (centerOffset.magnitude < 2f)
                    {
                        centerOffset = Vector3.zero;
                    }
                }
            }


        }
        //Debug.Log(centerOffset,Player);
        return centerOffset.normalized;
    }

}
