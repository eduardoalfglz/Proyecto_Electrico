using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;  //Para trabajar con archivos
public class Player_Script : MonoBehaviour {

	// player name
	public string Name;
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
	private GameObject[] players;
	private GameObject[] oponents;
	public Vector3 resetPosition;
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
		
	public InGameState_Script inGame;
		
	public Texture barTexture;
	public Texture barStaminaTexture;
	private int barPosition=0;
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

	public ScorerTimeHUD scorerTime;

	private GameObject keeper;
	private GameObject keeper_oponent;
	
	//--------------------------------------------------------

	void  Awake () {

		GetComponent<Animation>().Stop();
		state = Player_State.PREPARE_TO_KICK_OFF; 
	}



	void  Start (){
        //Debug.Log(GetComponentInChildren<SkinnedMeshRenderer>().material);

        //Material Mat= Resources.Load("Materials/" + "player_texture_blue") as Material;
        //Debug.Log(Mat);
        GetComponentInChildren<SkinnedMeshRenderer>().material = Resources.Load("Materials/" + "player_texture_"+PlayerPrefs.GetString(transform.parent.name)) as Material;
       
        keeper = GameObject.FindGameObjectWithTag("GoalKeeper");
		keeper_oponent = GameObject.FindGameObjectWithTag("GoalKeeper_Oponent");
		listPosicion= new List<string>();
			
		// get players and oponents and save it in both arrays

		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");
	
		
		resetPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z );

			

