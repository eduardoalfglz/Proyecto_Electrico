using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine.SceneManagement;

public class InGameState_Script : MonoBehaviour
{
    public bool PlayButton;
    private float timeb4koff = 10.0f;


    public enum SimulationType
    {
        Corner=0,
        ThrowIn=1,
        Oclusion=2,
        Regular=3,
    };
    public enum TypePlayer
    {
        DEFENDER,
        MIDDLER,
        ATTACKER
    };

    public enum Destination
    {
        Initial=0,
        Default=1,
        Corner = 2,
    }

    public enum FootEvent
    {
        KickOff,
        LocalGoal,
        VisitGoal,
        ThrowIn,
        Corner,
        GoalKick,
        HalfTime
    }

    //Behavior Tree Nodes
    //###############################################################################
    public Sequence RootNode;
    //###############################################################################
    public Selector FootballMatch; 
    public Sequence KickOff_Sequence;
    public Sequence Corner_Sequence;
    public Sequence Throw_In_Sequence;
    public Selector Playing;
    public Sequence HalfTime_Secuence;
    public Sequence Goal_Sequence; //FIXME: en el futuro puede incluir celebracion



    //Action nodes Normal Sim
    //###############################################################################
    public ActionNode PLAYING_Node;
    public ActionNode PREPARE_TO_KICK_OFF_Node;
    public ActionNode KICK_OFF_Node;
    public ActionNode GOAL_Node;
    public ActionNode THROW_IN_Setup_Node;
    public ActionNode THROW_IN_DOING_Node;
    public ActionNode THROW_IN_DONE_Node;
    public ActionNode CORNER_Setup_Node;
    public ActionNode CORNER_DOING_Node;
    public ActionNode CORNER_DONE_Node;
    public ActionNode GOAL_KICK_Node;
    public ActionNode GOAL_KICK_RUNNING_Node;
    public ActionNode GOAL_KICK_KICKING_Node;
    public ActionNode Medio_Tiempo_Node;
    public ActionNode Cambio_Cancha_Node;
    public ActionNode FIN_Node;


    //Custom Sim Nodes
    //###############################################################################
    public Selector CustomSim;
    public Sequence CornerSim;
    public Sequence ThrowInSim;
    public Sequence OclusionSim;
    //###############################################################################
    public ActionNode CheckCornerSim_Node;
    public ActionNode CheckThrowInSim_Node;
    public ActionNode CheckOclusionSim_Node;
    public ActionNode PlayingNS_Node;
    public ActionNode Oclusion_Node;
    


    //###############################################################################
    List<SPlayer> Locals = new List<SPlayer>();
    STeam Local;
    
    List<SPlayer> Visitors = new List<SPlayer>();
    STeam Visit;
    private GameObject keeper;
    private GameObject keeper_oponent;
    
    public GameObject goalKeeperPrefab;
    //------------------
    //Varibles de control
    public int score_local = 0;
    public int score_visiting = 0;
    [HideInInspector]
    public bool bFirstHalf = true;
    


    public FootEvent CurrEvent = FootEvent.KickOff;
    
    //---------------------
    public delegate void MatchExecuted();
    public event MatchExecuted onMatchExecuted;

    //GPSEnable Variables
    //###############################################################################
    private bool GPSEnable;

    //###############################################################################

    //##########################################################################################
    //Variables
    //##########################################################################################
    private Transform center;
    private Sphere sphere;
    private ScorerTimeHUD scorerTime;

    [HideInInspector]
    public float TimeInAction;
    [HideInInspector]
    public float TimePlayingCS;






