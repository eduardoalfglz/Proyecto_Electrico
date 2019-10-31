
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GoalKeeper_Script : MonoBehaviour {

	public string Name;	
	
	public enum GoalKeeper_State { 
	   RESTING,
	   GO_ORIGIN,
	   STOLE_BALL,
	   GET_BALL_DOWN,
	   UP_WITH_BALL,
	   PASS_HAND,
	   GOAL_KICK,
	   JUMP_LEFT,
	   JUMP_RIGHT,
	   JUMP_LEFT_DOWN,
	   JUMP_RIGHT_DOWN,
	   DESPLAZAR,
	   DESPLAZAR1	  
	};
	
	public GoalKeeper_State state;
	public Transform centro_campo;
	public Sphere sphere;
	public Vector3 initial_Position;
	public Transform hand_bone;
	private float timeToSacar = 1.0f;
	public CapsuleCollider capsuleCollider;	

	//-----------------------------------------------
	private const float STAMINA_DIVIDER = 64.0f;
	private const float STAMINA_MIN = 0.5f;	
	private const float STAMINA_MAX = 1.0f;	

	public float Speed = 1.0f;
	public string id;
	public Vector3 posicionFinal;
	public int BanderaPosicion=0;
	public int PosFinalMasLarga=0;
	public float stamina = 64.0f;
	//public InGameState_Script inGame;
	private float inputSteer; //Direccion de entrada
	private Vector3 initialPosition1;
	public int valor_state=0; //para saber que 

	public List<string> listPosicion;
	private ScorerTimeHUD scorerTime;

	private GameObject[] players;
	private GameObject[] opponents;

	//-----------------------------------------------

	// Use this for initialization
	void Start () {
        scorerTime = GameObject.FindObjectOfType(typeof(ScorerTimeHUD)) as ScorerTimeHUD;
        players = GameObject.FindGameObjectsWithTag("PlayerTeam1");   //rojos
		opponents = GameObject.FindGameObjectsWithTag("OpponentTeam");  //Amarillos
		listPosicion= new List<string>();

		initial_Position = transform.position;
		state = GoalKeeper_State.RESTING;
		GetComponent<Animation>()["running"].speed = 1.0f;		
		GetComponent<Animation>()["goalkeeper_clear_right_up"].speed = 1.0f;
		GetComponent<Animation>()["goalkeeper_clear_left_up"].speed = 1.0f;
		GetComponent<Animation>()["goalkeeper_clear_right_down"].speed = 1.0f;
		GetComponent<Animation>()["goalkeeper_clear_left_down"].speed = 1.0f;
	
	}
	
	// Update is called once per frame
	void Update () {

		//-----------------------------------
		//REQUERIDO PARA EL MOVIMIENTO DEL PORTERO
		stamina += 2.0f * Time.deltaTime;
		stamina = Mathf.Clamp(stamina, 1, 64);// sujeta un valor entre uno mínimo y otro maximo	
		//-----------------------------

		switch (state) {
	
			case GoalKeeper_State.JUMP_LEFT:
				
				capsuleCollider.direction = 0;
			
				if ( GetComponent<Animation>()["goalkeeper_clear_left_up"].normalizedTime < 0.45f  ) {
					transform.position -= transform.right * Time.deltaTime * 7.0f;
					GuardarPosicionesList();
				}
			
			
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_clear_left_up") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
			
			break;
	
			case GoalKeeper_State.JUMP_RIGHT:

				capsuleCollider.direction = 0;

				if ( GetComponent<Animation>()["goalkeeper_clear_right_up"].normalizedTime < 0.45f  ) {
					transform.position += transform.right * Time.deltaTime * 7.0f;
					GuardarPosicionesList();
				}		
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_clear_right_up") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
				
				
			break;
			
			case GoalKeeper_State.JUMP_LEFT_DOWN:
				
				
				capsuleCollider.direction = 0;
			
				if ( GetComponent<Animation>()["goalkeeper_clear_left_down"].normalizedTime < 0.45f  ) {
					transform.position -= transform.right * Time.deltaTime * 4.0f;
					GuardarPosicionesList();
				}
			
			
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_clear_left_down") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
			
			break;
	
			case GoalKeeper_State.JUMP_RIGHT_DOWN:

				capsuleCollider.direction = 0;

				if ( GetComponent<Animation>()["goalkeeper_clear_right_down"].normalizedTime < 0.45f  ) {
					transform.position += transform.right * Time.deltaTime * 4.0f;
					GuardarPosicionesList();
				}		
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_clear_right_down") ) {
					state = GoalKeeper_State.STOLE_BALL;		
					capsuleCollider.direction = 1;

				}
				
				
			break;
						
			case GoalKeeper_State.GOAL_KICK:
		
			break;			
			
			case GoalKeeper_State.PASS_HAND:
		
				if ( GetComponent<Animation>()["goalkeeper_throw_out"].normalizedTime < 0.65f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;
				}
		
				if ( GetComponent<Animation>()["goalkeeper_throw_out"].normalizedTime > 0.65f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) { 
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( transform.forward*5000.0f + new Vector3(0.0f, 1300.0f, 0.0f) );
				}
		
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_throw_out") || !GetComponent<Animation>().IsPlaying("goalkeeper_throw_out") ) {
					state  = GoalKeeper_State.GO_ORIGIN;			
				}
			
			break;
			

			case GoalKeeper_State.UP_WITH_BALL: //ARRIBA CON LA PELOTA
				sphere.codigo=1.0f;
			
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_catch_ball") ) {
				
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;
	
					timeToSacar -= Time.deltaTime;
					
					if ( timeToSacar < 0.0f ) {
						timeToSacar = UnityEngine.Random.Range( 2.0f, 5.0f );
						GetComponent<Animation>().Play("goalkeeper_throw_out");
						state = GoalKeeper_State.PASS_HAND;
					}
				
				
				} else {
				
					sphere.gameObject.transform.position = hand_bone.position;
					sphere.gameObject.transform.rotation = hand_bone.rotation;
				
					/*				
					Vector3 relativeCenter = transform.InverseTransformPoint( centro_campo.position );
					if ( relativeCenter.x > 10 )
						transform.Rotate( 0, 10, 0);
					else if ( relativeCenter.x < -10 )
						transform.Rotate( 0, -10, 0);
					*/
				
					transform.LookAt( centro_campo.position );
				
				
				}

			break;			
			
			case GoalKeeper_State.GET_BALL_DOWN: //CONSEGUIR EL BALON
			
				sphere.gameObject.transform.position = hand_bone.position;
				sphere.gameObject.transform.rotation = hand_bone.rotation;
				
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_get_ball_front") ) {
					GetComponent<Animation>().Play("goalkeeper_catch_ball");
					state = GoalKeeper_State.UP_WITH_BALL;
				}
			
			break;
			
			
			case GoalKeeper_State.RESTING:
			
				capsuleCollider.direction = 1;
				if ( !GetComponent<Animation>().IsPlaying("goalkeeper_rest") )
					GetComponent<Animation>().Play("goalkeeper_rest");
				
				transform.LookAt( new Vector3( sphere.gameObject.transform.position.x, transform.position.y , sphere.gameObject.transform.position.z)  );
			
				float distanceBall = (transform.position - sphere.gameObject.transform.position).magnitude;
		
				if ( distanceBall < 10.0f ) {
					state = GoalKeeper_Script.GoalKeeper_State.STOLE_BALL;
				} 
			
			
			break;

			case GoalKeeper_State.STOLE_BALL: //Bola robada

				GetComponent<Animation>().Play("running");

				Vector3 RelativeWaypointPosition = transform.InverseTransformPoint( sphere.gameObject.transform.position );
	
				float inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
			
				transform.Rotate(0, inputSteer*10.0f , 0);
				transform.position += transform.forward*6.0f*Time.deltaTime;
				GuardarPosicionesList();

		
				if ( RelativeWaypointPosition.magnitude < 1.0f ) {
					//Estaba quitado**********************************
       				//state = GoalKeeper_State.RESTING;					
				}

			break;
	
	
			case GoalKeeper_State.GO_ORIGIN:

				GetComponent<Animation>().Play("running");
				RelativeWaypointPosition = transform.InverseTransformPoint( initialPosition1 );
	
				inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
			
				transform.Rotate(0, inputSteer*10.0f, 0);
				transform.position += transform.forward*6.0f*Time.deltaTime;
				GuardarPosicionesList();
		
				if ( RelativeWaypointPosition.magnitude < 1.0f ) {
			
					state = GoalKeeper_State.RESTING;					
				}
		
			
			
			break;
			
			//--------------AGRUEGO
			case GoalKeeper_State.DESPLAZAR:
				// ESTE CASO ES PARA DESPLAZAR JUGADORES HASTA UNA POSICION FINAL
				
				float difMagnitud=(posicionFinal-transform.position).magnitude; //Saber distancia entre posicion final y actual
				
				if(difMagnitud > 0.35f){ //SI la diferencia entre posicion final y la actal es mayor de 1.0f, ingrese aquí.

					Vector3 relPos1 = transform.InverseTransformPoint( posicionFinal );
					inputSteer = relPos1.x / relPos1.magnitude;
					transform.Rotate(0, inputSteer*20.0f , 0);
				
					GetComponent<Animation>().Play("running");
					float staminaTemp4 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
					transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp4*Speed;
					GuardarPosicionesList();

				}else{
					// El jugador rota hasta quedar en direccion de la pelota y cambia animacion a 
					// descanzar o "rest"
					transform.LookAt(sphere.transform);
					GetComponent<Animation>().Play("rest");
					//BanderaPosicion=1;
					
					CambioEstado();
					
				}	
			
			break;
			case GoalKeeper_State.DESPLAZAR1:
				GetComponent<Animation>().Play("rest");
			break;
			//--------------	
			
		}
		
	}

	//--------------------FUNCIONES QUE PUSE------------

	public void PosicionInicial(Vector3 inicio){
		//Debug.log("portero: "+id+"Pos_ini_goalScrip: "+initial_Position+ "Pos_ini_InGame: "+inicio );
		initialPosition1=inicio;
		initial_Position=inicio;
		
	}
	

	public Vector3 MostrarInicio(){
		
		return initialPosition1;
	}
	public string MostrarID(){
		return id;
	}
	public void CambioCancha(){
		initialPosition1=new Vector3(initial_Position.x,initial_Position.y,initial_Position.z*-1.0f);
		initial_Position=initialPosition1;
	}
	
	public void PosMasLarga(int valor){
		PosFinalMasLarga=valor;
	}

	public void CambioEstado(){
		if(valor_state==1){
			//inGame.state = InGameState_Script.InGameState.Medio_Tiempo;
		}		
	}

	public void GuardarPosicionesList(){

		string valor=id+"\n";
		listPosicion.Add(valor);

		valor ="X: "+transform.position.x + ", "+"Y: "+transform.position.y + ", "+"Z: "+transform.position.z +"\n";
		listPosicion.Add(valor);
		string time = string.Format ("{0:00}:{1:00}:{00:00}", scorerTime.minutes1, scorerTime.seconds1,scorerTime.seconds);
		listPosicion.Add(time);
	}
	public List<string> ExportarLista(){
		return listPosicion;
	}
	public void ReseterListPosciones(){
		foreach(string s in listPosicion){
			listPosicion.Remove(s);
		}
	}
	//--------------------------------------------------
	
	
	
	// To know if GoalKeeper is touching Ball
	//Para saber si el portero esta tocando el balon
	void OnCollisionStay( Collision coll ) {
		
		//if ( Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		
		//	if ( coll.collider.transform.gameObject.tag == "Ball" && state != GoalKeeper_State.UP_WITH_BALL && state != GoalKeeper_State.PASS_HAND && state != GoalKeeper_State.GOAL_KICK &&
		//		 state != GoalKeeper_State.JUMP_LEFT && state != GoalKeeper_State.JUMP_RIGHT &&
		//		 state != GoalKeeper_State.JUMP_LEFT_DOWN && state != GoalKeeper_State.JUMP_RIGHT_DOWN) {
							
		//		Camera.main.GetComponent<InGameState_Script>().lastTouched = gameObject;//ESTE ES EL ULTIMO JUGADOR EN TOCAR EL BALON

		//		sphere.codigo=0.0f;
		//		foreach ( GameObject go in players ) {
		//			go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
		//		}
		//		foreach ( GameObject go in opponents ) {
		//			go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
		//		}

		//		Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
				
		//		// only get ball if the altitude is 0.35f (relative)
		//		if ( relativePos.y < 0.35f ) { 
				
		//			sphere.owner = null;
		
		//			GetComponent<Animation>().Play("goalkeeper_get_ball_front");
		//			state = GoalKeeper_State.GET_BALL_DOWN;
					
		//		}
				
				
		//	}
		
		//}
		
	}
	
}
