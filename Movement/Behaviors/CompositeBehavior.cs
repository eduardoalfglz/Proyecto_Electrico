using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Documentacion:
//Ultima vez editado 18-10-19
//Nota importante, este script ya no se va a usar, se migraron sus componentes a SPlayer para poder modificar el comportamiento según el jugador
//[CreateAssetMenu(menuName = "Movement/Behavior/Composite")]
public class CompositeBehavior : MonoBehaviour
{
    
    public Sbehavior[] behaviors;
    public float[] weights;
    
    private void Start()
    {
        //behaviors = Resources.FindObjectsOfTypeAll<Sbehavior>();
        //weights = new float[behaviors.Length];
        //for (int i=0; i<weights.Length; i++)
        //{
        //    weights[i] = 1;
        //}
        //Debug.Log(behaviors.Length);
    }
    public  Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team)
    {
        //handle data mismatch
        if (weights.Length != behaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        //set up move
        Vector3 move = Vector3.zero;

        //iterate through behaviors
        for (int i = 0; i < behaviors.Length; i++)
        {
            Vector3 partialMove = behaviors[i].CalculateMove(Player, context, team) * weights[i];

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[i] * weights[i])
                {
                    partialMove.Normalize();
                    partialMove *= weights[i];
                }

                move += partialMove;

            }
        }

        return move;


    }
}