    // Use this for initialization
    void SetVariables()
    {



        //##########################Nodes############################
        PLAYING_Node=new ActionNode(PLAYING);
        PREPARE_TO_KICK_OFF_Node=new ActionNode(PREPARE_TO_KICK_OFF);
        KICK_OFF_Node=new ActionNode(KICK_OFF);
        GOAL_Node=new ActionNode(GOAL);
        THROW_IN_Setup_Node=new ActionNode(THROW_IN_Setup);
        THROW_IN_DOING_Node=new ActionNode(THROW_IN_DOING);
        THROW_IN_DONE_Node=new ActionNode(THROW_IN_DONE);
        CORNER_Setup_Node=new ActionNode(CORNER_Setup);
        CORNER_DOING_Node=new ActionNode(CORNER_DOING);        
        CORNER_DONE_Node=new ActionNode(CORNER_DONE);
        GOAL_KICK_Node=new ActionNode(GOAL_KICK);
        GOAL_KICK_RUNNING_Node=new ActionNode(GOAL_KICK_RUNNING);
        GOAL_KICK_KICKING_Node=new ActionNode(GOAL_KICK_KICKING);
        Medio_Tiempo_Node=new ActionNode(Medio_Tiempo);
        Cambio_Cancha_Node=new ActionNode(Cambio_Cancha);
        FIN_Node=new ActionNode(FIN);
        


        //##########################Custom_Nodes############################

        CheckCornerSim_Node = new ActionNode(CheckCornerSim);
        CheckThrowInSim_Node = new ActionNode(CheckThrowInSim);
        CheckOclusionSim_Node = new ActionNode(CheckOclusionSim);
        PlayingNS_Node= new ActionNode(PlayingNS);
        Oclusion_Node = new ActionNode(Oclusion);
        //---------------------------Sequence-----------------------
        KickOff_Sequence = new Sequence(new List<Node> {
            PREPARE_TO_KICK_OFF_Node,
            KICK_OFF_Node,
        });
        HalfTime_Secuence = new Sequence(new List<Node> {
            Medio_Tiempo_Node,
            Cambio_Cancha_Node,
        });


        Corner_Sequence = new Sequence(new List<Node> {

            CORNER_Setup_Node,
            CORNER_DOING_Node,
            CORNER_DONE_Node,
            
        });

        Throw_In_Sequence = new Sequence(new List<Node> {

            THROW_IN_Setup_Node,
            THROW_IN_DOING_Node,
            THROW_IN_DONE_Node
        });


        FootballMatch = new Selector(new List<Node> {
            
            PLAYING_Node,
            Corner_Sequence,
            Throw_In_Sequence,
            GOAL_KICK_Node,
            GOAL_Node,
        });

        RootNode = new Sequence(new List<Node> {
            KickOff_Sequence,
            FootballMatch,

            HalfTime_Secuence,
            
            
        });

        CornerSim = new Sequence(new List<Node>
        {
            CheckCornerSim_Node,
            Corner_Sequence,
            PlayingNS_Node,


        });
        ThrowInSim = new Sequence(new List<Node>
        {
            CheckThrowInSim_Node,
            Throw_In_Sequence,
            PlayingNS_Node,
        });
        OclusionSim = new Sequence(new List<Node>
        {
            CheckOclusionSim_Node,

        });


        CustomSim = new Selector( new List<Node>
        {
            CornerSim,
            ThrowInSim,
            OclusionSim,
            RootNode
        });
        

    }
    void Start()
    {


        //Debug.Log("Error de inicio");
        center = GameObject.Find("Center_Field").GetComponent<Transform>();
        sphere = GameObject.Find("soccer_ball").GetComponent<Sphere>();



        //Buscar tiempo

        scorerTime = GameObject.Find("UI").GetComponentInChildren<ScorerTimeHUD>();

        Local = GameObject.Find("Local").GetComponent<STeam>();
        Visit = GameObject.Find("Visit").GetComponent<STeam>();

        // search PLayers, opponents and goalkeepers
        keeper = GameObject.FindGameObjectWithTag("GoalKeeper");
        keeper_oponent = GameObject.FindGameObjectWithTag("GoalKeeper_Oponent");


        Locals = Local.Locals;
        Visitors = Visit.Visitors;


        GPSEnable = PlayerPrefs.GetInt("GPSEnable") == 1 ? true : false;
        //Set initial objects
        if (!GPSEnable)
        {
            SetVariables();
        } else
        {
            //Aqui se lee el archivo por primera vez
        }
        
        





    }





    // Update is called once per frame
    void Update()
    {
        //

        //Debug.Log(CurrEvent);
        if (!GPSEnable)
        {
            Evaluate();
        } else
        {
            //Debug.Log(GPSEnable);
        }
            
    }
    /**
    *@funtion Evaluate
    *@brief Evalua el nodo principal y ejecuta la rutina principal del partido
    **/
    public void Evaluate()
    {
        CustomSim.Evaluate();
        //StartCoroutine(Execute());
    }

    /**
    *@funtion Execute
    *@brief Muestra mensajes en consola del estado del partido
    **/
    private IEnumerator Execute()
    {
        
        yield return new WaitForSeconds(0.5f);

        //if (KickOff_Sequence.nodeState == NodeStates.SUCCESS)
        //{
        //    Debug.Log("Ya estan en la posicion inicial");
            
        //}
        if (onMatchExecuted != null)
        {
            onMatchExecuted();
        }

    }











