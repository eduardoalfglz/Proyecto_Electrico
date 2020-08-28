using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;  //Para trabajar con archivos


//Obsoleto
public class Player_Script : MonoBehaviour {

	
	
	public TypePlayer type = TypePlayer.DEFENDER;
	public float Speed = 1.0f;	
	public float Strong = 1.0f;
	public float Control = 1.0f;
		
	//capacidad de resistencia física
	private const float STAMINA_DIVIDER = 64.0f;
	private const float STAMINA_MIN = 0.5f;	
	private const float STAMINA_MAX = 1.0f;	
		
		
	public enum TypePlayer {
			DEFENDER,
			MIDDLER,
			ATTACKER
		};
		
	public Vector3 actualVelocityPlayer;
	private Vector3 oldVelocityPlayer;
	public Sphere sphere;
	public GameObject[] players;
    public GameObject[] opponents;
	
	public Vector3 initialPosition;
	private Vector3 initialPosition1;
	private float inputSteer; //Direccion de entrada
	private const float initialDisplacement = 20.0f;	
	//public Transform goalPosition;
	public Vector3 goalPosition;
	public Transform headTransform;	
	[HideInInspector]	
	public bool temporallyUnselectable = true;
	[HideInInspector]	
	public float timeToBeSelectable = 1.0f;	
	public float maxDistanceFromPosition = 30.0f;	
		
	public enum Player_State { 
		   PREPARE_TO_KICK_OFF,
		   KICK_OFFER,
		   RESTING,
		   GO_ORIGIN,
		   CONTROLLING,
		   PASSING,
		   SHOOTING,
		   MOVE_AUTOMATIC,
		   ONE_STEP_BACK,
		   STOLE_BALL,
		   OPONENT_ATTACK,
		   PICK_BALL,
		   CHANGE_DIRECTION,
		   THROW_IN,
		   CORNER_KICK,
		   TACKLE,
		   Medio_Tiempo,
		   DESPLAZAR,
		   DESPLAZAR1
		  };
	   
	
	
	
	public Player_State state;
	private float timeToRemove = 3.0f;	
	private float timeToPass = 1.0f;
		
	// hand of player in squeleton hierarchy
	public Transform hand_bone;
		
	//private InGameState_Script inGame; FIXME: Cambiado temporalmente, 19-10-19
		
	public Texture barTexture;
	public Texture barStaminaTexture;
	//private int barPosition=0;
	private Quaternion initialRotation;	
		
	public float stamina = 64.0f;	

	public bool bPassButton;
	public float posiciones;

	//------------------AGREGO--------------------------------
		
	public string id;
	public Vector3 posicionFinal;
	public int valor_state=0; //para saber cual es el estado al que debe de seguir en InGameState_Script
		
	public bool mediaCancha1=false; //JUGADOR MAS CERCA DE MEDIA CANCHA ROJO
	public bool mediaCancha2=false; //SEGUNDO JUGADOR MAS CERCA DE MEDIA CANCHA ROJO

	public bool PosFinalMasLarga=false;	
	//public int jugadorMasLargo=-1; //Es para saber cual es el jugador mas largo y va desde 0-9

	public List<string> listPosicion;

	private ScorerTimeHUD scorerTime;

	private GameObject keeper;
	private GameObject keeper_oponent;




    //##########################EDVariables
    private float timeb4koff=10.0f;
	//--------------------------------------------------------

	void  Awake () {

		GetComponent<Animation>().Stop();
		state = Player_State.PREPARE_TO_KICK_OFF; 
	}



