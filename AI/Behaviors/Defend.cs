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
        Vector3 centerOffset=Vector3.zero;
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
        
        if (Player.TeamName == "Local")
        {
            opponents = Oteam.Visitors;
        }
        else
        {
            opponents = Oteam.Locals;
        }
        if (Timeb4changeAtt<=0)
        {
            Timeb4changeAtt = 3f;
            Vector3 tempOffset=new Vector3(0,15,0);
            for (int i = 0; i < opponents.Count; i++)
            {
                if ((opponents[i].transform.position-Player.transform.position).magnitude<tempOffset.magnitude)
                {
                    centerOffset.y = 0f;
                    centerOffset.x = opponents[i].transform.position.x - Player.transform.position.x;
                    centerOffset.z = opponents[i].transform.position.z - Player.transform.position.z;
                    if (opponents[i]==Ball.owner)//Prioridad al dueno del balon
                    {
                        centerOffset /= 10;
                    }
                    attacker = opponents[i];
                }
            }

            if (centerOffset.magnitude==0)
            {
                attacker = null;
                if (Player.playerTeam.name == "Local")
                {
                    centerOffset.x = team.local_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.local_position[Player.PlayerId].z - Player.transform.position.z;
                }
                else
                {
                    centerOffset.x = team.visit_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.visit_position[Player.PlayerId].z - Player.transform.position.z;
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
                centerOffset.z = attacker.transform.position.z - Player.transform.position.z;

            }
            else
            {
                if (Player.playerTeam.name == "Local")
                {
                    centerOffset.x = team.local_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.local_position[Player.PlayerId].z - Player.transform.position.z;
                }
                else
                {
                    centerOffset.x = team.visit_position[Player.PlayerId].x - Player.transform.position.x;
                    centerOffset.z = team.visit_position[Player.PlayerId].z - Player.transform.position.z;
                }
            }
            
            
        }
        return centerOffset.normalized;
    }

}