		if ( gameObject.tag == "PlayerTeam1" ){
			initialPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z+initialDisplacement ); 

		}
		if ( gameObject.tag == "OponentTeam" ){
			initialPosition = new Vector3( transform.position.x, transform.position.y, transform.position.z-initialDisplacement ); 
				
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

		
	// control of actual player	
	void Case_Controlling() { //Para los players
	     ///--------------------------------MODIFIQUE----------------------------
		actualVelocityPlayer = transform.forward*5.0f*Time.deltaTime;
		GetComponent<Animation>().Play("running_ball");

		if(this.gameObject==sphere.owner && sphere.owner.tag=="PlayerTeam1"){
			Debug.Log("El que tienen el balon es el que lo controla");
			float x=keeper_oponent.GetComponent<GoalKeeper_Script>().transform.position.x;
			float y=keeper_oponent.GetComponent<GoalKeeper_Script>().transform.position.y;
			float z=keeper_oponent.GetComponent<GoalKeeper_Script>().transform.position.z;
			goalPosition=new Vector3(x,y,z);
		}
		if(this.gameObject==sphere.owner && sphere.owner.tag=="OponentTeam"){
			//float x=keeper_oponent.GetComponent<Goal
			//goalPosition.position=keeper.GetComponent.GetComponent<GoalKeeper_Script>().transform.position;
			float x=keeper.GetComponent<GoalKeeper_Script>().transform.position.x;
			float y=keeper.GetComponent<GoalKeeper_Script>().transform.position.y;
			float z=keeper.GetComponent<GoalKeeper_Script>().transform.position.z;
			goalPosition=new Vector3(x,y,z);
		}


		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(goalPosition);
		inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
		transform.Rotate(0, inputSteer*10.0f , 0);
		float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
		transform.position += transform.forward*4.0f*Time.deltaTime*staminaTemp*Speed;
		GuardarPosicionesList();
		
			
		timeToPass -= Time.deltaTime;
			
		if ( timeToPass < 0.0f && NoOneInFront( players ) ) {
			timeToPass = UnityEngine.Random.Range( 1.0f, 5.0f);	
			state = Player_State.PASSING;
			GetComponent<Animation>().Play("pass");
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true; //true
		}
			
		float distance = (goalPosition - transform.position).magnitude;
		Vector3 relative = transform.InverseTransformPoint(goalPosition);
			
		if ( distance < 20.0f && relative.z > 0 ) {

			state = Player_State.SHOOTING;
			GetComponent<Animation>().Play("shoot");
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true; //true
				
		}
		//------------------------------------------------------------------------------
		//-----------------------------------ORIGINAL---------------------------------------------------
		/*
		if ( sphere.inputPlayer == gameObject ) {
			/*		
			if ( sphere.fVertical != 0.0f || sphere.fHorizontal != 0.0f ) {
						
				oldVelocityPlayer = actualVelocityPlayer;
				
				Vector3 right = inGame.transform.right;
				Vector3 forward = inGame.transform.forward;
					
				right *= sphere.fHorizontal;
				forward *= sphere.fVertical;
					
				Vector3 target = transform.position + right + forward;
				target.y = transform.position.y;
							
				float speedForAnimation = 5.0f;
				
				// if is owner of Ball....
				if ( sphere.owner == gameObject ) {
				
					if ( GetComponent<Animation>().IsPlaying("rest") ) {
						GetComponent<Animation>().Play("starting_ball");
						speedForAnimation = 1.0f;
					}
						
					if ( GetComponent<Animation>().IsPlaying("starting_ball") == false )
						GetComponent<Animation>().Play("running_ball");
				
				}
				else {
						
					if ( GetComponent<Animation>().IsPlaying("rest") ) {
						GetComponent<Animation>().Play("starting");
						speedForAnimation = 1.0f;
					}
						
					if ( GetComponent<Animation>().IsPlaying("starting") == false )
						GetComponent<Animation>().Play("running");
						
				}
					
					
				transform.LookAt( target );
				float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				actualVelocityPlayer = transform.forward*speedForAnimation*Time.deltaTime*staminaTemp*Speed;
				transform.position += actualVelocityPlayer;
				
					
				// if get a radical diferent direction of player change animation...		
				float dotp = Vector3.Dot( oldVelocityPlayer.normalized, actualVelocityPlayer.normalized );
				
				if ( dotp < 0.0f && sphere.owner == gameObject ) {
			
					GetComponent<Animation>().Play("turn");
					state = Player_State.CHANGE_DIRECTION;
					transform.forward = -transform.forward;
					sphere.owner = null;
					gameObject.GetComponent<CapsuleCollider>().enabled = false;
					sphere.gameObject.GetComponent<Rigidbody>().AddForce(  -transform.forward.x*1500.0f, -transform.forward.y*1500.0f, -transform.forward.z*1500.0f );
				}
					
					
			} else {
		
				GetComponent<Animation>().Play("rest");
			}
			*/	
		/*		
			// pass
			if ( sphere.bPassButton && sphere.owner == gameObject ) {
				GetComponent<Animation>().Play("pass");
				timeToBeSelectable = 2.0f;
				state = Player_State.PASSING;
				sphere.pressiPhonePassButton = false;
			}
					
			// shoot
			if ( sphere.bShootButtonFinished && sphere.owner == gameObject ) {
				GetComponent<Animation>().Play("shoot");
				timeToBeSelectable = 2.0f;
				state = Player_State.SHOOTING;
				sphere.pressiPhoneShootButton = false;
				sphere.bShootButtonFinished = false;
			}


			if ( sphere.bPassButton && sphere.owner != gameObject ) {
				GetComponent<Animation>().Play("tackle");
	            //timeToBeSelectable = 2.0f;
				state = Player_State.TACKLE;
				sphere.pressiPhonePassButton = false;
			}

					
							
		} else {
		
			state = Player_State.MOVE_AUTOMATIC;
				
		}
		*/	
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
	
	
	// Oponent control
	void Case_Oponent_Attack() {
			
		actualVelocityPlayer = transform.forward*5.0f*Time.deltaTime;
		GetComponent<Animation>().Play("running_ball");
		Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(goalPosition);
		inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;
		transform.Rotate(0, inputSteer*10.0f , 0);
		float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
		transform.position += transform.forward*4.0f*Time.deltaTime*staminaTemp*Speed;
		GuardarPosicionesList();	
		timeToPass -= Time.deltaTime;
			
		if ( timeToPass < 0.0f && NoOneInFront( oponents ) ) {
			timeToPass = UnityEngine.Random.Range( 1.0f, 5.0f);	
			state = Player_State.PASSING;
			GetComponent<Animation>().Play("pass");
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true; //true
		}
			
		float distance = (goalPosition - transform.position).magnitude;
		Vector3 relative = transform.InverseTransformPoint(goalPosition);
			
		if ( distance < 20.0f && relative.z > 0 ) {

			state = Player_State.SHOOTING;
			GetComponent<Animation>().Play("shoot");
			timeToBeSelectable = 1.0f;
			temporallyUnselectable = true; //true
				
		}
			
	}
	
	void LateUpdate() {

		// turn head if necesary
		Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
		
		if ( relativePos.z > 0.0f ) {
	
			Quaternion lookRotation = Quaternion.LookRotation (sphere.transform.position + new Vector3(0, 1.0f,0) - headTransform.position);
			headTransform.rotation = lookRotation * initialRotation ;			
			headTransform.eulerAngles = new Vector3( headTransform.eulerAngles.x, headTransform.eulerAngles.y,  -90.0f);
			
		}
				
	}
	
	void  Update() {
				
					
		stamina += 2.0f * Time.deltaTime;
		stamina = Mathf.Clamp(stamina, 1, 64);// sujeta un valor entre uno mínimo y otro maximo		
		
		switch ( state ) {
			


			case Player_State.PREPARE_TO_KICK_OFF:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				GuardarPosicionesList();
				transform.LookAt( new Vector3(sphere.transform.position.x, transform.position.y, sphere.transform.position.z) );
			break;


			case Player_State.KICK_OFFER:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				GuardarPosicionesList();
				bPassButton = Input.GetKey(KeyCode.Space); 

				//--------------------------Modifiqué
				//if ( sphere.bPassButton || this.gameObject.tag == "OponentTeam" ) {
				
				///*if ( this.gameObject.tag == "OponentTeam" ) {
				if ( bPassButton){
					//Debug.Log("SE PRECIONO LA TECLA ESPACIO");
					GetComponent<Animation>().Play("pass");
					timeToBeSelectable = 2.0f;
					state = Player_State.PASSING;
					inGame.state = InGameState_Script.InGameState.PLAYING;
					
				}
				//*/
		
			break;

			case Player_State.THROW_IN:
				
			break;

			case Player_State.CORNER_KICK:
				
			break;
				
			case Player_State.CHANGE_DIRECTION:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				GuardarPosicionesList();
				if ( !GetComponent<Animation>().IsPlaying("turn")) {
					gameObject.GetComponent<CapsuleCollider>().enabled = true;
					transform.forward = -transform.forward;
					GuardarPosicionesList();
					GetComponent<Animation>().Play("rest");
					state = Player_State.CONTROLLING;//-------------------------------------------------------
					//PARA GUARDAR EL CAMBIO
					//GuardarPosicionesList();
				}
				
			break;
				
				
	 		case Player_State.CONTROLLING:
			 	transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				GuardarPosicionesList();
				if ( gameObject.tag == "PlayerTeam1" ) 
					Case_Controlling();			
			break;

			case Player_State.OPONENT_ATTACK:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				GuardarPosicionesList();
				Case_Oponent_Attack();			
			break;
				
				
			case Player_State.PICK_BALL:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				transform.position += transform.forward * Time.deltaTime * 5.0f;
				GuardarPosicionesList();
				////Debug.Log("FORWAR EN Player_State.PICK_BALL: "+transform.forward);

				//----------------------------
				//GuardarPosicionesList();
							
				if (GetComponent<Animation>().IsPlaying("fight") == false) {
					
					if ( gameObject.tag == "OponentTeam" )
						state = Player_State.OPONENT_ATTACK;
					 else
					{
						state = Player_State.MOVE_AUTOMATIC;
					}	
				}

			break;
				

			case Player_State.SHOOTING:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				
				if (GetComponent<Animation>().IsPlaying("shoot") == false)
					state = Player_State.MOVE_AUTOMATIC;

				
				if (GetComponent<Animation>()["shoot"].normalizedTime > 0.2f && sphere.owner == this.gameObject) {
					state = Player_State.MOVE_AUTOMATIC;
					sphere.owner = null;
					if ( gameObject.tag == "PlayerTeam1" ) {
						//---------------------------ORIGINAL----------------------------------
						//sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f, 5.0f, transform.forward.z*30.0f );
					    //barPosition = 0;

						//----------------Cambie----------------------------------------------------
						float valueRndY = UnityEngine.Random.Range( 4.0f, 10.0f );
						sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f, valueRndY, transform.forward.z*30.0f );
					//--------------------------------------------------------------------
					}

					else {
					
						float valueRndY = UnityEngine.Random.Range( 4.0f, 10.0f );
						sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(transform.forward.x*30.0f, valueRndY, transform.forward.z*30.0f );
					}
					
				}
			break;
				
			case Player_State.PASSING:
				//Debug.Log("JUGADOR HIZO UN PASE");
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);

				if (GetComponent<Animation>().IsPlaying("pass") == false)
					state = Player_State.MOVE_AUTOMATIC;
		
					
				if (GetComponent<Animation>()["pass"].normalizedTime > 0.3f && sphere.owner == this.gameObject) {
					sphere.owner = null;
									
					GameObject bestCandidatePlayer = null;
					float bestCandidateCoord = 1000.0f;
					
					//-------------------------- Modifiqué----------------
					if ( gameObject.tag == "PlayerTeam1" ) {

						foreach ( GameObject go in players ) {
							
							if ( go != gameObject ) {
								Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
										
								float magnitude = relativePos.magnitude;
								float direction = Mathf.Abs(relativePos.x);
								
								if ( relativePos.z > 0.0f && direction < 15.0f && (magnitude+direction < bestCandidateCoord) ) {
									bestCandidateCoord = magnitude+direction;
									bestCandidatePlayer = go;	//mejor jugador candidato	
								}
						
							}
								
						}


						///---------------------------ORIGINAL---------------------------------------
						/*
						foreach ( GameObject go in players ) {
							
							if ( go != gameObject ) {
								Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
										
								float magnitude = relativePos.magnitude;
								float direction = Mathf.Abs(relativePos.x);
								
								if ( relativePos.z > 0.0f && direction < 5.0f && magnitude < 15.0f && (direction < bestCandidateCoord) ) {
									bestCandidateCoord = direction;
									bestCandidatePlayer = go;
									
								}
							}
								
						}
						*/
					} else {
					
						foreach ( GameObject go in oponents ) {
							
							if ( go != gameObject ) {
								Vector3 relativePos = transform.InverseTransformPoint( new Vector3( go.transform.position.x, go.transform.position.y, go.transform.position.z  ) );
										
								float magnitude = relativePos.magnitude;
								float direction = Mathf.Abs(relativePos.x);
								
								if ( relativePos.z > 0.0f && direction < 15.0f && (magnitude+direction < bestCandidateCoord) ) {
									bestCandidateCoord = magnitude+direction;
									bestCandidatePlayer = go;	//mejor jugador candidato	
								}
						
							}
								
						}
						
					}
					
					if ( bestCandidateCoord != 1000.0f ) {
					
						//sphere.inputPlayer = bestCandidatePlayer;
						//El balon va hacia el candidato
						Vector3 directionBall = (bestCandidatePlayer.transform.position - transform.position).normalized;
						float distanceBall = (bestCandidatePlayer.transform.position - transform.position).magnitude*1.4f;
						distanceBall = Mathf.Clamp( distanceBall, 15.0f, 40.0f );//Delimita el valor entre min y max
						sphere.gameObject.GetComponent<Rigidbody>().velocity = new Vector3(directionBall.x*distanceBall, distanceBall/5.0f, directionBall.z*distanceBall );
					
					} else {
						// if not found a candidate just throw the ball forward....
						
						//Si no se encuentra un candidato solo tira la pelota hacia adelante.
						sphere.gameObject.GetComponent<Rigidbody>().velocity = transform.forward*20.0f;
						
					}
		
				
				
				}
				break;
	 		case Player_State.GO_ORIGIN://Para hacer que jugador ingrese al campo
			 	//Debug.Log("jugador: "+id+" Entró a Player_State.GO_ORIGIN");

			 	transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);//Para forzar a que siempre este en la misma posicion en y
				//transform.Rotate(0,90.0f,0);
				GetComponent<Animation>().Play("running");
				////Debug.Log("JUGADOR FUERA DE LA CANCHA "+id);
				// now we just find the relative position of the waypoint from the car transform,
				// that way we can determine how far to the left and right the waypoint is.

				/*ahora solo encontramos la posición relativa del punto de ruta a partir de 
				la transformación del automóvil, 				de esa manera podemos determinar
				 qué tan lejos a la izquierda y a la derecha se encuentra el punto de ruta. */

				Vector3 RelativeWaypointPosition = transform.InverseTransformPoint(new Vector3( 
															initialPosition.x, 
															initialPosition.y, 
															initialPosition.z ) );
				////Debug.Log("La RelativeWaypointPosition es: "+RelativeWaypointPosition);
		
				// by dividing the horizontal position by the magnitude, we get a decimal percentage of the turn angle that we can use to drive the wheels
				
				/*Al dividir la posición horizontal por la magnitud, obtenemos un porcentaje decimal del ángulo de giro
				*/
				inputSteer = RelativeWaypointPosition.x / RelativeWaypointPosition.magnitude;

				if ( inputSteer == 0 && RelativeWaypointPosition.z < 0 )
					inputSteer = 10.0f;
				
				transform.Rotate(0, inputSteer*10.0f , 0);
				float staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*3.0f*Time.deltaTime*staminaTemp*Speed;			transform.position += transform.forward*3.0f*Time.deltaTime;
				GuardarPosicionesList();
				////Debug.Log("FORWAR EN Player_State.GO_ORIGIN: "+transform.forward);
				////Debug.Log("Posicion del jugador que salio de la cancha: "+transform.position);
				if ( RelativeWaypointPosition.magnitude < 1.0f ) {
					state = Player_State.MOVE_AUTOMATIC;					
				}
				////GuardarPosicionesList();
					
	 							
			break;

			case Player_State.MOVE_AUTOMATIC:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);				
				timeToRemove += Time.deltaTime;	
				//float distance = (transform.position - initialPosition).magnitude;
				float distance = (transform.position - initialPosition1).magnitude;
				
				// know the distance of ball and player	

				//saber la distancia de pelota y jugador
				float distanceBallMove = (transform.position - sphere.transform.position).magnitude;
				
				// if we get out of bounds of our player we come back to initial position

				//Si salimos de los límites de nuestro jugador volvemos a la posición inicial.
				if ( distance > maxDistanceFromPosition ) {
										//Transforma la posición del espacio global al espacio local.
					Vector3 RelativeWaypointP = transform.InverseTransformPoint(new Vector3( 
																initialPosition.x, 
																initialPosition.y, 
																initialPosition.z ) );

					
					inputSteer = RelativeWaypointP.x / RelativeWaypointP.magnitude;
						
			
					if ( inputSteer == 0 && RelativeWaypointP.z < 0 )
						inputSteer = 10.0f;
						
					transform.Rotate(0, inputSteer*20.0f , 0);
					GetComponent<Animation>().Play("running");
					float staminaTemp2 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
					transform.position += transform.forward*5.5f*Time.deltaTime*staminaTemp2*Speed;
					GuardarPosicionesList();
					////Debug.Log("FORWAR EN Player_State.MOVE_AUTOMATIC: "+transform.forward);
					//mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
					//GuardarPosicionesList();
										
				} // if not we go to Ball...
				//Si la distancia es menor que maxDistanceFromPosition
				else {
			
					Vector3 ball = sphere.transform.position;
					Vector3 direction = (ball - transform.position).normalized;
					Vector3 posFinal = initialPosition + ( direction * maxDistanceFromPosition ); 
					
					Vector3 RelativeWaypointP = new Vector3(posFinal.x, posFinal.y, posFinal.z);
					
					// go to Ball position....

					//ir a la posición de la bola
					if ( distanceBallMove > 5.0f ) {
						RelativeWaypointP = transform.InverseTransformPoint(new Vector3( 
																	posFinal.x, 
																	posFinal.y, 
																	posFinal.z ) );

					} else if ( distanceBallMove < 5.0f && distanceBallMove > 2.0f ) {
					
						// if we are less than 5 meters of ball we stop

						//Si estamos a menos de 5 metros de la bola, paramos
						RelativeWaypointP = transform.InverseTransformPoint(new Vector3( 
																	transform.position.x, 
																	transform.position.y, 
																	transform.position.z ) );
		
					// if we are too close we go back with special animation

					//Si estamos demasiado cerca volvemos con animación especial.
					} else if ( distanceBallMove < 2.0f ) {
						
						GetComponent<Animation>().Play("jump_backwards_bucle");
						state = Player_State.ONE_STEP_BACK;
						break;
						
					}
					
					inputSteer = RelativeWaypointP.x / RelativeWaypointP.magnitude;
		
					if ( inputSteer == 0 && RelativeWaypointP.z < 0 )
						inputSteer = 10.0f;

					if ( inputSteer > 0.0f )
						transform.Rotate(0, inputSteer*20.0f , 0);
					
				
					// this just checks if the player's position is near enough.

					//Esto solo verifica si la posición del jugador está lo suficientemente cerca
					if ( RelativeWaypointP.magnitude < 1.5f ) {
											
						transform.LookAt( new Vector3( sphere.GetComponent<Transform>().position.x, transform.position.y ,sphere.GetComponent<Transform>().position.z)  );
						GetComponent<Animation>().Play("rest");		
						timeToRemove = 0.0f;
						
					}	else {			

						
						if ( timeToRemove > 1.0f ) {					
							GetComponent<Animation>().Play("running");
							staminaTemp = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN , STAMINA_MAX );
							transform.position += transform.forward*5.5f*Time.deltaTime*staminaTemp*Speed;
							GuardarPosicionesList();
							////Debug.Log("FORWAR EN Player_State.MOVE_AUTOMATIC FINAL: "+transform.forward);

							//mmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmmm
							////GuardarPosicionesList();
						}
					}
			
					
				}
				
				
			break;
			
	 
	 		case Player_State.RESTING:
				if(temporallyUnselectable!=true){
				transform.LookAt( new Vector3( sphere.GetComponent<Transform>().position.x, transform.position.y ,sphere.GetComponent<Transform>().position.z)  );
				
				GetComponent<Animation>().Play("rest"); 
				}		  
	 		
	 		break;
							
				
			case Player_State.ONE_STEP_BACK:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				if (GetComponent<Animation>().IsPlaying("jump_backwards_bucle") == false)
					state = Player_State.MOVE_AUTOMATIC;

				transform.position -= transform.forward*Time.deltaTime*4.0f;
				GuardarPosicionesList();
				////Debug.Log("FORWAR EN Player_State.ONE_STEP_BACK: "+transform.forward);	
				
			break;
				
				
			case Player_State.STOLE_BALL:
			
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				Vector3 relPos = transform.InverseTransformPoint( sphere.transform.position );
				inputSteer = relPos.x / relPos.magnitude;
				transform.Rotate(0, inputSteer*20.0f , 0);
				
				GetComponent<Animation>().Play("running");
				float staminaTemp3 = Mathf.Clamp ((stamina/STAMINA_DIVIDER), STAMINA_MIN ,STAMINA_MAX );
				transform.position += transform.forward*4.5f*Time.deltaTime*staminaTemp3*Speed;
				GuardarPosicionesList();
				////Debug.Log("FORWAR EN Player_State.STOLE_BALL: "+transform.forward);
			
				
				
			break;


			case Player_State.TACKLE:
				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				if ( GetComponent<Animation>().IsPlaying("tackle") ) {
				
					transform.position += transform.forward * (Time.deltaTime * (1.0f-GetComponent<Animation>()["tackle"].normalizedTime) * 10.0f);
					GuardarPosicionesList();
				} else {

					GetComponent<Animation>().Play("rest");
					temporallyUnselectable = false;
					state = Player_State.MOVE_AUTOMATIC;

				}

			break;

			case Player_State.DESPLAZAR1:
				valor_state=0;
			
			break;

			case Player_State.DESPLAZAR:
				////Debug.Log("ENTRO A Player_State.DESPLAZAR "+id);

				transform.position=new Vector3(transform.position.x,0.255f,transform.position.z);
				// ESTE CASO ES PARA DESPLAZAR JUGADORES HASTA UNA POSICION FINAL
				////Debug.Log("posicionFinal "+posicionFinal+ " posicionActual "+transform.position+ " id "+id);			
				float difMagnitud=(posicionFinal-transform.position).magnitude; //Saber distancia entre posicion final y actual
				
				if(difMagnitud > 0.5f){ //SI la diferencia entre posicion final y la actal es mayor de 0.5f, ingrese aquí.
					////Debug.Log("difMagnitud > 0.5f "+id);
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
					//Debug.Log("Jugador: "+id+" llegó al FINAL");
					transform.LookAt(sphere.transform);
					GetComponent<Animation>().Play("rest");

					CambioEstado();
					state=Player_State.DESPLAZAR1;//ESTADO PARA QUE NO HAGA NADA
					//valor_state=0;				
				}
					
			
			break;
		
		}		
							
		// after pass or shoot player get in a Unselectable state some little time

		//Después de pasar o disparar, el jugador se pone en un estado no seleccionable un poco de tiempo
		timeToBeSelectable -= Time.deltaTime;
				
		if ( timeToBeSelectable < 0.0f )
			temporallyUnselectable = false;
		else
			temporallyUnselectable = true;

	}
		
	
	void OnCollisionEnter(Collision other){
		
		if ( (other.transform.gameObject.tag == "PlayerTeam1" ||other.transform.gameObject.tag == "OponentTeam") && inGame.state == InGameState_Script.InGameState.PLAYING) {
			//Debug.Log("UN JUGADOR ESTA CHOCANDO ");
			/*
			if(this.gameObject==sphere.owner){
				
				state=Player_State.CHANGE_DIRECTION;
			}
			*/
		}
	}
	void OnCollisionStay( Collision coll ) {
		if ( (coll.transform.gameObject.tag == "PlayerTeam1" ||coll.transform.gameObject.tag == "OponentTeam")){
			//Debug.Log("UN JUGADOR ESTA CHOCANDO ");
		}
	
		if ( coll.collider.transform.gameObject.tag == "Ball" && !gameObject.GetComponent<Player_Script>().temporallyUnselectable ) {

			inGame.lastTouched = gameObject;
			if ( state == Player_State.TACKLE ) {

				sphere.transform.position += transform.forward;

			}

			Vector3 relativePos = transform.InverseTransformPoint( sphere.gameObject.transform.position );
		
			// only "glue" the ball to player if the collision is at bottom

			//solo "pegue" la pelota al jugador si la colisión está en la parte inferior
			if ( relativePos.y < 0.35f ) { 
			
				coll.rigidbody.rotation = Quaternion.identity;
				GameObject ball = coll.collider.transform.gameObject;
				ball.GetComponent<Sphere>().owner = gameObject;
				
				if ( gameObject.tag == "OponentTeam" ) {
					//////Debug.Log("AHORA LA BOLA ES DE OponentTeam ");
					state = Player_Script.Player_State.OPONENT_ATTACK;
				}
				//------------------------AGREGUE PARTE--------------
				if ( gameObject.tag =="PlayerTeam1" ) {
					//////Debug.Log("AHORA LA BOLA ES DE PlayerTeam1 ");
					state = Player_Script.Player_State.CONTROLLING;
				}
			}//------------------------------------------------------
				
			
		}
		
		
	}
	//Esta funcion se va a disparar cuando se entra en contacto con un trigger
	void OnTriggerEnter( Collider other){

		if(other.gameObject.tag=="barrera1"){
			////Debug.Log("OBJETO COLICIONÓ CON BARRERA: "+id);
		}

		
	}

	//Esta funcion realiza animancion de desplazamiento entre dos lugares. 
	//Recibe como argumento la posicion final a la cual debe de desplazarse.

	// Para cambiar posicion inicial en cambio de cancha
	public void PosicionInicial(Vector3 inicio){
		//resetPosition=inicio;
		initialPosition1=inicio;
		initialPosition=inicio;
	}
	//Definir nuevas posiciones iniciales
	//public void CambioCancha(Vector3 nuevo){
	public void CambioCancha(){	
		initialPosition1=new Vector3(initialPosition1.x,initialPosition1.y,initialPosition1.z*-1.0f);
		initialPosition=initialPosition1;
		//initialPosition=nuevo;
		//initialPosition1=nuevo;
		
	}

	// Retorna las posciones a las cuales debe de llegar cada jugador cuando se hace cambio de cancha y gol en segundo tiempo
	public Vector3 MostrarInicio(){
		
		//initialPosition1=new Vector3(initialPosition1.x,initialPosition1.y,initialPosition1.z*-1.0f);
		
		//CambioCancha(initialPosition1);
		return initialPosition1;
	}
	
	
	public string MostrarID(){
		return id;
	}

	// Retorna las posciones a las cuales debe de llegar cada jugador cuando se hace un gol, en primer tiempo
	public Vector3 MostrarInicioT1(){
		return initialPosition1;
	}
	
	//Define el estado (state) en InGameState_Script que sigue
	public void CambioEstado(){
		if(valor_state==1){
			inGame.state = InGameState_Script.InGameState.Medio_Tiempo;
		}	
		if(valor_state==2){inGame.state = InGameState_Script.InGameState.PREPARE_TO_KICK_OFF;}
		if(valor_state==3){inGame.state = InGameState_Script.InGameState.PRUEBA2;}
		if(valor_state==4){inGame.state = InGameState_Script.InGameState.THROW_1;}


	}
	//Retorna magnitud de la posicion mas larga.
	public float PosMasLarga(){
		return (posicionFinal).magnitude;
	}

	public void GuardarPosicionesList(){

		string time1 = string.Format ("{0:00}:{1:00}:{00:00}", scorerTime.minutes1, scorerTime.seconds1, scorerTime.seconds);
		string valor=id+","+transform.position.x+","+transform.position.y+","+transform.position.z+","+time1;
		listPosicion.Add(valor);
		//listPosicion.Add(time1);
	}
	public List<string> ExportarLista(){
		return listPosicion;
	}
	public void ReseterListPosiciones(){
		foreach(string s in listPosicion){
			listPosicion.Remove(s);
		}
	}

	/*OnGUI se llama para representar y manejar eventos GUI
	Es llamado una vez por frame
	*/		
	void OnGUI() {

		/* 
		//----------------------------------------
		Para el caso de los jugadores del players
		//----------------------------------------


		if ( sphere.timeShootButtonPressed > 0.0f && sphere.inputPlayer == this.gameObject) {
				
			Vector3 posBar = Camera.main.WorldToScreenPoint( headTransform.position + new Vector3(0,0.8f,0) );
			GUI.DrawTexture( new Rect( posBar.x-30, (Screen.height-posBar.y), barPosition, 10 ), barTexture );
				
			barPosition = (int)(sphere.timeShootButtonPressed * 128.0f);
			if ( barPosition >= 63 )
				barPosition = 63;
				
		}
			*/
		if ( sphere.owner == this.gameObject ) {
			
			//Vector3 posBar = Camera.main.WorldToScreenPoint( headTransform.position + new Vector3(0,1.0f,0) );
			//GUI.DrawTexture( new Rect( posBar.x-30, (Screen.height-posBar.y), (int)stamina, 10 ), barStaminaTexture );
			//stamina -= 1.5f * Time.deltaTime;
				
		}

		
	
	}
}