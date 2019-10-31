using UnityEngine;
using System.Collections;

public class Corner_Script : MonoBehaviour {
	
	
	public Transform downPosition;
	public Transform upPosition;
	
	public GameObject area;
	public Transform point_goalkick;
	public GameObject goalKeeper;

	public GameObject goalKeeper2;
	
	
	public Sphere sphere;

	public bool fueraCancha;

	//private GameObject[] players;
	//private GameObject[] oponents;
	
	// Use this for initialization
	void Start () {
		
		//players = GameObject.FindGameObjectsWithTag("PlayerTeam1");   //rojos
		//oponents = GameObject.FindGameObjectsWithTag("OponentTeam");  //Amarillos
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );
        if (transform.name=="Corner_Trigger_Left")
        {
            downPosition = GameObject.Find("Corner_LD").GetComponent<Transform>();
            upPosition = GameObject.Find("Corner_LU").GetComponent<Transform>();
            area = GameObject.Find("Area_Trigger_Left");
            goalKeeper = GameObject.Find("GoalKeeper_Local");
            goalKeeper2 = GameObject.Find("GoalKeeper_Visit");
            point_goalkick = GameObject.Find("Goal_kick_L").GetComponent<Transform>();
        }
        else if (transform.name == "Corner_Trigger_Right")
        {
            downPosition = GameObject.Find("Corner_RD").GetComponent<Transform>();
            upPosition = GameObject.Find("Corner_RU").GetComponent<Transform>();
            area = GameObject.Find("Area_Trigger_Right");
            point_goalkick = GameObject.Find("Goal_kick_R").GetComponent<Transform>();
            goalKeeper = GameObject.Find("GoalKeeper_Visit");
            goalKeeper2 = GameObject.Find("GoalKeeper_Local");

            //FIXME 17.9.19 no se si los porteros se cambian a medio partido, tengo que revisarlo
        }
		//jugador=(Player_Script)GameObject.FindObjectOfType(typeof(Player_Script));
		//inGame = GameObject.FindObjectOfType( typeof( InGameState_Script ) ) as InGameState_Script;
	}
	
	// Update is called once per frame
//	void Update () {
//			if(Camera.main.GetComponent<InGameState_Script>().state== InGameState_Script.InGameState.Medio_Tiempo){
//				goalKeeper=goalKeeper2;
//			}
	
//	}
		
	
//	void OnTriggerEnter( Collider other) {
		
		
//		//Mientras no sea un Goal, ingresa aquí
//		if ( Camera.main.GetComponent<InGameState_Script>().state != InGameState_Script.InGameState.GOAL ) {
			
//			// Detect if Players are outside of field
//			//Detecta si jugador esta fuera del campo
//			if ( ((other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING)) {
										
//				//if((Camera.main.GetComponent<InGameState_Script>().state != InGameState_Script.InGameState.CORNER||Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PRUEBA)){}
//				if ( other.gameObject != sphere.owner ) {//Entra si el jugador que ingrese en esta area es un jugador que no tiene el balon
//					////Debug.Log("OBJETO NO ES OWNER");
//					//El jugador es temporalmente inseleccionable 
//					//other.gameObject.transform.LookAt(other.gameObject.GetComponent<Player_Script>().MostrarInicio());
//					other.gameObject.GetComponent<Player_Script>().temporallyUnselectable = true;
//					other.gameObject.GetComponent<Player_Script>().timeToBeSelectable = 0.5f;
//					other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
//				}
				
//			}
			
//			if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state != InGameState_Script.InGameState.PLAYING ) {
//				other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
//				Debug.Log("ENTRÓ EN UN ESTADO DEL TRIGGER CORNER");
				
//			}
			
//			//PARA REGRESAR EL JUGADOR A LA CANCHA SI ESTÁ FUERA DE LA CANCHA
//			/*
//			if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ){
//				if(other.gameObject!=inGame.candidateToThrowIn && !inGame.candidateToThrowIn ){}
//			}
//			*/
	
//			// Chekc if is corner-kick or goal-kick
//			//Chekea si es corner o saque de puerta, cuando un jugador sale del campo con el balon
//			//if ( (other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING) || (other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PRUEBA )) {
//			if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING)  {	
//				/*
//				foreach(GameObject go in players){
//					go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
//				}
//				foreach(GameObject go in oponents){
//					go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
//				}*/
				
//				sphere.owner = null;//	Ningún jugador tiene el balon
//				Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;
//				Camera.main.GetComponent<InGameState_Script>().areaCorner = area; //Le asigna el area o posicion, del lugar  donde salio el balon
//				Camera.main.GetComponent<InGameState_Script>().goal_kick = point_goalkick; //Define la posicion donde se realiza el saque de puerta. Ya esta definido en el "Inspector"
//				Camera.main.GetComponent<InGameState_Script>().goalKeeper = goalKeeper;//Se asigna el portero del marco o area que corresponde
//				Camera.main.GetComponent<InGameState_Script>().cornerTrigger = this.gameObject; //Se asigna este triger
				
				
//				// loonking for the near corner point

//				//Buscando la esquina mas cercana.
//				Vector3 positionBall = sphere.gameObject.transform.position; //guarda en positionBall la posicion de la bola en este momento	
//				//downPosition y upPosition ya estan definidos en el Inspector
//				if ( (positionBall-downPosition.position).magnitude > (positionBall-upPosition.position).magnitude ) {
//					Camera.main.GetComponent<InGameState_Script>().cornerSource = upPosition;
//				} else {
//					Camera.main.GetComponent<InGameState_Script>().cornerSource = downPosition;		
//				}
				
//				Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.CORNER;
				
//			}
		
//		}	
		
//	}
}