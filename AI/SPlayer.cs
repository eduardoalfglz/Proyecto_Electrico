using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
/**
 *@class SPlayer
 *@brief Script encargado del comportamiento general del jugador
 **/
public class SPlayer : MonoBehaviour
{
    [HideInInspector]
    public int PlayerId;

    public enum TypePlayer
    {
        DEFENDER,
        MIDDLER,
        ATTACKER
    };

    public TypePlayer type = TypePlayer.DEFENDER;

    [HideInInspector]
    public STeam playerTeam;
    public List<SPlayer> Teammates;
    [HideInInspector]
    public string TeamName;

    [HideInInspector]
    public STeam opponentTeam;
    public List<SPlayer>  Opponents;

    Collider playerCollider;
    public Collider PlayerCollider { get { return playerCollider; } }


    //Componentes de comportamiento
    public Sbehavior[] sbehaviors;
    public Cbehavior[] cbehaviors;

    public Dictionary<string, float> weights;
    public float[] pweights;
    public string[] wkeys;


    //Componentes del ambiente
    [HideInInspector]
    public Sphere sphere;
    [HideInInspector]
    public InGameState_Script inGame;
    [HideInInspector]
    public GameObject GoalLocal;
    [HideInInspector]
    public GameObject GoalVisit;
    [HideInInspector]
    public Transform center;
    [HideInInspector]
    public bool bFirstHalf;

    
    Vector3 VelocityXZ;



    //
    //capacidad de resistencia física
    private bool Walking = false;
    private const float STAMINA_DIVIDER = 64.0f;
    private const float STAMINA_MIN = 0.5f;
    private const float STAMINA_MAX = 1.0f;
    
    [Range(1f, 100f)]
    public float stamina = 64f;

    [Range(1f, 100f)]
    public float MAXSPEED = 5f;
    public float WalkingSPEED = 2f;





    //Behavior Tree Nodes
    //###############################################################################
    public Selector RootNode;
    public Selector TestRoot;       //
    //###############################################################################
    public Inverter FootballMatch;
    public Sequence Attack;
    public Sequence Deffend;
    public Sequence Setup;


    //###############################################################################
    Selector CustomSim; //Para uso futuro
    //###############################################################################


    //Action nodes
    //###############################################################################
    public ActionNode KICK_OFFER_Node;   //Accion de ejecutar el primer saque          
    public ActionNode GO_ORIGIN_Node;   //Puede ser que no sea necesario, esto lo hace inGameState
    public ActionNode CONTROLLING_Node; //Ni idea
    public PublicNode PASSING_Node;     //


    public ActionNode SHOOTING_Node;
    public ActionNode CHECK_ATTACK_Node;
    public ActionNode MOVE_AUTOMATIC_ATTACK_Node;



    public ActionNode MOVE_AUTOMATIC_DEFFEND_Node;  //Set Weights
    public ActionNode ONE_STEP_BACK_Node;       //Para animacion de salto atras
    public ActionNode STOLE_BALL_Node;
    public ActionNode PICK_BALL_Node;
    public ActionNode CHANGE_DIRECTION_Node;
    public ActionNode CORNER_KICK_Node;
    public ActionNode TACKLE_Node;
    public ActionNode Medio_Tiempo_Node;
    public ActionNode Celebration_Node;
    public ActionNode LOOK_FOR_BALL_Node;

    //Variables de control
    public Dictionary<string, bool> ControlStates;
    public List<string> Ckeys;

    public Dictionary<string, float> ControlTimes;
    public List<string> Tkeys;


