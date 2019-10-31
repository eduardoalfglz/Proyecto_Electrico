using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/**
 *@class STeam
 *@brief Scripts relacionados al equipo completo
 **/
public class STeam : MonoBehaviour
{

    public enum Destination
    {
        Initial = 0,
        Default = 1
    }
    //Variables to instatiate players
    //public SPlayer[] locals;
    private GameObject Local;
    public Vector3[] local_position;
    public Vector3[] local_init_position;
    //public SPlayer[] visitors;
    private GameObject Visit;
    public Vector3[] visit_position;
    public Vector3[] visit_init_position;
    public SPlayer playerPrefab;
    public SPlayer OpponentPrefab;
    //




    public List<SPlayer> Locals = new List<SPlayer>();
    public List<SPlayer> Visitors = new List<SPlayer>();


    //public CompositeBehavior behavior;


    
    [Range(1f, 10f)]
    public float neighborRadius = 5f;
    [Range(0f, 1f)]
    public float avoidanceRadiusMultiplier = 0.5f;

    public Dictionary<string, float> ControlTimes;
    public List<string> Tkeys;

    // Start is called before the first frame update
    void Start()
    {

        ControlTimes = new Dictionary<string, float>();
        Tkeys = new List<string>(); //FIXME: esto no tiene sentido

        ControlTimes.Add("Pass", 2f);
        //behavior = GameObject.FindObjectOfType<CompositeBehavior>() as CompositeBehavior;


        //Inicializacion de jugadores

        //Local
        Local = GameObject.Find("Local");
        //FIXME 16.9.19 Aqui se puede crear un archivo para guardar las posiciones predeterminadas de los jugadores, de esa forma crear "formaciones"
        local_position = new Vector3[10];

        //Defenders
        local_position[0] = new Vector3(-24.96248f, 0.2552834f, -42.93238f); //Posicion jugador 0
        local_position[1] = new Vector3(-6.817955f, 0.2552834f, -40.34002f); //Posicion jugador 1
        local_position[2] = new Vector3(5.819132f, 0.2552834f, -40.9691f); //Posicion jugador 2
        local_position[3] = new Vector3(22.76717f, 0.2552834f, -42.93238f); //Posicion jugador 3

        //Middlers
        local_position[4] = new Vector3(22.55046f, 0.2552834f, -24.74275f); //Posicion jugador 4
        local_position[5] = new Vector3(12.20201f, 0.2552834f, -22.64586f); //Posicion jugador 5
        local_position[6] = new Vector3(-0.4350719f, 0.2552834f, -22.01679f); //Posicion jugador 6
        local_position[7] = new Vector3(-17.71658f, 0.2552834f, -23.01151f); //Posicion jugador 7

        //Attackers
        local_position[8] = new Vector3(-6.5f, 0.2552834f, -10f); //Posicion jugador 8
        local_position[9] = new Vector3(6f, 0.2552834f, -9f); //Posicion jugador 8



        local_init_position = new Vector3[10];
        //FIXME: esto se tiene que cambiar, ahora me da pereza no tengo mouse y no tengo segundo monitor
        //Defenders
        local_init_position[0] = new Vector3(-23.4f, 0.2552834f, -31.4f); //Posicion jugador 0
        local_init_position[1] = new Vector3(-10.1f, 0.2552834f, -37.2f); //Posicion jugador 1
        local_init_position[2] = new Vector3(10.1f, 0.2552834f, -36.3f); //Posicion jugador 2
        local_init_position[3] = new Vector3(24.77f, 0.2552834f, -29.39f); //Posicion jugador 3
             
        //Middlers
        local_init_position[4] = new Vector3(20.46f, 0.2552834f, -8.85f); //Posicion jugador 4
        local_init_position[5] = new Vector3(7.65f, 0.2552834f, -13.26f); //Posicion jugador 5
        local_init_position[6] = new Vector3(-5.7f, 0.2552834f, -13.4f); //Posicion jugador 6
        local_init_position[7] = new Vector3(-21.1f, 0.2552834f, -8.7f); //Posicion jugador 7
             
        //Attackers
        local_init_position[8] = new Vector3(-1.5f, 0.2552834f, -1); //Posicion jugador 8
        local_init_position[9] = new Vector3(1.5f, 0.2552834f, -1); //Posicion jugador 8


        Visit = GameObject.Find("Visit");
        //FIXME 16.9.19 Aqui se puede crear un archivo para guardar las posiciones predeterminadas de los jugadores, de esa forma crear "formaciones"
        visit_position = new Vector3[10];
        visit_init_position = new Vector3[10];
        for (int i = 0; i < 10; i++) //Same position as local but mirrowed
        {
            visit_position[i] = local_position[i];
            visit_position[i].z = -visit_position[i].z;
            visit_init_position[i] = local_init_position[i];
            visit_init_position[i].z = -local_init_position[i].z+10f;
            //visit_init_position[i].z = -visit_position[i].z;
        }

        visit_init_position[8]= new Vector3(-6.2f, 0.2552834f, 8.8f); //Posicion jugador 8 visitante debe ser diferente a la del local
        visit_init_position[9] = new Vector3(6.59f, 0.2552834f, 8.4f); //Posicion jugador p visitante debe ser diferente a la del local


        if (this.name=="Local")
        {
            SPlayer tempPlayer;

            for (int i = 0; i < 10; i++)
            {
                tempPlayer = Instantiate(playerPrefab, local_position[i], Quaternion.identity, Local.transform);
                tempPlayer.name = string.Concat("Local_", i);
                tempPlayer.PlayerId = i;
                Locals.Add(tempPlayer);

                


            }
            for (int i = 0; i < 4; i++)
            {
                Locals[i].GetComponent<SPlayer>().type = SPlayer.TypePlayer.DEFENDER;
            }
            for (int i = 4; i < 8; i++)
            {
                Locals[i].GetComponent<SPlayer>().type = SPlayer.TypePlayer.MIDDLER;
            }

            Locals[8].GetComponent<SPlayer>().type = SPlayer.TypePlayer.ATTACKER;
            Locals[9].GetComponent<SPlayer>().type = SPlayer.TypePlayer.ATTACKER;

        }
        else if (this.name=="Visit")
        {
            SPlayer tempPlayer;

            for (int i = 0; i < 10; i++)
            {
                tempPlayer= Instantiate(OpponentPrefab, visit_position[i], Quaternion.identity, Visit.transform);
                tempPlayer.name = string.Concat("Visit_", i);
                tempPlayer.PlayerId = i;
                Visitors.Add(tempPlayer);
            }


            for (int i = 0; i < 4; i++)
            {
                Visitors[i].GetComponent<SPlayer>().type = SPlayer.TypePlayer.DEFENDER;
            }
            for (int i = 4; i < 8; i++)
            {
                Visitors[i].GetComponent<SPlayer>().type = SPlayer.TypePlayer.MIDDLER;
            }

            Visitors[8].GetComponent<SPlayer>().type = SPlayer.TypePlayer.ATTACKER;
            Visitors[9].GetComponent<SPlayer>().type = SPlayer.TypePlayer.ATTACKER;

        }

        //Local








    }