    //NODOS
    //############################################################################################################################################
    //############################################################################################################################################
    //############################################################################################################################################
    //############################################################################################################################################
    //############################################################################################################################################

    /**
    *@funtion PLAYING
    *@brief Revisa las condiciones de tiempo para el partido
    *@return Retorna exito en caso de que termine el primer tiempo o el segundo.
    **/
    public NodeStates PLAYING()
    {
        //Debug.Log("Jugando");
        if (scorerTime.minutes <= PlayerPrefs.GetInt("PeriodTime",45)-1 && bFirstHalf)
        {
            if (sphere.OutofBounds)
            {
                return NodeStates.FAILURE;  //Checks corner or throw in
            }
            return NodeStates.RUNNING;

        }else if (scorerTime.minutes== PlayerPrefs.GetInt("PeriodTime", 45)-1 && !bFirstHalf)
        {
            return NodeStates.FAILURE;
        }
        // Se realiza cambio de media cancha.
        if (scorerTime.minutes == PlayerPrefs.GetInt("PeriodTime", 45) && bFirstHalf)
        {
            bFirstHalf = false;
            return NodeStates.SUCCESS;

        }

        if (scorerTime.minutes >= PlayerPrefs.GetInt("PeriodTime", 45) && !bFirstHalf)
        {
            if (sphere.OutofBounds)
            {
                return NodeStates.FAILURE;  //Checks corner or throw in
            }
            return NodeStates.RUNNING;

        }
        // Finaliza el juego
        if (scorerTime.minutes == 90 && !bFirstHalf)
        {
            bFirstHalf = true;
            Debug.Log("End of simulation");
            return NodeStates.SUCCESS;
            
        }

        return NodeStates.FAILURE;
    }
    /**
    *@funtion PREPARE_TO_KICK_OFF
    *@brief Mueve a los jugadores a la posicion inicial
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    public NodeStates PREPARE_TO_KICK_OFF()
    {
        if (CurrEvent != FootEvent.KickOff && CurrEvent != FootEvent.HalfTime)//Esto debido a que es el primer nodo a evaluar
        {
            return NodeStates.SUCCESS;
        }
        if (PREPARE_TO_KICK_OFF_Node.nodeState == NodeStates.PENDING) { scorerTime.GState = ScorerTimeHUD.GameState.KickOff; }
        
        //Este codigo es necesario para que una vez que el nodo tenga exito deje de ejecutarse
        if (CurrEvent == FootEvent.KickOff && PREPARE_TO_KICK_OFF_Node.nodeState==NodeStates.SUCCESS)
        {
            return NodeStates.SUCCESS;
        }
        if (CurrEvent == FootEvent.HalfTime && Cambio_Cancha_Node.nodeState == NodeStates.SUCCESS)
        {
            
            return NodeStates.SUCCESS;
        }
        if ((CurrEvent == FootEvent.LocalGoal || CurrEvent == FootEvent.VisitGoal) && GOAL_Node.nodeState == NodeStates.SUCCESS)
        {

            return NodeStates.SUCCESS;
        }


        int wLenght = Locals[0].sbehaviors.Length + Locals[0].cbehaviors.Length;

        //
        for (int i=0;i<10; i++ )
        {
            for (int j = 0; j < wLenght; j++)
            {
                Locals[i].weights[Locals[i].wkeys[j]] = 0f;
            }
            
            Locals[i].weights["ReturnInit"]=1;
            
        }

        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < wLenght; j++)
            {
                Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
            }
            
            Visitors[i].weights["ReturnInit"] = 1;
        }

        if (Local.checkDestination((int)Destination.Initial) && Visit.checkDestination((int)Destination.Initial))//Nota, es necesario hacerlo para ambos ya que cada uno tiene su equipo
        {
            //Debug.Log("Posiciones iniciales");
            
            if (bFirstHalf && CurrEvent == FootEvent.LocalGoal)
            {
                Vector3[] temp = Local.local_init_position;
                Local.local_init_position = Visit.visit_init_position;
                Visit.visit_init_position = temp;
                for (int i = 0; i < 10; i++)
                {
                    Local.local_init_position[i].z = -Local.local_init_position[i].z;
                    Visit.visit_init_position[i].z = -Visit.visit_init_position[i].z;
                }
                
            }
            if (!bFirstHalf && CurrEvent==FootEvent.VisitGoal)
            {
                Vector3[] temp = Local.local_init_position;
                Local.local_init_position = Visit.visit_init_position;
                Visit.visit_init_position = temp;
                for (int i = 0; i < 10; i++)
                {
                    Local.local_init_position[i].z = -Local.local_init_position[i].z;
                    Visit.visit_init_position[i].z = -Visit.visit_init_position[i].z;
                }
                
            }
            
            return NodeStates.SUCCESS;
        }
        return NodeStates.RUNNING;
    }

    /**
    *@funtion KICK_OFF
    *@brief Comprueba que el balon este en el centro, inicia el tiempo y fuerza el primer pase
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    private NodeStates KICK_OFF()
    {
        if (CurrEvent != FootEvent.KickOff && CurrEvent != FootEvent.HalfTime)//Esto debido a que es el primer nodo a evaluar
        {
            return NodeStates.SUCCESS;
        }
        if (KICK_OFF_Node.nodeState == NodeStates.SUCCESS && (CurrEvent == FootEvent.KickOff || CurrEvent==FootEvent.HalfTime))
        {
            return NodeStates.SUCCESS;
        }

        PlayButton= Input.GetKey(KeyCode.Space);

        timeb4koff -= Time.deltaTime;

        Locals[9].ControlStates["Kick_Offer"] = true;
        if (PlayButton)
        {
            //Debug.Log("SE PRECIONO LA TECLA ESPACIO");


            scorerTime.GState = ScorerTimeHUD.GameState.Playing;
            //Debug.Log("Patada inicial");
            if (bFirstHalf) { CurrEvent = FootEvent.KickOff; } //Esto elimina el hecho de que el evento actual sea un goal;
            else { CurrEvent = FootEvent.HalfTime; }           //Locks down the events
            return NodeStates.SUCCESS;
            

        }
        if (timeb4koff < 0)
        {
            scorerTime.GState = ScorerTimeHUD.GameState.Playing;
            Debug.Log("Patada inicial");
            timeb4koff = 5f; //Una vez que se logra se esperan 5 s para el siguiente evento
            if (bFirstHalf) { CurrEvent = FootEvent.KickOff; } //Esto elimina el hecho de que el evento actual sea un goal;
            else { CurrEvent = FootEvent.HalfTime; }           //Locks down the events
            return NodeStates.SUCCESS;
            



        }

        
        //sphere.owner = null;
        sphere.gameObject.transform.position = center.position;
        sphere.gameObject.GetComponent<Rigidbody>().drag = 0.5f;
        
        return NodeStates.RUNNING;
    }

    /**
    *@funtion GOAL
    *@brief Aumenta los marcadores cambia las posiciones de inicio y llama a prepare_kick_off
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    private NodeStates GOAL()//Fixme: esto no ha sido probado
    {
        scorerTime.GState = ScorerTimeHUD.GameState.Stopped;
        if (CurrEvent!=FootEvent.LocalGoal && CurrEvent!=FootEvent.VisitGoal)
        {
            return NodeStates.FAILURE;
        }
        if (CurrEvent == FootEvent.LocalGoal)
        {
            if (GOAL_Node.nodeState!=NodeStates.RUNNING)//Fixme: REvisar
            {
                score_local++;
            }
            
            
            if (bFirstHalf)
            {
                Vector3[] temp = Local.local_init_position;
                Local.local_init_position = Visit.visit_init_position;
                Visit.visit_init_position = temp;
                for (int i = 0; i < 10; i++)
                {
                    Local.local_init_position[i].z = -Local.local_init_position[i].z;
                    Visit.visit_init_position[i].z = -Visit.visit_init_position[i].z;
                }
                
            }
            
            return PREPARE_TO_KICK_OFF();
        }
        else if (CurrEvent == FootEvent.VisitGoal)
        {
            if (GOAL_Node.nodeState != NodeStates.RUNNING)//Fixme: REvisar
            {
                score_visiting++;
            }
            
            if (!bFirstHalf)
            {
                Vector3[] temp = Local.local_init_position;
                Local.local_init_position = Visit.visit_init_position;
                Visit.visit_init_position = temp;
                for (int i = 0; i < 10; i++)
                {
                    Local.local_init_position[i].z = -Local.local_init_position[i].z;
                    Visit.visit_init_position[i].z = -Visit.visit_init_position[i].z;
                }
            }
            
            return PREPARE_TO_KICK_OFF();
        }
        return NodeStates.FAILURE;
    }




    /**
    *@funtion THROW_IN_Setup
    *@brief throw in, ajusta las posiciones iniciales
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    private NodeStates THROW_IN_Setup()
    {
        if (CurrEvent != FootEvent.ThrowIn)
        {
            return NodeStates.FAILURE;
        }

        if (CurrEvent == FootEvent.ThrowIn && THROW_IN_Setup_Node.nodeState == NodeStates.SUCCESS)
        {
            return NodeStates.SUCCESS;
        }
        if (sphere.OutPosition.x < 0)
        {
            sphere.transform.position = new Vector3(-30.6f, 1, 30);
        }
        else
        {
            sphere.transform.position = new Vector3(30.6f, 1, 30);
        }

        sphere.OutPosition = new Vector3(38, 100, 55);
        TimeInAction = 4f;
        TimePlayingCS = 14f;
       
        return NodeStates.SUCCESS;
        //if (CurrEvent!=FootEvent.ThrowIn)
        //{
        //    return NodeStates.FAILURE;
        //}

        //if (CurrEvent == FootEvent.ThrowIn && THROW_IN_Setup_Node.nodeState == NodeStates.SUCCESS)
        //{
        //    return NodeStates.SUCCESS;
        //}
        //int wLenght = Locals[0].sbehaviors.Length + Locals[0].cbehaviors.Length;

        ////
        //for (int i = 0; i < 10; i++)
        //{
        //    for (int j = 0; j < wLenght; j++)
        //    {
        //        Locals[i].weights[Locals[i].wkeys[j]] = 0f;
        //        Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
        //    }

        //    Locals[i].weights["ReturnDefault"] = 1;
        //    Visitors[i].weights["ReturnDefault"] = 1;

        //}

        //if (Local.checkDestination((int)Destination.Default) || Visit.checkDestination((int)Destination.Default))//Nota, es necesario hacerlo para ambos ya que cada uno tiene su equipo
        //{
        //    Debug.Log("test");
        //    if (sphere.LastTouch.TeamName == "Local")
        //    {
        //        if ((sphere.OutPosition.z < 0 && bFirstHalf) || (sphere.OutPosition.z > 0 && !bFirstHalf))
        //        {
        //            Visitors[8].transform.position =new Vector3(sphere.OutPosition.x,0,sphere.OutPosition.z);

        //            if (sphere.OutPosition.x<0)
        //            {
        //                Visitors[9].transform.position = new Vector3(sphere.OutPosition.x+5, 0, sphere.OutPosition.z+5);
        //                Visitors[7].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z - 5);
        //            }
        //            else
        //            {
        //                Visitors[9].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z + 5);
        //                Visitors[7].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z - 5);
        //            }
        //        }
        //        else
        //        {
        //            Visitors[5].transform.position = new Vector3(sphere.OutPosition.x, 0, sphere.OutPosition.z);

        //            if (sphere.OutPosition.x < 0)
        //            {
        //                Visitors[6].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z + 5);
        //                Visitors[4].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z - 5);
        //            }
        //            else
        //            {
        //                Visitors[6].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z + 5);
        //                Visitors[4].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z - 5);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if ((sphere.OutPosition.z < 0 && bFirstHalf) || (sphere.OutPosition.z > 0 && !bFirstHalf))
        //        {


        //            if (sphere.OutPosition.x < 0)
        //            {
        //                Locals[8].transform.position = new Vector3(sphere.OutPosition.x - 1, 0, sphere.OutPosition.z);
        //                Locals[9].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z + 5);
        //                Locals[7].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z - 5);
        //            }
        //            else
        //            {
        //                Locals[9].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z + 5);
        //                Locals[7].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z - 5);
        //                Locals[8].transform.position = new Vector3(sphere.OutPosition.x + 1, 0, sphere.OutPosition.z);
        //            }
        //        }
        //        else
        //        {


        //            if (sphere.OutPosition.x < 0)
        //            {
        //                Locals[5].transform.position = new Vector3(sphere.OutPosition.x - 1, 0, sphere.OutPosition.z);
        //                Locals[6].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z + 5);
        //                Locals[4].transform.position = new Vector3(sphere.OutPosition.x + 5, 0, sphere.OutPosition.z - 5);
        //            }
        //            else
        //            {
        //                Locals[5].transform.position = new Vector3(sphere.OutPosition.x + 1, 0, sphere.OutPosition.z);
        //                Locals[6].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z + 5);
        //                Locals[4].transform.position = new Vector3(sphere.OutPosition.x - 5, 0, sphere.OutPosition.z - 5);
        //            }
        //        }
        //    }
        //    if (sphere.OutPosition.x<0)
        //    {
        //        sphere.transform.position = new Vector3(-37.6f, 1, sphere.OutPosition.z);
        //    }
        //    else
        //    {
        //        sphere.transform.position = new Vector3(37.6f, 1, sphere.OutPosition.z);
        //    }

        //    sphere.OutPosition = new Vector3(38, 100, 55);
        //    TimeInAction = 4f;
        //    TimePlayingCS = 14f;
        //    for (int i = 0; i < 10; i++)
        //    {
        //        for (int j = 0; j < wLenght; j++)
        //        {
        //            Locals[i].weights[Locals[i].wkeys[j]] = 0f;
        //            Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
        //        }
        //    }
        //    return NodeStates.SUCCESS;
        //}
        //return NodeStates.RUNNING;


    }    
    private NodeStates THROW_IN_DOING()
    {
        
        //if (TimeInAction > 0)
        //{
        //    Debug.Log("Throw In Set");
        //    TimeInAction -= Time.deltaTime;
        //    return NodeStates.RUNNING;
        //}
        
        return NodeStates.SUCCESS;
        
    }
    private NodeStates THROW_IN_DONE()
    {
        
        if (bFirstHalf) { CurrEvent = FootEvent.KickOff; } //Esto elimina el hecho de que el evento actual sea un saque;
        else { CurrEvent = FootEvent.HalfTime; }           //Locks down the events
        return NodeStates.RUNNING;
    }
    /**
    *@funtion CORNER_Setup
    *@brief Verifica si es un corner o un throw in, en caso de corner ajusta las posiciones iniciales
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    private NodeStates CORNER_Setup()
    {
        //Check if it is a corner
        
        if (Mathf.Abs(sphere.OutPosition.z)<54.5f)
        {
            CurrEvent = FootEvent.ThrowIn;
            return NodeStates.FAILURE;
        }
        if (bFirstHalf)
        {
            if (sphere.OutPosition.z>0 )
            {
                if (Mathf.Abs(sphere.OutPosition.x)<3.1 && Mathf.Abs(sphere.OutPosition.y) < 3.2)
                {
                    CurrEvent = FootEvent.LocalGoal;
                    return NodeStates.FAILURE;
                }
                if (sphere.LastTouch.TeamName == "Local")
                {
                    CurrEvent = FootEvent.GoalKick;
                    return NodeStates.FAILURE;
                }

            }
            else
            {
                if (Mathf.Abs(sphere.OutPosition.x) < 3.1 && Mathf.Abs(sphere.OutPosition.y) < 3.2)
                {
                    CurrEvent = FootEvent.VisitGoal;
                    return NodeStates.FAILURE;
                }
                if (sphere.LastTouch.TeamName == "Visit")
                {
                    CurrEvent = FootEvent.GoalKick;
                    return NodeStates.FAILURE;
                }
            }
        }
        else
        {
            if (sphere.OutPosition.z < 0 )
            {
                if (Mathf.Abs(sphere.OutPosition.x) < 3.1 && Mathf.Abs(sphere.OutPosition.y) < 3.2)
                {
                    CurrEvent = FootEvent.VisitGoal;
                    return NodeStates.FAILURE;
                }
                if (sphere.LastTouch.TeamName == "Visit")
                {
                    CurrEvent = FootEvent.GoalKick;
                    return NodeStates.FAILURE;
                }

            }
            else
            {
                if (Mathf.Abs(sphere.OutPosition.x) < 3.1 && Mathf.Abs(sphere.OutPosition.y) < 3.2)
                {
                    CurrEvent = FootEvent.LocalGoal;
                    return NodeStates.FAILURE;
                }
                if (sphere.LastTouch.TeamName == "Local")
                {
                    CurrEvent = FootEvent.GoalKick;
                    return NodeStates.FAILURE;
                }
            }
        }
        if (CurrEvent == FootEvent.Corner && CORNER_Setup_Node.nodeState == NodeStates.SUCCESS)
        {
            return NodeStates.SUCCESS;
        }
        


        int wLenght = Locals[0].sbehaviors.Length + Locals[0].cbehaviors.Length;

        //
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < wLenght; j++)
            {
                Locals[i].weights[Locals[i].wkeys[j]] = 0f;
                Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
            }

            Locals[i].weights["CornerSetup"] = 1;
            Visitors[i].weights["CornerSetup"] = 1;
        }

        

        if (Local.checkDestination((int)Destination.Corner) && Visit.checkDestination((int)Destination.Corner))//Nota, es necesario hacerlo para ambos ya que cada uno tiene su equipo
        {


            Debug.Log("Corner set");
            if (sphere.OutPosition.z<0)
            {
                if (sphere.OutPosition.x < 0)
                {
                    sphere.transform.position = new Vector3(-37, 0.1f, -55);
                }
                else
                {
                    sphere.transform.position = new Vector3(37, 0.1f, -55);
                }
            }
            else
            {
                if (sphere.OutPosition.x < 0)
                {
                    sphere.transform.position = new Vector3(-37, 0.1f, 55);
                }
                else
                {
                    sphere.transform.position = new Vector3(37, 0.1f, 55);
                }
            }
            sphere.OutPosition=new Vector3(38, 100, 55);
            TimeInAction = 4f;
            TimePlayingCS = 14f;
            return NodeStates.SUCCESS;
        }
        return NodeStates.RUNNING;
        
    }


    /**
    *@funtion CORNER_Setup
    *@brief Verifica si es un corner o un throw in, en caso de corner ajusta las posiciones iniciales
    *@return Retorna exito cuado los jugadores estan cerca de la posicion inicial.
    **/
    private NodeStates CORNER_DOING()
    {

        if (TimeInAction>0)
        {
            TimeInAction -= Time.deltaTime;
            return NodeStates.RUNNING;
        }

        return NodeStates.SUCCESS;
    }
    