    // Start is called before the first frame update
    void Start()
    {
        
        inGame = FindObjectOfType<InGameState_Script>();
        center = GameObject.Find("Center_Field").GetComponent<Transform>();

        bFirstHalf = inGame.bFirstHalf;
        //Inicializacion de variables de comportamiento
        sbehaviors = new Sbehavior[6];

        cbehaviors = new Cbehavior[7];
        weights = new Dictionary<string, float>();
        pweights = new float[sbehaviors.Length + cbehaviors.Length];
        wkeys = new string[sbehaviors.Length + cbehaviors.Length];


        ControlStates = new Dictionary<string, bool>();
        Ckeys = new List<string>(); //FIXME: esto no tiene sentido

        ControlTimes = new Dictionary<string, float>();
        Tkeys = new List<string>(); //FIXME: esto no tiene sentido

        //Inicializacion de variables de ambiente
        sphere = GameObject.Find("soccer_ball").GetComponent<Sphere>();
        GoalLocal = GameObject.Find("GoalRight");
        GoalVisit = GameObject.Find("GoalLeft");



        if (this.transform.parent.name == "Local")
        {
            TeamName = "Local";
            playerTeam = GameObject.Find("Local").GetComponent<STeam>();
            opponentTeam = GameObject.Find("Visit").GetComponent<STeam>();
            Opponents = opponentTeam.Visitors;
            Teammates = playerTeam.Locals;
        }
        else
        {
            TeamName = "Visit";
            playerTeam = GameObject.Find("Visit").GetComponent<STeam>();
            opponentTeam = GameObject.Find("Local").GetComponent<STeam>();
            Opponents = opponentTeam.Locals;
            Teammates = playerTeam.Visitors;
        }
        Initialize();
        playerCollider = GetComponent<Collider>();
        GetComponentInChildren<SkinnedMeshRenderer>().material = Resources.Load("Materials/" + "player_texture_" + PlayerPrefs.GetString(transform.parent.name)) as Material;


        //Animaciones------------------------------------------------------------------------------------------------
        GetComponent<Animation>()["jump_backwards_bucle"].speed = 1.5f;
        GetComponent<Animation>()["starting"].speed = 1.0f;
        GetComponent<Animation>()["starting_ball"].speed = 1.0f;
        GetComponent<Animation>()["running"].speed = 1.2f;
        GetComponent<Animation>()["running_ball"].speed = 1.0f;
        GetComponent<Animation>()["pass"].speed = 1.8f;
        GetComponent<Animation>()["rest"].speed = 1.0f;
        GetComponent<Animation>()["turn"].speed = 2f;
        GetComponent<Animation>()["tackle"].speed = 4.0f;

        GetComponent<Animation>()["fight"].speed = 1.2f;
        // para el movimiento de la cabeza de los jugadores

        GetComponent<Animation>().Play("rest");
    }