    // Update is called once per frame
    void Update()
    {
        //foreach (SPlayer player in Locals)
        //{
        //    List<Transform> context = GetTransforms(player);

            
        //    Vector3 move = behavior.CalculateMove(player, context, this);
        //    move *= driveFactor;
        //    if (move.sqrMagnitude > squareMaxSpeed)
        //    {
        //        move = move.normalized * maxSpeed;
        //    }
        //    player.Move(move);
        //}
    }


    /**
    *@funtion GetTransforms
    *@brief Devuelve al jugador el equipo al que pertenece
    *@param velocity velocidad calculada por CalculateMove
    **/
    public List<Transform> GetTransforms(SPlayer player)
    {
        List<Transform> context = new List<Transform>();
        //Collider[] contextColliders = Physics.OverlapSphere(player.transform.position, neighborRadius);
        if (player.transform.parent.name=="Local")
        {
            foreach (SPlayer c in Locals)
            {
                if (c != player)
                {
                    context.Add(c.transform);
                }
            }
        }

        if (player.transform.parent.name == "Visit")
        {
            foreach (SPlayer c in Visitors)
            {
                if (c != player)
                {
                    context.Add(c.transform);
                }
            }
        }

        return context;
    }
    /**
    *@funtion CheckDestination
    *@brief Revisa si todos los jugadores llegaron al destino deseado
    *@param destination Indica a cual posicion deben llegar
    *@param team equipo al que pertenece 
    **/
    public bool checkDestination(int destination)
    {
        if (destination == (int)Destination.Default)
        {
            for (int i = 0; i < Visitors.Count; i++)
            {
                Vector3 difference = Locals[i].transform.position - local_position[i];

                if (difference.magnitude>3f)//FIXME: esto se puede ajusta a un mejor valor
                {
                    return false;
                }
            }
            for (int i = 0; i < Visitors.Count; i++)
            {
                Vector3 difference = Visitors[i].transform.position - visit_position[i];

                if (difference.magnitude > 3f)//FIXME: esto se puede ajusta a un mejor valor
                {
                    return false;
                }
            }
        }
        if (destination == (int)Destination.Initial)
        {
            for (int i = 0; i < Locals.Count; i++)
            {
                Vector3 difference = Locals[i].transform.position - local_init_position[i];

                if (difference.magnitude > 3f)//FIXME: esto se puede ajusta a un mejor valor
                {
                    return false;
                }
            }
            for (int i = 0; i < Visitors.Count; i++)
            {
                Vector3 difference = Visitors[i].transform.position - visit_init_position[i];

                if (difference.magnitude > 3f)//FIXME: esto se puede ajusta a un mejor valor
                {
                    return false;
                }
            }
        }
        return true ;
    }


