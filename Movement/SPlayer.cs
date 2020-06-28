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
    public enum SimulationType
    {
        Corner = 0,
        ThrowIn = 1,
        Oclusion = 2,
        Regular = 3,
    };
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
    public bool Walking = false;
    private const float STAMINA_DIVIDER = 64.0f;
    private const float STAMINA_MIN = 0.5f;
    private const float STAMINA_MAX = 1.0f;
    
    [Range(1f, 100f)]
    public float stamina = 64f;

    [Range(1f, 100f)]
    public float MAXSPEED = 5f;
    public float WSPEED = 3f;





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
    public ActionNode ThrowIn_Node;
    public ActionNode TACKLE_Node;
    public ActionNode Medio_Tiempo_Node;
    public ActionNode Celebration_Node;
    public ActionNode LOOK_FOR_BALL_Node;
    public ActionNode Oclusion_Node;


    //GPSEnable Variables
    //###############################################################################
    private bool GPSEnable;

    //###############################################################################

    //Variables de control
    public Dictionary<string, bool> ControlStates;
    public List<string> Ckeys;

    public Dictionary<string, float> ControlTimes;
    public List<string> Tkeys;


    // Start is called before the first frame update
    void Start()
    {
        GPSEnable = PlayerPrefs.GetInt("GPSEnable") == 1 ? true : false;

        inGame = FindObjectOfType<InGameState_Script>();
        center = GameObject.Find("Center_Field").GetComponent<Transform>();

        bFirstHalf = inGame.bFirstHalf;
        //Inicializacion de variables de comportamiento
        sbehaviors = new Sbehavior[6];

        cbehaviors = new Cbehavior[8];
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
        
        if (!GPSEnable)
        {
            
        } 
        

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
        cbehaviors[7] = ScriptableObject.CreateInstance("CornerSetup") as Cbehavior;
        weights.Add("CornerSetup", 0.1f);
        wkeys[sbehaviors.Length + 7] = "CornerSetup";





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
        ThrowIn_Node = new ActionNode(ThrowIn);        
        TACKLE_Node = new ActionNode(TACKLE);
        Medio_Tiempo_Node = new ActionNode(Medio_Tiempo);
        Celebration_Node = new ActionNode(Celebration);
        LOOK_FOR_BALL_Node= new ActionNode(LOOK_FOR_BALL);
        Oclusion_Node = new ActionNode(Oclusion);
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
            Oclusion_Node,
            FootballMatch,
            CORNER_KICK_Node,
            ThrowIn_Node,
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
        ControlTimes.Add("Reanimate", 0f);
        Tkeys.Add("Reanimate");
    }


    void Update()
    {

        if (!GPSEnable)
        {
            MovePlayer();
            if (TeamName == "Visit")  //Esta linea se utiliza para invertir ciertos valores dependiendo del equipo
            {
                bFirstHalf = !inGame.bFirstHalf;
            }
            else
            {
                bFirstHalf = inGame.bFirstHalf;
            }

            RootNode.Evaluate();
            //TestRoot.Evaluate();

            if (!ControlStates["Animate"]) //Esto reactiva la animación despues de un error
            {
                if (ControlTimes["Reanimate"] < 0f)
                {
                    ControlStates["Animate"] = true;
                }
                ControlTimes["Reanimate"] -= Time.deltaTime;
            }
            for (int i = 0; i < pweights.Length; i++) { pweights[i] = weights[wkeys[i]]; } //mostrar los pesos en el inspector

        }
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
            move *= WSPEED;
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
            //Debug.Log(ControlStates["Animate"]);
        }
        VelocityXZ = Vector3.Lerp(VelocityXZ, velocity, 0.5f); ;
        VelocityXZ.y = 0f;
        //if (MOVE_AUTOMATIC_DEFFEND_Node.nodeState == NodeStates.RUNNING)
        //{
        //    Debug.Log(VelocityXZ.magnitude, this);
        //}
        if (VelocityXZ.magnitude <= 2f)
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
                stamina -= 0.01f * Time.deltaTime;
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
            transform.forward = Vector3.Lerp(transform.forward, VelocityXZ, 0.5f);
            
                
            transform.rotation = Quaternion.Slerp(PRotation, transform.rotation, 0.1f);

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
        if (inGame.CurrEvent==InGameState_Script.FootEvent.Corner || inGame.CurrEvent == InGameState_Script.FootEvent.ThrowIn) //Fixme Esto se debe revisar
        {
            return NodeStates.SUCCESS;
        }
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
        foreach (SPlayer item in Teammates)
        {
            if (item.PASSING_Node.nodeState==NodeStates.RUNNING)
            {
                return CHECK_ATTACK_Node.nodeState;
            }
        }
        
      
        //Tarda 2 s en cambiar de modo ataque a modo defensa y vice versa
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
            ControlTimes["Reanimate"] = 4f;
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
                    if (go != this)
                    {
                        Vector3 relativePos = go.transform.position-transform.position;

                        if (!bFirstHalf)
                        {
                            relativePos.z = -relativePos.z;
                        }
                        float magnitude = relativePos.magnitude;
                        float direction = Mathf.Abs(relativePos.x);
                        

                        if (relativePos.z > 0.0f)
                            relativePos/=5f;       //Dar prioridad a los jugadores adelante del que tiene el balon
                        if (direction < 15.0f && (magnitude + direction < bestCandidateCoord))
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
                if (KICK_OFFER_Node.nodeState!=NodeStates.RUNNING)
                {
                    distanceBall = UnityEngine.Random.Range(distanceBall - 3f, distanceBall + 3f);
                    directionBall.x = UnityEngine.Random.Range(directionBall.x - 2f, directionBall.x + 2f);
                    directionBall.z = UnityEngine.Random.Range(directionBall.z - 2f, directionBall.z + 2f);
                }
                    
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
        ControlTimes["Reanimate"] = 4f;
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
            //Debug.Log("Pasando");
            PASSING();
            if(PASSING_Node.nodeState == NodeStates.SUCCESS) { return NodeStates.RUNNING; }
            else { return PASSING_Node.nodeState; }
        }
        ControlStates["Animate"] = true;
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
           
            if (OnAreaCheck() || CheckDefInfront()==0)
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
            weights["FollowOwner"] = 1f;
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
        //return NodeStates.PENDING;        //NOTE: for testing
        if (TACKLE_Node.nodeState==NodeStates.RUNNING)
        {
            return NodeStates.SUCCESS;
        }
        for (int j = 0; j < weights.Count; j++)
        {
            weights[wkeys[j]] = 0f;
        }

        weights["Avoidance"] = 1f;
        weights["StayInStadium"] = 1f;
        weights["Defend"] = 4f;
        //weights["FollowGoal"] = 2f;
        //weights["AvoidOpponents"] = 1f;
        if (CloseToball(4f) && ControlTimes["Tackle"] <= 0)
        {
            ControlStates["Animate"] = false;
            ControlTimes["Reanimate"] = 4f;
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
        ControlTimes["Reanimate"] = 4f;
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

    /**
    *@funtion CORNER_KICK
    *@brief Ejecución de Corner
    *@return 
    **/
    private NodeStates CORNER_KICK()
    {
        if (inGame.CurrEvent==InGameState_Script.FootEvent.Corner)
        {
            if (Mathf.Abs(sphere.transform.position.x)== 37 && inGame.TimeInAction<0) //El Corner se ha ajustado
            {
                if ((transform.position-sphere.transform.position).magnitude<5)
                {
                    Debug.Log("Realizando Tiro de esquina");
                    ControlStates["Animate"] = false;
                    ControlTimes["Reanimate"] = 4f;
                    GetComponent<Animation>().Play("shoot");
                }
                
                

                
                if (GetComponent<Animation>()["shoot"].normalizedTime > 0.3f)
                {
                    sphere.owner = null;
                    if (sphere.transform.position.z>0)
                    {
                        
                        float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                        sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);   //FIXME: Se Puede mejorar
                        //--------------------------------------------------------------------
                    }

                    else
                    {
                        
                        float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                        sphere.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);
                    }

                }

                return NodeStates.SUCCESS;
            }


            

            return NodeStates.RUNNING;
        }

        return NodeStates.FAILURE;
    }
    /**
    *@funtion ThrowIn
    *@brief Ejecución de Corner
    *@return 
    **/
    private NodeStates ThrowIn()
    {
        
        if (inGame.CurrEvent == InGameState_Script.FootEvent.ThrowIn)
        {
            if ( inGame.TimeInAction < 0.5) //El Corner se ha ajustado
            {
                Debug.Log("jdjdjdjd");
                if ((transform.position - sphere.transform.position).magnitude < 5 && ControlStates["Animate"])
                {
                    Debug.Log("Pase");
                    ControlStates["Animate"] = false;
                    ControlTimes["Reanimate"] = 2f;
                    GetComponent<Animation>().Play("saque_banda");
                }




                if (GetComponent<Animation>()["saque_banda"].normalizedTime > 0.1f)
                {
                    sphere.owner = null;



                    //sphere.inputPlayer = bestCandidatePlayer;
                    //El balon va hacia el candidato
                    Vector3 directionBall;
                    float distanceBall;
                    if (transform.position.x<0)
                    {
                        directionBall = (new Vector3(transform.position.x+5,1,transform.position.z-5) - transform.position).normalized;
                        distanceBall = (new Vector3(transform.position.x + 5, 1, transform.position.z - 5) - transform.position).magnitude * 1.4f;
                    }
                    else
                    {
                        directionBall = (new Vector3(transform.position.x - 5, 1, transform.position.z - 5) - transform.position).normalized;
                        distanceBall = (new Vector3(transform.position.x - 5, 1, transform.position.z - 5) - transform.position).magnitude * 1.4f;
                    }

                    
                    distanceBall = Mathf.Clamp(distanceBall, 15.0f, 40.0f);//Delimita el valor entre min y max
                       

                    sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x * distanceBall, distanceBall / 5.0f, directionBall.z * distanceBall);
                    sphere.owner = null;

                        
                    //if (sphere.transform.position.z > 0)
                    //{

                    //    float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                    //    sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);   //FIXME: Se Puede mejorar
                    //    //--------------------------------------------------------------------
                    //}

                    //else
                    //{

                    //    float valueRndY = UnityEngine.Random.Range(4.0f, 10.0f);
                    //    sphere.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x * 30.0f, valueRndY, transform.forward.z * 30.0f);
                    //}

                }
                inGame.CurrEvent = InGameState_Script.FootEvent.KickOff;
                return NodeStates.SUCCESS;
            }




            return NodeStates.RUNNING;
        }

        return NodeStates.FAILURE;
        
    }

    private NodeStates Medio_Tiempo()
    {
        return NodeStates.FAILURE;
    }

    /**
    *@funtion Oclusion
    *@brief Oclusion modo iddle
    *@return 
    **/
    private NodeStates Oclusion()
    {
        if (PlayerPrefs.GetInt("SimType", (int)SimulationType.Regular) == (int)SimulationType.Oclusion)
        {
            return NodeStates.RUNNING;
        }
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
        
          

        return false;
    }

    /**
    *@funtion CloseToball
    *@brief Revisa si el jugador esta lo suficientemente cerca del balon
    *@return True si se encuentra adentro del rango establecido
    *@param distance Distancia del rango 
    **/
    private bool CloseToball(float distance)
    {
        if ((transform.position - sphere.transform.position).magnitude < distance)
        {
            return true;
        }
        return false;
    }
    /**
    *@funtion CheckAround
    *@brief Revisa Cuantos jugadores hay adentro de un radio establecido
    *@return Cantidad de jugadores adentro del rango
    *@param Radius Radio del rango 
    **/
    public int CheckAround(float Radius)
    {
        int count = 0;
        foreach (SPlayer op in Opponents)
        {

            Vector3 relativePos = transform.InverseTransformPoint(op.transform.position);

            if (Mathf.Abs(relativePos.magnitude) < Radius)
                count++;
        }
        return count;
    }
    /**
    *@funtion CheckDefInfront
    *@brief Revisa Cuantos defensas hay adelante del jugador actual
    *@return Cantidad de jugadores 
    **/
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
    /**
    *@funtion CheckDefInfront
    *@brief Revisa si hay jugadores adelante del jugador actual
    *@return True si hay jugadores
    **/
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
        float decisionMaker = Random.Range(0, 100.0f);
        if ((CheckTeamInfront() || CheckDefInfront() >= 6) && ControlTimes["Pass"] <= 0 && playerTeam.ControlTimes["Pass"] <= 0 && decisionMaker<1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
}