    /**
    *@funtion Initialize
    *@brief Crea las variables iniciales de comportamiento y nodos
    *@param 
    **/
    public void Initialize()
    {
        playerTeam = transform.parent.GetComponent<STeam>();


        sbehaviors[0] = ScriptableObject.CreateInstance("Alignment") as Sbehavior;
        weights.Add("Alignment", 0.1f);
        wkeys[0] = "Alignment";
        sbehaviors[1] = ScriptableObject.CreateInstance("Avoidance") as Sbehavior;
        weights.Add("Avoidance", 0.1f);
        wkeys[1] = "Avoidance";
        sbehaviors[2] = ScriptableObject.CreateInstance("Cohesion") as Sbehavior;
        weights.Add("Cohesion", 0.1f);
        wkeys[2] = "Cohesion";
        sbehaviors[3] = ScriptableObject.CreateInstance("StayInStadium") as Sbehavior;
        weights.Add("StayInStadium", 0.1f);
        wkeys[3] = "StayInStadium";
        sbehaviors[4] = ScriptableObject.CreateInstance("GoCenter") as Sbehavior;
        weights.Add("GoCenter", 0.1f);
        wkeys[4] = "GoCenter";
        sbehaviors[5] = ScriptableObject.CreateInstance("TackleMove") as Sbehavior;
        weights.Add("TackleMove", 0.1f);
        wkeys[5] = "TackleMove";

        cbehaviors[0] = ScriptableObject.CreateInstance("FollowBall") as Cbehavior;
        weights.Add("FollowBall", 0.1f);
        wkeys[sbehaviors.Length] = "FollowBall";
        cbehaviors[1] = ScriptableObject.CreateInstance("FollowGoal") as Cbehavior;
        weights.Add("FollowGoal", 0.1f);
        wkeys[sbehaviors.Length+1] = "FollowGoal";
        cbehaviors[2] = ScriptableObject.CreateInstance("ReturnDefault") as Cbehavior;
        weights.Add("ReturnDefault", 0.1f);
        wkeys[sbehaviors.Length + 2] = "ReturnDefault";
        cbehaviors[3] = ScriptableObject.CreateInstance("ReturnInit") as Cbehavior;
        weights.Add("ReturnInit", 0.1f);
        wkeys[sbehaviors.Length + 3] = "ReturnInit";
        cbehaviors[4] = ScriptableObject.CreateInstance("AvoidOpponents") as Cbehavior;
        weights.Add("AvoidOpponents", 0.1f);
        wkeys[sbehaviors.Length + 4] = "AvoidOpponents";
        cbehaviors[5] = ScriptableObject.CreateInstance("FollowOwner") as Cbehavior;
        weights.Add("FollowOwner", 0.1f);
        wkeys[sbehaviors.Length + 5] = "FollowOwner";
        cbehaviors[6] = ScriptableObject.CreateInstance("Defend") as Cbehavior;
        weights.Add("Defend", 0.1f);
        wkeys[sbehaviors.Length + 6] = "Defend";





        ///Inicialización de nodos
        KICK_OFFER_Node = new ActionNode(KICK_OFFER);
        CHECK_ATTACK_Node = new ActionNode(CHECK_ATTACK);
        GO_ORIGIN_Node = new ActionNode(GO_ORIGIN);
        CONTROLLING_Node = new ActionNode(CONTROLLING);
        PASSING_Node = new PublicNode();
        SHOOTING_Node = new ActionNode(SHOOTING);
        MOVE_AUTOMATIC_ATTACK_Node = new ActionNode(MOVE_AUTOMATIC_ATTACK);
        MOVE_AUTOMATIC_DEFFEND_Node = new ActionNode(MOVE_AUTOMATIC_DEFFEND);
        ONE_STEP_BACK_Node = new ActionNode(ONE_STEP_BACK);
        STOLE_BALL_Node = new ActionNode(STOLE_BALL);
        PICK_BALL_Node = new ActionNode(PICK_BALL);
        CHANGE_DIRECTION_Node = new ActionNode(CHANGE_DIRECTION);
        CORNER_KICK_Node = new ActionNode(CORNER_KICK);
        TACKLE_Node = new ActionNode(TACKLE);
        Medio_Tiempo_Node = new ActionNode(Medio_Tiempo);
        Celebration_Node = new ActionNode(Celebration);
        LOOK_FOR_BALL_Node= new ActionNode(LOOK_FOR_BALL);

        //Behavior Tree Nodes
        //###############################################################################
        FootballMatch = new Inverter(KICK_OFFER_Node);
        //###############################################################################
        //public Sequence FootballMatch;
        Attack = new Sequence(new List<Node> {
            CHECK_ATTACK_Node,
            MOVE_AUTOMATIC_ATTACK_Node,
            //PASSING_Node,
            SHOOTING_Node,
        });

        Deffend = new Sequence(new List<Node> {
            MOVE_AUTOMATIC_DEFFEND_Node,
            TACKLE_Node,
            STOLE_BALL_Node,
        });


        RootNode = new Selector(new List<Node> {
            FootballMatch,
            Attack,
            LOOK_FOR_BALL_Node,
            Deffend,
        });

        TestRoot = new Selector(new List<Node> {
            FootballMatch,
            SHOOTING_Node,
        });







        //Bools de control;
        ControlStates.Add("Kick_Offer", false);
        Ckeys.Add("Kick_Offer");
        ControlStates.Add("Animate", true);
        Ckeys.Add("Animate");
        ControlStates.Add("Passing", false);
        Ckeys.Add("Passing");
        ControlStates.Add("OnArea", false);
        Ckeys.Add("OnArea");
        ControlStates.Add("Tackling", false);
        Ckeys.Add("Tackling");
        ControlStates.Add("StoleBall", false);
        Ckeys.Add("StoleBall");
        ControlStates.Add("Shooting", false);
        Ckeys.Add("Shooting");


        //Tiempos de espera
        ControlTimes.Add("Thinking", 0f);
        Tkeys.Add("Thinking");
        ControlTimes.Add("Tackle", 0f);
        Tkeys.Add("Tackle");
        ControlTimes.Add("Pass", 0f);
        Tkeys.Add("Pass");
    }


    void Update()
    {

        MovePlayer();
        bFirstHalf = inGame.bFirstHalf;
        RootNode.Evaluate();
        //TestRoot.Evaluate();

        
        for (int i = 0; i < pweights.Length; i++) { pweights[i] = weights[wkeys[i]]; } //mostrar los pesos en el inspector

    }