    /**
    *@funtion EstaturaJugadores
    *@brief Le cambia la estatura a los jugadores inicialmente.
    **/
    //FIXME: Esta funcion no ha sido revisada.
    void EstaturaJugadores(GameObject[] arrayPlayers, GameObject[] arrayPlayers2, GameObject portero1, GameObject portero2, float x1, float y1, float z1)
    {

        float x;
        float y;
        float z;
        foreach (GameObject go in arrayPlayers)
        {
            x = Random.Range(0.0f, x1);
            y = Random.Range(0.0f, y1);
            z = Random.Range(0.0f, z1);
            go.transform.localScale = new Vector3(go.transform.localScale.x + x, go.transform.localScale.y + y, go.transform.localScale.z + z);
        }
        foreach (GameObject go in arrayPlayers2)
        {
            x = Random.Range(0.0f, x1);
            y = Random.Range(0.0f, y1);
            z = Random.Range(0.0f, z1);
            go.transform.localScale = new Vector3(go.transform.localScale.x + x, go.transform.localScale.y + y, go.transform.localScale.z + z);
        }

        x = Random.Range(0.0f, x1);
        y = Random.Range(0.0f, y1);
        z = Random.Range(0.0f, z1);
        portero1.transform.localScale = new Vector3(portero1.transform.localScale.x + x, portero1.transform.localScale.y + y, portero1.transform.localScale.z + z);

        x = Random.Range(0.0f, x1);
        y = Random.Range(0.0f, y1);
        z = Random.Range(0.0f, z1);
        portero2.transform.localScale = new Vector3(portero2.transform.localScale.x + x, portero2.transform.localScale.y + y, portero2.transform.localScale.z + z);

    }
}