	void  Start (){
        //Debug.Log(GetComponentInChildren<SkinnedMeshRenderer>().material);
        scorerTime = GameObject.FindObjectOfType(typeof(ScorerTimeHUD)) as ScorerTimeHUD;
        //Cambiar los uniformes
        GetComponentInChildren<SkinnedMeshRenderer>().material = Resources.Load("Materials/" + "player_texture_"+PlayerPrefs.GetString(transform.parent.name)) as Material;
       
        keeper = GameObject.FindGameObjectWithTag("GoalKeeper");
		keeper_oponent = GameObject.FindGameObjectWithTag("GoalKeeper_Oponent");
		listPosicion= new List<string>();
        //inGame = GameObject.FindObjectOfType(typeof(InGameState_Script)) as InGameState_Script;
        sphere = GameObject.Find("soccer_ball").GetComponent<Sphere>();
        // get players and opponents and save it in both arrays

        players = GameObject.FindGameObjectsWithTag("PlayerTeam1");
		opponents = GameObject.FindGameObjectsWithTag("OpponentTeam");
	
		
		

			

		if ( gameObject.tag == "PlayerTeam1")
        {
			initialPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z+initialDisplacement );
        }
        if (gameObject.tag == "OpponentTeam")
        {
            initialPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - initialDisplacement);
        }


        // set animations speed to fit perfect movements		
        GetComponent<Animation>()["jump_backwards_bucle"].speed = 1.5f;
		GetComponent<Animation>()["starting"].speed = 1.0f;
		GetComponent<Animation>()["starting_ball"].speed = 1.0f;
		GetComponent<Animation>()["running"].speed = 1.2f;
		GetComponent<Animation>()["running_ball"].speed = 1.0f;
		GetComponent<Animation>()["pass"].speed = 1.8f;
		GetComponent<Animation>()["rest"].speed = 1.0f;
		GetComponent<Animation>()["turn"].speed = 1.3f;
		GetComponent<Animation>()["tackle"].speed = 2.0f;

		GetComponent<Animation>()["fight"].speed = 1.2f;	
		// para el movimiento de la cabeza de los jugadores
			
		GetComponent<Animation>().Play("rest");	
			
		initialRotation = transform.rotation * headTransform.rotation;
	}

	
	// ask if someone is in front of me
	// Pregunta si alguno esta enfrente
	bool NoOneInFront( GameObject[] team_players ) {
		
			
		foreach( GameObject go in team_players ) {

			Vector3 relativePos = transform.InverseTransformPoint( go.transform.position ); 
			
			if ( relativePos.z > 0.0f )
				return true;		
		}
			
		return false;
			
	}
	
	
	void LateUpdate() {
        //Se puede utilizar para voltear la cabeza del jugador
				
	}
	
	void  Update() {

        		
		
		switch ( state ) {
			


			case Player_State.PREPARE_TO_KICK_OFF:
                
                //Debug.Log("####");
			break;


			case Player_State.KICK_OFFER:
                

                break;

			case Player_State.THROW_IN:
				
			break;

			case Player_State.CORNER_KICK:
				
			break;
				
			case Player_State.CHANGE_DIRECTION:
				
			break;
				
				
	 		case Player_State.CONTROLLING:
			 	
			break;

			case Player_State.OPONENT_ATTACK:
				
			break;
				
				
			case Player_State.PICK_BALL:
				
			break;
				

			case Player_State.SHOOTING:
				
			break;
				
			case Player_State.PASSING:
				
	 		case Player_State.GO_ORIGIN://Para hacer que jugador ingrese al campo
			 		
	 							
			break;

			case Player_State.MOVE_AUTOMATIC:
				
			break;
			
	 
	 		case Player_State.RESTING:
				
	 		
	 		break;
							
				
			case Player_State.ONE_STEP_BACK:
				
				
			break;
				
				
			case Player_State.STOLE_BALL:
			
				


            break;


			case Player_State.TACKLE:
				

			break;

			case Player_State.DESPLAZAR1:
				
			break;

			case Player_State.DESPLAZAR:
					
			
			break;
		
		}		
			
	}
		
	
	void OnCollisionEnter(Collision other){
		
		
	}
	void OnCollisionStay( Collision coll ) {
        
		
	}
	//Esta funcion se va a disparar cuando se entra en contacto con un trigger
	void OnTriggerEnter( Collider other){

		
		
	}

	//Esta funcion realiza animancion de desplazamiento entre dos lugares. 
	//Recibe como argumento la posicion final a la cual debe de desplazarse.

	// Para cambiar posicion inicial en cambio de cancha
	
}