    private NodeStates CORNER_DONE()
    {
        if (bFirstHalf) { CurrEvent = FootEvent.KickOff; } //Esto elimina el hecho de que el evento actual sea un goal;
        else { CurrEvent = FootEvent.HalfTime; }           //Locks down the events
        return NodeStates.SUCCESS;
    }
    /**
    *@funtion GOAL_KICK
    *@brief Saque de puerta
    *@return 
    **/
    private NodeStates GOAL_KICK()
    {
        if (CurrEvent == FootEvent.LocalGoal || CurrEvent == FootEvent.VisitGoal)
        {            
            return NodeStates.FAILURE;
        }
        int wLenght = Locals[0].sbehaviors.Length + Locals[0].cbehaviors.Length;

        //
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < wLenght; j++)
            {
                Locals[i].weights[Locals[i].wkeys[j]] = 0f;
                Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
            }

            Locals[i].weights["ReturnDefault"] = 1;
            Visitors[i].weights["ReturnDefault"] = 1;
        }



        if (Local.checkDestination((int)Destination.Default) && Visit.checkDestination((int)Destination.Default))//Nota, es necesario hacerlo para ambos ya que cada uno tiene su equipo
        {


            Debug.Log("KickOff set");
            if (sphere.OutPosition.z < 0)
            {
                sphere.transform.position = new Vector3(0,0,-45);
                //Configurar portero
            }
            else
            {
                sphere.transform.position = new Vector3(0, 0, 45);
            }

            sphere.OutPosition = new Vector3(38, 100, 55);
            if (bFirstHalf) { CurrEvent = FootEvent.KickOff; } //Esto elimina el hecho de que el evento actual sea un goal;
            else { CurrEvent = FootEvent.HalfTime; }           //Locks down the events
            return NodeStates.RUNNING;
        }
        return NodeStates.RUNNING;


    }

    private NodeStates GOAL_KICK_RUNNING()
    {
        return NodeStates.FAILURE;
    }

    private NodeStates GOAL_KICK_KICKING()
    {
        return NodeStates.FAILURE;
    }


    private NodeStates Medio_Tiempo()
    {
        
        Debug.Log("Halftime");
        scorerTime.GState = ScorerTimeHUD.GameState.HalfTime;
        return NodeStates.SUCCESS;
        
       
    }

    private NodeStates Cambio_Cancha()
    {
        //Este codigo es necesario para que una vez que el nodo tenga exito deje de ejecutarse
        if (Cambio_Cancha_Node.nodeState == NodeStates.SUCCESS)
        {
            return NodeStates.SUCCESS;
        }
        //Esto se ejecuta una unica vez
        if (Cambio_Cancha_Node.nodeState == NodeStates.PENDING)
        {
            Vector3[] temp = Local.local_init_position;
            Local.local_init_position = Visit.visit_init_position;
            Visit.visit_init_position = temp;
        }

        CurrEvent = FootEvent.HalfTime;
        return PREPARE_TO_KICK_OFF();
    }



    private NodeStates FIN()
    {
        return NodeStates.FAILURE;
    }
    /**
    *@funtion CheckCornerSim
    *@brief Verifica si es simulacion de corner
    *@return 
    **/
    private NodeStates CheckCornerSim()
    {
        
        if (PlayerPrefs.GetInt("SimType",(int)SimulationType.Regular)==(int)SimulationType.Corner)
        {

            if (CheckCornerSim_Node.nodeState==NodeStates.PENDING)
            {
                sphere.OutPosition = new Vector3(30, 0.2f, 56);
                sphere.LastTouch = Visitors[0];
                CurrEvent = FootEvent.Corner;
            }
            
            return NodeStates.SUCCESS;
        }


        return NodeStates.FAILURE;
    }
    /**
    *@funtion CheckThrowInSim
    *@brief Verifica si es simulacion de Saque de banda, este codigo debe volver a escribirse por la forma en la que está definido action node
    *@return 
    **/
    private NodeStates CheckThrowInSim()
    {
        if (PlayerPrefs.GetInt("SimType", (int)SimulationType.Regular) == (int)SimulationType.ThrowIn)
        {
            if (CheckThrowInSim_Node.nodeState == NodeStates.PENDING)
            {
                sphere.OutPosition = new Vector3(38, 0.2f, 20);
                sphere.LastTouch = Visitors[0];
                CurrEvent = FootEvent.ThrowIn;
            }
            return NodeStates.SUCCESS;
        }


        return NodeStates.FAILURE;
    }
    /**
    *@funtion CheckOclusionSim
    *@brief Verifica si es simulacion de oclusion, este codigo debe volver a escribirse por la forma en la que está definido action node
    *@return 
    **/
    private NodeStates CheckOclusionSim()
    {
        if (PlayerPrefs.GetInt("SimType", (int)SimulationType.Regular) == (int)SimulationType.Oclusion)
        {
            return NodeStates.SUCCESS;
        }


        return NodeStates.FAILURE;
    }    
    /**
    *@funtion CheckOclusionSim
    *@brief Verifica si es simulacion de oclusion, este codigo debe volver a escribirse por la forma en la que está definido action node
    *@return 
    **/
    private NodeStates Oclusion()
    {
        int wLenght = Locals[0].sbehaviors.Length + Locals[0].cbehaviors.Length;

        //
        for (int i = 0; i < 10; i++)
        {
            for (int j = 0; j < wLenght; j++)
            {
                Locals[i].weights[Locals[i].wkeys[j]] = 0f;
                Visitors[i].weights[Visitors[i].wkeys[j]] = 0f;
            }

            Locals[i].weights["Avoidance"] = 1.5f;
            Locals[i].weights["Alignment"] = 2;
            Locals[i].weights["Cohesion"] = 1;
            
            Visitors[i].weights["Avoidance"] = 1.5f;
            Visitors[i].weights["Alignment"] = 2;
            Visitors[i].weights["Cohesion"] = 1;
        }
        return NodeStates.RUNNING;
    }
    /**
    *@funtion CheckOclusionSim
    *@brief Verifica si es simulacion de oclusion, este codigo debe volver a escribirse por la forma en la que está definido action node
    *@return 
    **/
    private NodeStates PlayingNS()
    {
        if (TimePlayingCS>0)
        {
            CurrEvent = FootEvent.KickOff;
            TimePlayingCS -= Time.deltaTime;
            return PLAYING();
        }
        if (PlayerPrefs.GetInt("SimType", (int)SimulationType.Regular) == (int)SimulationType.Corner)
        {
            CurrEvent = FootEvent.Corner;
            sphere.OutPosition = new Vector3(15, 0.2f, 56);
        }
        if (PlayerPrefs.GetInt("SimType", (int)SimulationType.Regular) == (int)SimulationType.ThrowIn)
        {
            CurrEvent = FootEvent.ThrowIn;
            sphere.OutPosition = new Vector3(-38, 0.2f, 0.5f);
        }

            
        sphere.LastTouch = Visitors[0];
        return NodeStates.SUCCESS;
    }
}