    public void MovePlayer()
    {
        List<Transform> context = playerTeam.GetTransforms(this);


        Vector3 move = CalculateMove(this, context, playerTeam);
        move = move.normalized;
        float staminaTemp = Mathf.Clamp((stamina / STAMINA_DIVIDER), STAMINA_MIN, STAMINA_MAX);

        if (Walking)
        {
            move *= staminaTemp;
            move *= WalkingSPEED;
            GetComponent<Animation>()["running"].speed = 0.7f;
        }
        else
        {
            move *= staminaTemp;
            move *= MAXSPEED;
            GetComponent<Animation>()["running"].speed = 1.2f;
        }
        //Debug.Log(move);
        this.Move(move);
    }
    



    /**
    *@funtion Move.
    *@brief Considerando la velocidad calculada por CalculateMove mueve al jugador
    *@param velocity velocidad calculada por CalculateMove
    **/
    public void Move(Vector3 velocity)
    {
        if (sphere.owner==this)
        {
            Debug.Log(ControlStates["Animate"]);
        }
        VelocityXZ = new Vector3(velocity.x, 0, velocity.z);

        if (VelocityXZ.magnitude <= 1)
        {
            Vector3 centerOffset;
            centerOffset.x = sphere.transform.position.x - transform.position.x;
            centerOffset.z = sphere.transform.position.z - transform.position.z;
            centerOffset.y = 0f;

            transform.forward = centerOffset;
            if (ControlStates["Animate"])
            {
                GetComponent<Animation>().Play("rest");
            }
            transform.position += VelocityXZ * Time.deltaTime/1000;
        }
        else
        {
            GetComponent<Animation>()["running"].speed *= (VelocityXZ.magnitude / MAXSPEED);
            if (Walking)
            {
                stamina -= 0.5f * Time.deltaTime;
                float staminaTemp = Mathf.Clamp((stamina / STAMINA_DIVIDER), STAMINA_MIN, STAMINA_MAX);
                GetComponent<Animation>()["running"].speed *= staminaTemp;
            }
            else
            {
                stamina -= 0.01f * Time.deltaTime;
                float staminaTemp = Mathf.Clamp((stamina / STAMINA_DIVIDER), STAMINA_MIN, STAMINA_MAX);
                GetComponent<Animation>()["running"].speed *= staminaTemp;
            }

            if (ControlStates["Animate"])
            {
                GetComponent<Animation>().Play("running");
            }
            Quaternion PRotation = transform.rotation;
            transform.forward = Vector3.Lerp(transform.forward, VelocityXZ, 0.8f);
            
                
            transform.rotation = Quaternion.Slerp(PRotation, transform.rotation, 0.5f);

            //}

            transform.position += VelocityXZ * Time.deltaTime;
        }

        
    }
    /**
    *@funtion CalculateMove
    *@brief Considera los comportamientos y sus respectivos pesos y calcula el movimiento del jugador
    *@param context contexto del jugador, en este caso equipo y obstaculos
    *@param team equipo al que pertenece 
    **/
    public Vector3 CalculateMove(SPlayer Player, List<Transform> context, STeam team)
    {
        //handle data mismatch
        if (weights.Count != sbehaviors.Length + cbehaviors.Length)
        {
            Debug.LogError("Data mismatch in " + name, this);
            return Vector3.zero;
        }

        //set up move
        Vector3 move = Vector3.zero;

        //iterate through sbehaviors
        for (int i = 0; i < sbehaviors.Length; i++)
        {
            Vector3 partialMove=Vector3.zero;
            if (weights[wkeys[i]]!=0)
            {
                partialMove = sbehaviors[i].CalculateMove(Player, context, team) * weights[wkeys[i]];
            }
            

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[wkeys[i]] * weights[wkeys[i]])
                {
                    partialMove.Normalize();
                    partialMove *= weights[wkeys[i]];
                }

                move += partialMove;

            }
        }
        for (int i = 0; i < cbehaviors.Length; i++)
        {
            Vector3 partialMove = Vector3.zero;
            if (weights[wkeys[i + sbehaviors.Length]] != 0)
            {
                partialMove = cbehaviors[i].CalculateMove(Player, context, team, opponentTeam, sphere, (TeamName == "Local") ? GoalLocal : GoalVisit) * weights[wkeys[i + sbehaviors.Length]];
            }
            

            if (partialMove != Vector3.zero)
            {
                if (partialMove.sqrMagnitude > weights[wkeys[i + sbehaviors.Length]] * weights[wkeys[i + sbehaviors.Length]])
                {
                    partialMove.Normalize();
                    partialMove *= weights[wkeys[i + sbehaviors.Length]];
                }
                //Debug.Log(i+sbehaviors.Length);
                move += partialMove;

            }
        }

        return move;


    }




    ///######################################################################################################################
    //////#############################################Nodos#################################################################
    //////###################################################################################################################
    ///######################################################################################################################
    //Accion de ejecutar el primer saque
    //Esperar
    //Puede ser que no sea necesario, esto lo hace inGameState
    //Ni idea
    //

    //Set Weight

    /**
    *@funtion KICK_OFFER
    *@brief Ejecutar la patada inicial
    *@return Retorna exito cuando la patada inicial se ejecuta o si no es el jugador al que le corresponde realizarla
    **/
    private NodeStates KICK_OFFER()
    {
        if (KICK_OFFER_Node.nodeState==NodeStates.SUCCESS)
        {
            ControlStates["Kick_Offer"] = false;
            return NodeStates.SUCCESS;
        }
        
        if (ControlStates["Kick_Offer"]==false && inGame.KICK_OFF_Node.nodeState==NodeStates.SUCCESS)
        {            
            return NodeStates.SUCCESS;
        }else if(ControlStates["Kick_Offer"] == false)
        {
            return NodeStates.FAILURE;
        }
        if (inGame.PREPARE_TO_KICK_OFF_Node.nodeState != NodeStates.SUCCESS)
        {
            //Debug.Log("Kick_Offer_failure", this);
            return NodeStates.FAILURE;
        }

        //mover al centro de la cancha

        Vector3 centerOffset = transform.position - center.transform.position;
        if (centerOffset.magnitude>0.7f)
        {

            for (int i = 0; i < weights.Count; i++)
            {
                weights[wkeys[i]] = 0;
            }
            weights["GoCenter"] = 1;
            sphere.owner = this;
            ControlStates["Passing"] = true;
            return NodeStates.RUNNING;
        }

        for (int i = 0; i < weights.Count; i++)
        {
            weights[wkeys[i]] = 0;
        }
        if (inGame.KICK_OFF_Node.nodeState==NodeStates.RUNNING)
        {
            return NodeStates.RUNNING;
        }
        //Debug.Log(sphere.owner);
        return PASSING();


        //Debo mover al jugador al centro de la cancha y forzar un pase
        
    }

    /**
    *@funtion CHECK_ATTACK
    *@brief Revisa si el actual equipo es el dueño del balon
    *@return Retorna exito si se tiene el balon
    **/
    private NodeStates CHECK_ATTACK()
    {
        List<SPlayer> temp;
        if (TeamName == "Local")
            temp = playerTeam.Locals;
        else
            temp = playerTeam.Visitors;

        for (int i = 0; i < 10; i++)
        {
            if (sphere.owner == temp[i])
                //
                return NodeStates.SUCCESS;
        }
        
        return NodeStates.FAILURE;
    }

    private NodeStates GO_ORIGIN()
    {
        return NodeStates.FAILURE;
    }

    private NodeStates CONTROLLING()
    {
        return NodeStates.FAILURE;
    }

    /**
    *@funtion PASSING
    *@brief Pase de balon
    *@return Retorna exito cuando la animacion se completa
    **/
    private NodeStates PASSING()
    {
        if (ControlStates["Passing"]==true)
        {
            //Debug.Log(sphere.owner);
            GetComponent<Animation>().Play("pass");
            ControlStates["Animate"] = false;
        }
        
        transform.position = new Vector3(transform.position.x, 0.255f, transform.position.z); //Linea para devolver el jugado a la altura deseada

        if (GetComponent<Animation>().IsPlaying("pass") == false)
        {
            ControlStates["Animate"] = true;
            PASSING_Node.ChangeState(NodeStates.SUCCESS);
            return NodeStates.SUCCESS;
        }
            


        if (GetComponent<Animation>()["pass"].normalizedTime > 0.3f && sphere.owner == this)
        {

            //Debug.Log("ejecutando el paso");
            SPlayer bestCandidatePlayer = null;
            float bestCandidateCoord = 1000.0f;

            //-------------------------- Modifiqué----------------

            if (sphere.owner!=null)
            {
                List<SPlayer> team;
                if (TeamName == "Local")
                {
                    team= playerTeam.Locals;
                }
                else
                {
                    team = playerTeam.Visitors;
                }
                foreach (SPlayer go in team)
                {
                    if (go != gameObject)
                    {
                        Vector3 relativePos = transform.InverseTransformPoint(go.transform.position);

                        float magnitude = relativePos.magnitude;
                        float direction = Mathf.Abs(relativePos.x);
                        

                        if (relativePos.z > 0.0f)
                            relativePos/=10f;       //Dar prioridad a los jugadores adelante del que tiene el balon
                        if (relativePos.z > 0.0f && direction < 15.0f && (magnitude + direction < bestCandidateCoord))
                        {
                            bestCandidateCoord = magnitude + direction;
                            bestCandidatePlayer = go;   //mejor jugador candidato	
                        }

                    }

                }
            }
               
            if (bestCandidatePlayer==null)
            {
                PASSING_Node.ChangeState(NodeStates.FAILURE);
                return NodeStates.FAILURE;
            }


                
            
            //WORKINGHERE

            if (bestCandidateCoord != 1000.0f)
            {

                //sphere.inputPlayer = bestCandidatePlayer;
                //El balon va hacia el candidato
                Vector3 directionBall = (bestCandidatePlayer.transform.position - transform.position).normalized;
                float distanceBall = (bestCandidatePlayer.transform.position - transform.position).magnitude * 1.4f;
                distanceBall = Mathf.Clamp(distanceBall, 15.0f, 40.0f);//Delimita el valor entre min y max
                sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x * distanceBall, distanceBall / 5.0f, directionBall.z * distanceBall);
                sphere.owner = null;
                
                ControlStates["Passing"] = false;
                PASSING_Node.ChangeState(NodeStates.RUNNING);
                return NodeStates.RUNNING;
            }
            else
            {
                // if not found a candidate just throw the ball forward....
                //sphere.gameObject.GetComponent<Rigidbody>().velocity = transform.forward * 20.0f;
                PASSING_Node.ChangeState(NodeStates.FAILURE);
                return NodeStates.FAILURE;
                //Si no se encuentra un candidato solo tira la pelota hacia adelante.
                

            }



        }
        PASSING_Node.ChangeState(NodeStates.FAILURE);
        return NodeStates.FAILURE;
    }
    /**
    *@funtion SHOOTING
    *@brief Tiro a marco.
    *@return Retorna exito cuando la animacion se completa
    **/
    private NodeStates SHOOTING()
    {

        ControlStates["Animate"] = false;
        if (GetComponent<Animation>().IsPlaying("shoot") == false)
        {
            ControlStates["Animate"] = true;
            return NodeStates.SUCCESS;
        }

        //Debug.Log(GetComponent<Animation>()["shoot"].normalizedTime);
        if (GetComponent<Animation>()["shoot"].normalizedTime > 0.3f && sphere.owner == this)
        {
            sphere.owner = null;
            if (TeamName=="Local")
            {
                //---------------------------ORIGINAL----------------------------------
                //sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f, 5.0f, transform.forward.z*30.0f );
                //barPosition = 0;
                
                //----------------Cambie----------------------------------------------------
                float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);   //FIXME: Se Puede mejorar
                //ControlStates["Animate"] = true;    //FIXME:esto va a hacer que se corte la animación
                return NodeStates.RUNNING;
                //--------------------------------------------------------------------
            }

            else
            {
                //Fixme: porque putas esta en un if?
                float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                sphere.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);
                return NodeStates.RUNNING;
            }

        }
        Debug.Log("Shoot");
        return NodeStates.RUNNING;
    }



    /**
    *@funtion MOVE_AUTOMATIC_ATTACK
    *@brief Movimiento en ataque, es el nodo encargado de establecer los pesos para los comportamientos de movimiento
    *@return RUNNING si esta lejos de la cancha y tiene la bola, SUCCESS si llega al area contrincante, RUNNING si no tiene el balon
    **/
    private NodeStates MOVE_AUTOMATIC_ATTACK()
    {
        if (SHOOTING_Node.nodeState == NodeStates.RUNNING)
        {

            return SHOOTING();

        }
        if (PASSING_Node.nodeState!=NodeStates.SUCCESS)
        {
            Debug.Log("Pasando");
            PASSING();
            if(PASSING_Node.nodeState == NodeStates.SUCCESS) { return NodeStates.RUNNING; }
            else { return PASSING_Node.nodeState; }
        }
        
        if (sphere.owner == this)
        {
            ControlTimes["Tackle"] = 5f;      //Esperar despues de perder el balon 
            //Debug.Log("Dueño", this);
            for (int j = 0; j < weights.Count; j++)
            {
                weights[wkeys[j]] = 0f;
            }

            weights["Alignment"] = 0.5f;
            weights["Avoidance"] = 1f;
            weights["StayInStadium"] = 1f;
            weights["FollowGoal"] = 2f;
            weights["AvoidOpponents"] = 1f;

            if (PassDecision()  && !ControlStates["Shooting"])
            {
                //weights["FollowGoal"] = 1f;
                //weights["AvoidOpponents"] = 0f;

                ControlTimes["Pass"] = 5f;
                playerTeam.ControlTimes["Pass"] = 3f;
                ControlStates["Passing"] = true;
                PASSING();
                if (PASSING_Node.nodeState == NodeStates.SUCCESS) { return NodeStates.RUNNING; }
                else { return PASSING_Node.nodeState; }
            }
            ControlTimes["Pass"] -= Time.deltaTime;
            playerTeam.ControlTimes["Pass"] -= Time.deltaTime;
            if (OnAreaCheck())
            {
                //Debug.Log("EnArea");
                ControlStates["OnArea"] = true;
                ControlStates["Shooting"] = true;
                GetComponent<Animation>().Play("shoot");
                return NodeStates.SUCCESS;  //FIXME esto lo estoy haciendo para que cuando entre al area dispare, hay que poner más condiciones para disparo, FUTUREWORK
            }
            return NodeStates.RUNNING;
        }
        else
        {

            for (int j = 0; j < weights.Count; j++)
            {
                weights[wkeys[j]] = 0f;
            }

            weights["Alignment"] = 0.1f;
            weights["Avoidance"] = 1f;
            weights["StayInStadium"] = 1f;
            weights["FollowBall"] = 0.05f;
            weights["AvoidOpponents"] = 1f;
            weights["FollowOwner"] = 2f;
            weights["FollowGoal"] = 0.5f;
            return NodeStates.RUNNING;
        }
        
        //return NodeStates.FAILURE;
    }


    /**
    *@funtion MOVE_AUTOMATIC_DEFFEND
    *@brief Movimiento en defensa, es el nodo encargado de establecer los pesos para los comportamientos de movimiento
    *@return RUNNING si esta lejos de la cancha y tiene la bola, SUCCESS si llega al area contrincante, RUNNING si no tiene el balon
    **/
    private NodeStates MOVE_AUTOMATIC_DEFFEND()
    {
        //return NodeStates.PENDING;        NOTE: for testing
        if (TACKLE_Node.nodeState==NodeStates.RUNNING)
        {
            return NodeStates.SUCCESS;
        }
        for (int j = 0; j < weights.Count; j++)
        {
            weights[wkeys[j]] = 0f;
        }

        weights["Avoidance"] = 3f;
        weights["StayInStadium"] = 1f;
        weights["Defend"] = 1f;
        //weights["FollowGoal"] = 2f;
        //weights["AvoidOpponents"] = 1f;
        if (CloseToball(4f) && ControlTimes["Tackle"] <= 0)
        {
            ControlStates["Animate"] = false;
            GetComponent<Animation>().Play("tackle");
            for (int j = 0; j < weights.Count; j++)
            {
                weights[wkeys[j]] = 0f;
            }
            weights["TackleMove"] = 1f;
            transform.forward= sphere.transform.position - transform.position;
            return NodeStates.SUCCESS;
        }
        else
        {
            ControlTimes["Tackle"] -= Time.deltaTime;
            return NodeStates.RUNNING;
        }
        
    }
    /**
    *@funtion TACKLE
    *@brief Accion asociada al intento de robo de balón
    *@return RUNNING si esta lejos de la cancha y tiene la bola, SUCCESS si llega al area contrincante, RUNNING si no tiene el balon
    **/
    private NodeStates TACKLE()
    {
        ControlStates["Animate"] = false;
        if (GetComponent<Animation>().IsPlaying("tackle") == true)
        {
            if ((sphere.transform.position - transform.position).magnitude<0.5f)
            {
                sphere.owner = this;
                ControlStates["Animate"] = true;
                return NodeStates.SUCCESS;
            }
            //Debug.Log((sphere.transform.position - transform.position).magnitude);
            return NodeStates.RUNNING;
        }
        ControlStates["Animate"] = true;
        //GetComponent<Animation>().Play("tackle");
        ControlTimes["Tackle"] = 5f;


        
        return NodeStates.FAILURE;
        
        
        
        
    }

    private NodeStates STOLE_BALL()
    {
        return NodeStates.FAILURE;
    }

    private NodeStates ONE_STEP_BACK()
    {
        return NodeStates.FAILURE;
    }

    

    
    private NodeStates PICK_BALL()
    {
        return NodeStates.FAILURE;
    }

    private NodeStates CHANGE_DIRECTION()
    {
        return NodeStates.FAILURE;
    }


    private NodeStates CORNER_KICK()
    {
        return NodeStates.FAILURE;
    }

    
    private NodeStates Medio_Tiempo()
    {
        return NodeStates.FAILURE;
    }

    private NodeStates Celebration()
    {
        return NodeStates.FAILURE;
    }
    /**
    *@funtion LOOK_FOR_BALL
    *@brief Nodo responsable de la busqueda del balón cuando el deuño del balón es nulo
    *@return RUNNING si el dueño actual sigue siendo nulo.
    **/
    private NodeStates LOOK_FOR_BALL()
    {
        //return NodeStates.PENDING;        Note: For testing
        if (sphere.owner!=null)
        {
            return NodeStates.FAILURE;
        }
        if (CloseToball(20f))
        {
            for (int j = 0; j < weights.Count; j++)
            {
                weights[wkeys[j]] = 0f;
            }
            weights["StayInStadium"] = 1f;
            weights["FollowBall"] = 1f;
            return NodeStates.RUNNING;
        }
        else
        {
            for (int j = 0; j < weights.Count; j++)
            {
                weights[wkeys[j]] = 0f;
            }
            weights["StayInStadium"] = 1f;
            weights["ReturnDefault"] = 1f;
        }
        return NodeStates.RUNNING;
    }

    public bool OnAreaCheck()
    {
        if (TeamName=="Local")
        {
            if (bFirstHalf)
            {
                if (transform.position.z >= 38)
                {
                    if (transform.position.x <=20 && transform.position.x >= -20)
                    {
                        return true;
                    }   
                }
            }
            else
            {
                if (transform.position.z <= -38)
                {
                    if (transform.position.x <= 20 && transform.position.x >= -20)
                    {
                        return true;
                    }
                }
            }
        }
        else
        {
            if (!bFirstHalf)
            {
                if (transform.position.z >= 38)
                {
                    if (transform.position.x <= 20 && transform.position.x >= -20)
                    {
                        return true;
                    }
                }
            }
            else
            {
                if (transform.position.z <= -38)
                {
                    if (transform.position.x <= 20 && transform.position.x >= -20)
                    {
                        return true;
                    }
                }
            }
        }
          

        return false;
    }
    private bool CloseToball(float distance)
    {
        if ((transform.position - sphere.transform.position).magnitude < distance)
        {
            return true;
        }
        return false;
    }

    int CheckDefInfront()
    {


        int count=0;
        foreach (SPlayer op in Opponents)
        {

            Vector3 relativePos = transform.InverseTransformPoint(op.transform.position);

            if (relativePos.z > 0.0f)
                count++ ;
        }

        return count;

    }
    bool CheckTeamInfront()
    {

        foreach (SPlayer op in Teammates)
        {
            if (op!=this)
            {
                Vector3 relativePos = transform.InverseTransformPoint(op.transform.position);

                if (relativePos.z > 0.0f)
                    return true;
            }
            
        }

        return false;

    }
    bool PassDecision()
    {
        if ((CheckTeamInfront() || CheckDefInfront() >= 7) && ControlTimes["Pass"] <= 0 && playerTeam.ControlTimes["Pass"] <= 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
