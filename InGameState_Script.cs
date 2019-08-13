using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using UnityEngine.SceneManagement;

public class InGameState_Script : MonoBehaviour {

	
	public enum InGameState {

		PLAYING,
		PREPARE_TO_KICK_OFF,
		KICK_OFF,
		GOAL,
		THROW_IN,
		THROW_IN_CHASING,
		THROW_IN_DOING,
		THROW_IN_DONE,
		CORNER,
		CORNER_CHASING,
		CORNER_DOING,
		CORNER_DOING_2,
		CORNER_DONE,
		GOAL_KICK,
		GOAL_KICK_RUNNING,
		GOAL_KICK_KICKING,
		Medio_Tiempo,
		DESPLAZAR,
		DESPLAZAR1,
		PRUEBA,
		PRUEBA2,
		THROW_1,
		FIN
	};
	

	//public Material localTeam;
	//public Material visitTeam;
	
	public bool scoredbylocal = true; //false
	public bool scoredbyvisiting = false; //true

	public InGameState state;
	private GameObject[] players;
	private GameObject[] oponents;
	private GameObject keeper;
	private GameObject keeper_oponent;
	//------------------
	
	//---------------------
	public GameObject lastTouched;
	public float timeToChangeState = 0.0f;
	public Vector3 positionSide;
	public Sphere sphere;
	public Transform center;
	public Vector3 target_throw_in;
	private GameObject whoLastTouched;
	public GameObject candidateToThrowIn;
	private float timeToSaqueOponent = 3.0f;
	
	public Transform cornerSource;
	public GameObject areaCorner;
	public Transform goal_kick;
	public GameObject goalKeeper;
	public GameObject cornerTrigger;
	
	public Mesh[] Meshes;
	public Material[] Mat;
	
	private float timeToKickOff = 4.0f;
	public GameObject lastCandidate = null;
	//Para el marcador
	public int score_local = 0;
	public int score_visiting = 0;
	
	public GameObject[] playerPrefab;
	public GameObject goalKeeperPrefab;
	public GameObject ballPrefab;

	public Transform target_oponent_goal;

	public ScorerTimeHUD scorerTime;
	public int bFirstHalf = 0;	

	public Material localMaterial;
	public Material visitMaterial;
	
	//Son los jugadores mas largo a ser colocados en una posicion
	public GameObject masLargo_atacante;
	public GameObject masLargo_defensor;

	public GameObject jugadorMasLargo=null;
	
	Vector3 cornerpos;
	public Vector3 inicio;
	
	public Vector3 medioTiempoO=new Vector3(-5.0f,0.0f,-1.0f);
	public Vector3 medioTiempoP=new Vector3(0.0f,0.0f,1.0f);

	
	public bool bPassButton; 
	
	StreamWriter archivo;

	public int identificador;
	
	void Awake() {
	
	}
	// Use this for initialization
	void Start () {
		archivo=new StreamWriter("ArchivoPosiciones1.csv",true);
		
		// search PLayers, Oponents and goalkeepers
		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");   //rojos
		oponents = GameObject.FindGameObjectsWithTag("OponentTeam");  //Amarillos
		keeper = GameObject.FindGameObjectWithTag("GoalKeeper");
		keeper_oponent = GameObject.FindGameObjectWithTag("GoalKeeper_Oponent");

		/*Poner los ID a los jugadores----------------AGREGUE-----------------
		*/
		//PonerIDjugadores();

		//Poner ID a los jugadores y ver cual es el que tiene la posicion mas cerca de la media cancha
		string idAux;
		int id=0;
		foreach ( GameObject go in players ) {  //ROJOS
			go.transform.position=new Vector3(go.transform.position.x,0.25f,go.transform.position.z);
			idAux="player_"+id;
			go.GetComponent<Player_Script>().id=idAux;
			
			id++;
			float distancia=(go.transform.position).magnitude;
			
			//Activa jugador mas cerca de media cancha
			if(distancia<11.0f){
				
				go.GetComponent<Player_Script>().mediaCancha1=true;//JUGADOR MAS CERCA DE LA MEDIA CANCHA
			}
			if(distancia>11.0f && distancia<12.5f){
				
				go.GetComponent<Player_Script>().mediaCancha2=true; //SEGUNDO JUGADOR MAS CERCA DE LA MEDIA CANCHA
			}
			
			//Guarda la posicion inicial del jugador
			go.SendMessage("PosicionInicial",go.transform.position);

			// Para guardar la posicion en una lista
			go.SendMessage("GuardarPosicionesList");
			
		}
		
		id=0;

		foreach ( GameObject go in oponents ) { //AMARILLOS
			go.transform.position=new Vector3(go.transform.position.x,0.25f,go.transform.position.z);
			idAux="oponent_"+id;
			go.GetComponent<Player_Script>().id=idAux;
			id++;

			float distancia=(go.transform.position).magnitude;


			//Activa jugador mas cerca de media cancha
			
			if(distancia<11.0f){
				
				go.GetComponent<Player_Script>().mediaCancha1=true;//JUGADOR MAS CERCA DE LA MEDIA CANCHA
				identificador=id-1;
												
			}
			if(distancia>11.0f && distancia<12.5f){
				
				go.GetComponent<Player_Script>().mediaCancha2=true; //SEGUNDO JUGADOR MAS CERCA DE LA MEDIA CANCHA
								
			}
			
			go.SendMessage("PosicionInicial",go.transform.position);
			go.SendMessage("GuardarPosicionesList"); //Guarda posiciones en lista
			
		}

		idAux="PorteroPlyer";
		keeper.GetComponent<GoalKeeper_Script>().id=idAux;// AGREGAR ESTA FUNCION ******
		keeper.SendMessage("PosicionInicial",keeper.transform.position);
		keeper.SendMessage("GuardarPosicionesList");
		
		idAux="PorteroOponent";
		keeper_oponent.GetComponent<GoalKeeper_Script>().id=idAux;
		keeper_oponent.SendMessage("PosicionInicial",keeper_oponent.transform.position);
		keeper_oponent.SendMessage("GuardarPosicionesList");
		
		
		
		state = InGameState.PREPARE_TO_KICK_OFF;
		//----------------------------------AGREGUE---------------------------
		scoredbylocal = true; //false,  Lo puse
		scoredbyvisiting = false; //true, Lo puse
		//--------------------------------------------------------------------
		bFirstHalf = 0;	

		//EstaturaJugadores(players,oponents,keeper,keeper_oponent,0.1f,0.2f,0.2f);
		// Load Team textures 
		//LoadTeams ();

	}
	
    //This code is useless ED
	void LoadTeams ()
	{
        Debug.Log(PlayerPrefs.GetString("Local"));
        Debug.Log(PlayerPrefs.GetString("Visit"));
        localMaterial.mainTexture = Resources.Load ("Textures/" + "player_" + PlayerPrefs.GetString ("Local") + "_texture") as Texture2D;
		visitMaterial.mainTexture = Resources.Load ("Textures/" + "player_" + PlayerPrefs.GetString ("Visit") + "_texture") as Texture2D;
	}	
	
	


	// Update is called once per frame
	void Update () {
		int j=0;
		// little time between states
		timeToChangeState -= Time.deltaTime;
		
		if ( timeToChangeState < 0.0f ) {
		

			// Handle all states related to match

			switch (state) {

				case InGameState.PLAYING://Jugando
					//Debug.Log("ESTOY JUGANDO");
					//candidateToThrowIn=null; //El candidato a saque de banda es null								
					//////Debug.Log("Jugador: "+oponents[identificador]+" ESTADO "+oponents[identificador].GetComponent<Player_Script>().state+" POSICION.z "+oponents[identificador].transform.position.z+" unSelectable "+oponents[identificador].GetComponent<Player_Script>().temporallyUnselectable);

					if ( scorerTime.minutes ==44 && bFirstHalf == 0){
						
						bFirstHalf = 1;
						
					}
					// Se realiza cambio de media cancha.
					if ( scorerTime.minutes == 45 && bFirstHalf == 1){
						medioTiempoO=new Vector3(0.0f,0.0f,1.0f);
						medioTiempoP=new Vector3(0.0f,0.0f,-1.0f);

						foreach(GameObject go in players){
							go.SendMessage("CambioCancha");
							}
						foreach(GameObject go in oponents){
							go.SendMessage("CambioCancha");
							}

						keeper.SendMessage("CambioCancha");
						keeper_oponent.SendMessage("CambioCancha");
						//jugadorMasLargo=keeper.gameObject;
						keeper.GetComponent<GoalKeeper_Script>().valor_state=1;//CAMBIA A MEDIO TIEMPO
							
						sphere.codigo=0.0f;

						bFirstHalf = 2;
						scoredbyvisiting=true;
						scoredbylocal=false;
						state=InGameState.DESPLAZAR1;

					}
					
				
					if ( scorerTime.minutes > 90 && bFirstHalf == 2) {

						PlayerPrefs.SetInt("ScoreLocal", score_local );
						PlayerPrefs.SetInt("ScoreVisit", score_visiting );

						Application.LoadLevel( "Select_Team" );
						state=InGameState.FIN;
					} 

							
				break;

				//-------------------------
				case InGameState.DESPLAZAR: //PONE A TODOS LOS JUGADORES A DESPLAZRSE
					sphere.owner = null;
					sphere.codigo=0.0f;
					////////Debug.Log("Ingresó a: "+InGameState.DESPLAZAR);
									
					j=0;
														
					while(j<10){
						////////Debug.Log("InGameState.DESPLAZAR j= "+j);
												
						//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
						players[j].GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
						//players[j].GetComponent<Player_Script>().valor_state=0;
							
												
						//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
						oponents[j].GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
						//oponents[j].GetComponent<Player_Script>().valor_state=0;
																	
						j++;					

					}
													
					
					keeper_oponent.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.DESPLAZAR;
					//keeper_oponent.GetComponent<GoalKeeper_Script>().valor_state=1;
				
										
					keeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.DESPLAZAR;
					//keeper.GetComponent<GoalKeeper_Script>().valor_state=0;
					/*
					if(jugadorMasLargo){
						jugadorMasLargo.GetComponent<Player_Script>().valor_state=1;
					}
					*/
					//jugadorMasLargo=null;																
				break;

				//*******************************************
				case InGameState.DESPLAZAR1: //ESTADO PARA Definir posiciones finales a las cuales debe de llegar el jugador
					
					////////Debug.Log("Ingresó a: "+InGameState.DESPLAZAR1);
					sphere.owner = null;
					sphere.codigo=0.0f;
					Vector3 playerA;
					Vector3 playerB;
					j=0;
					
					// Se requiere cambiar posiciones finales de llegada
					
					
					while(j<10){
						//PONER JUGADORES EN ESTADO DE DESCANSO 
						players[j].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						oponents[j].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						
						j++;

					}
					
					j=0;
					
					while(j<10){
						//Recupera las coordenadas iniciales de los jugadores "resetPosition"
						playerA=players[j].GetComponent<Player_Script>().MostrarInicio();
						playerB=oponents[j].GetComponent<Player_Script>().MostrarInicio();
						

						//AMARILLOS ----> OPONENTES
						//ROJOS ----> PLAYERS

						//AMARILLOS MUEVEN EL BALON
						if(scoredbylocal){
							
							if(oponents[j].GetComponent<Player_Script>().mediaCancha1==true){
								oponents[j].GetComponent<Player_Script>().posicionFinal= medioTiempoO;
													
							}else if(oponents[j].GetComponent<Player_Script>().mediaCancha2==true){
								oponents[j].GetComponent<Player_Script>().posicionFinal= new Vector3(0.0f,0.25f,medioTiempoO.z*(-4.0f));
							
							}
							else{
								//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
								oponents[j].GetComponent<Player_Script>().posicionFinal=playerB;
							}
							players[j].GetComponent<Player_Script>().posicionFinal=playerA;

						}
						//ROJOS
						if(scoredbyvisiting){
							
							if(players[j].GetComponent<Player_Script>().mediaCancha1==true){
								players[j].GetComponent<Player_Script>().posicionFinal=medioTiempoP;
												
							}else if(players[j].GetComponent<Player_Script>().mediaCancha2==true){
								Vector3 prue=new Vector3(0.0f,0.25f,medioTiempoP.z*(-4.0f));
								players[j].GetComponent<Player_Script>().posicionFinal= prue;// new Vector3(0.0f,0.0f,medioTiempoP.z*(-4.0f));
								
							}
							else{
								//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
								players[j].GetComponent<Player_Script>().posicionFinal=playerA;
								
							}
							oponents[j].GetComponent<Player_Script>().posicionFinal=playerB;

						}
											
						j++;					

					}
									

					playerB=keeper_oponent.GetComponent<GoalKeeper_Script>().MostrarInicio();
					keeper_oponent.GetComponent<GoalKeeper_Script>().posicionFinal=playerB;

					playerA=keeper.GetComponent<GoalKeeper_Script>().MostrarInicio();
					keeper.GetComponent<GoalKeeper_Script>().posicionFinal=playerA;
					
					
																					
					state = InGameState_Script.InGameState.DESPLAZAR;
					
													
				break;
				//*****************************************


				case InGameState.PRUEBA:
					////Debug.Log(candidateToThrowIn.GetComponent<Player_Script>().id +"_"+candidateToThrowIn.GetComponent<Player_Script>().state);
								
				break;

				case InGameState.FIN:

					foreach(GameObject go in players){
						//GuardarPosicionesListTXT(go);
					}
					foreach(GameObject go in oponents){
						//GuardarPosicionesListTXT(go);
					}
					//GuardarPosicionesListTXT(keeper);
					//GuardarPosicionesListTXT(keeper_oponent);
					archivo.Close();
					SceneManager.LoadScene("Football_Match");
				break;


				//-------------------------------------------------------------------------
				//El caso de Medio_Tiempo, 
				//realiza el movimiento de jugadores hasta 
				// posicion de inicio de medio tiempo
				case InGameState.Medio_Tiempo:
					////////Debug.Log("Ingresó a: "+InGameState.Medio_Tiempo);
								
					int k=0;
					while(k<10){
						
						players[k].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						GuardarPosicionesListTXT(players[k]);

						oponents[k].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						GuardarPosicionesListTXT(oponents[k]);
												
						k++;

					}
					
					keeper_oponent.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
					//GuardarPosicionesListTXT(keeper_oponent);//No se puede porque se obtuvo componente Player_Script

					keeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
					//GuardarPosicionesListTXT(keeper);

					scoredbylocal=false;
					scoredbyvisiting =true;	

					//archivo.Close();// CIERRO EL ARCHIVO AQUI-------------------------------

					state = InGameState_Script.InGameState.PREPARE_TO_KICK_OFF;
					
													
				break;
	
				case InGameState.THROW_IN://Saque de banda
					
				
					whoLastTouched = lastTouched;
					sphere.transform.position=positionSide;
					//Debug.Log("Posicion para SAQUE BANDA "+positionSide);
					////////Debug.Log("Ultimo Jugador en Tocar Balon es InGameState.THROW_IN = "+ whoLastTouched);
					////////Debug.Log("El tag del ultimo jugador en tocar balon es: "+ whoLastTouched.tag);
					//Debug.Log("UTIMO EN TOCAR EL BALON "+whoLastTouched);	
				
					foreach ( GameObject go in players ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
					foreach ( GameObject go in oponents ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
					//Para que ultimo jugador en tocar el balon vuelva a la cancha
					whoLastTouched.transform.LookAt(whoLastTouched.GetComponent<Player_Script>().MostrarInicio());		
					whoLastTouched.GetComponent<Player_Script>().state=Player_Script.Player_State.GO_ORIGIN;
					sphere.owner = null;
					

					if ( whoLastTouched.tag == "PlayerTeam1" )
						candidateToThrowIn = SearchPlayerNearBall( oponents );
					else{	
						candidateToThrowIn = SearchPlayerNearBall( players );
					}
					
					//Debug.Log("Candidato a saque de banda: "+candidateToThrowIn+ " y tag es "+candidateToThrowIn.tag);

					//Para quitar el balon de enfrente del candidato a saque de banda
					sphere.transform.position=new Vector3(sphere.transform.position.x,sphere.transform.position.y,sphere.transform.position.z-2.0f);
					
					candidateToThrowIn.GetComponent<Player_Script>().posicionFinal=new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);
					candidateToThrowIn.GetComponent<Player_Script>().state=Player_Script.Player_State.DESPLAZAR;
					candidateToThrowIn.GetComponent<Player_Script>().valor_state=4; //CAMBIA A THROW_1
				
					state = InGameState.PRUEBA;
				
				break;

				case InGameState.THROW_1:
					
					candidateToThrowIn.transform.position = new Vector3( positionSide.x, candidateToThrowIn.transform.position.y, positionSide.z);
					candidateToThrowIn.SendMessage("GuardarPosicionesList");
					//Para que jugador gire hacia la cancha
					candidateToThrowIn.transform.LookAt(center.position);

					if ( whoLastTouched.tag == "PlayerTeam1" ) {
					
						candidateToThrowIn.GetComponent<Player_Script>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player_Script>().timeToBeSelectable = 3.0f;

						//MIRA HACIA EL COMPAÑERO MAS CERCANO
						GameObject receptor=SearchPlayerNearBall( oponents );
						candidateToThrowIn.transform.LookAt( receptor.transform.position);
						////Debug.Log("RECEPTOR AMARILLO ES: "+ receptor +" y el tag es: "+receptor.tag);
					}
					else{
						GameObject receptor=SearchPlayerNearBall( players );
						candidateToThrowIn.transform.LookAt( receptor.transform.position);
						////Debug.Log("RECEPTOR ROJO ES: "+ receptor +" y el tag es: "+receptor.tag);
						//candidateToThrowIn.transform.LookAt( center ); 
					}
							//fHorizontal  no funciona, tengo que ver porque lo cambio
					//candidateToThrowIn.transform.Rotate(0, sphere.fHorizontal*10.0f, 0);
					
					//candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.THROW_IN;//SAQUE DE BANDA, PERO NO HACE NADA, PORQUE NO TIENE NADA DEFINIDO
				
					
					sphere.GetComponent<Rigidbody>().isKinematic = true;
					//La bola se situa en las manos del jugador
					sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player_Script>().hand_bone.position;
					//Se define una posicion a partir de la posicion del jugador seleccionado para el saque de banda
					target_throw_in = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;

					candidateToThrowIn.GetComponent<Animation>().Play("saque_banda");
					candidateToThrowIn.GetComponent<Animation>()["saque_banda"].time = 0.1f;
					candidateToThrowIn.GetComponent<Animation>()["saque_banda"].speed = 0.0f;

					state=InGameState.THROW_IN_CHASING;


				break;
				case InGameState.THROW_IN_CHASING:
					////Debug.Log("INGRESÓ A InGameState.THROW_IN_CHASING");
					
					//candidateToThrowIn.transform.LookAt(center.position);
					candidateToThrowIn.GetComponent<Player_Script>().valor_state=0;//NO REALIZA CAMBIOS

					if ( whoLastTouched.tag != "PlayerTeam1" ) {
						////////Debug.Log("Ingresó en: whoLastTouched.tag != PlayerTeam1");
						
						timeToSaqueOponent -= Time.deltaTime;
					
						if ( timeToSaqueOponent < 0.0f ) {					
							timeToSaqueOponent = 3.0f;
							sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
							candidateToThrowIn.GetComponent<Animation>().Play("saque_banda");
							state = InGameState.THROW_IN_DOING;
						}

						/*
						target_throw_in += new Vector3( 0,0,sphere.fHorizontal/10.0f);
					
						//if (sphere.bPassButton) {//--------------MODIFIQUE---------------------
							candidateToThrowIn.GetComponent<Animation>().Play("saque_banda");
							state = InGameState.THROW_IN_DOING;
		
						//}
						*/
						
					} else {
						////////Debug.Log("Ingresó en: whoLastTouched.tag = PlayerTeam1");
						timeToSaqueOponent -= Time.deltaTime;
					
						if ( timeToSaqueOponent < 0.0f ) {					
							timeToSaqueOponent = 3.0f;
							sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
							candidateToThrowIn.GetComponent<Animation>().Play("saque_banda");
							state = InGameState.THROW_IN_DOING;
						}
					
					}
					//}
				
				break;	
				
				case InGameState.THROW_IN_DOING:
					////Debug.Log("INGRESÓ A InGameState.THROW_IN_DOING: ");
					
					candidateToThrowIn.GetComponent<Animation>()["saque_banda"].speed = 1.0f;

					if ( candidateToThrowIn.GetComponent<Animation>()["saque_banda"].normalizedTime < 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
						sphere.gameObject.transform.position = candidateToThrowIn.GetComponent<Player_Script>().hand_bone.position;
					}

					if ( candidateToThrowIn.GetComponent<Animation>()["saque_banda"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
						sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
						sphere.gameObject.GetComponent<Rigidbody>().AddForce( candidateToThrowIn.transform.forward*4000.0f + new Vector3(0.0f, 1300.0f, 0.0f) );					
					} 
				
				
				
					if ( candidateToThrowIn.GetComponent<Animation>().IsPlaying("saque_banda") == false ) {
						state = InGameState.THROW_IN_DONE;
					}
				
				
				break;

				case InGameState.THROW_IN_DONE:
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;
					state = InGameState.PLAYING;
				
				break;
							
				
				case InGameState.CORNER:
					
					
					whoLastTouched = lastTouched;	 // Jugador que toco la pelota por ultima vez
					////////Debug.Log("Ultimo jugador en tener el balon "+whoLastTouched);

					//Define en cual porteria se realiza el saque de esquina y elige equipo
					if ( whoLastTouched.tag == "GoalKeeper_Oponent" )
						whoLastTouched.tag = "OponentTeam";	//AMARILLO
					if ( whoLastTouched.tag == "GoalKeeper" )
						whoLastTouched.tag = "PlayerTeam1";	//ROJOS
				
				
				
					// decidimos si es Corner o Saque de puerta

					//--------+-----------+----Saque de puerta----------+----------------+
				
					if ( cornerTrigger.tag == "Corner_Oponent" && whoLastTouched.tag == "PlayerTeam1") {
						state = InGameState.GOAL_KICK;
						break;
					}
					if ( cornerTrigger.tag != "Corner_Oponent" && whoLastTouched.tag == "OponentTeam" ) {
						state = InGameState.GOAL_KICK;
						break;
					}
								
					//--------+-----------+-----Saque de puerta---------+----------------+	

					//-----------------*-----------tiro de esquina--------*----------------	
					

					//Debug.Log("Ingresó a InGameState.CORNER");
					
					//Se pone a descanzar o se para a todos los jugadores 	
					foreach ( GameObject go in players ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}
					foreach ( GameObject go in oponents ) {
						go.GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
					}

					whoLastTouched.transform.LookAt(whoLastTouched.GetComponent<Player_Script>().MostrarInicio());
				
					//sphere.owner = null; //Ningun jugador tiene la pelota
					//sphere.prueba=0;
					
					sphere.transform.position=new Vector3(cornerSource.position.x,cornerSource.position.y,cornerSource.position.z);
					sphere.owner = null; //Ningun jugador tiene la pelota
					//PONER JUGADORES EN EL AREA Y COLOCAR JUGADOR PARA REALIZAR EL SAQUE DE ESQUINA
					//-----------L-------------------------L---------------------------
					
					// Los amarillos realizan el corner
					if ( whoLastTouched.tag == "PlayerTeam1" ) { //SI EL ULTIMO En TOCAR EL BALON FUE ROJO
						//El candidato a realizar el saque es amarillo y es el mas cercano al balon
						candidateToThrowIn = SearchPlayerNearBall( oponents );

						if(candidateToThrowIn.GetComponent<Player_Script>().type== Player_Script.TypePlayer.ATTACKER){
							candidateToThrowIn.GetComponent<Player_Script>().type= Player_Script.TypePlayer.MIDDLER;
						}
						PutPlayersInCornerArea( players ,oponents, Player_Script.TypePlayer.ATTACKER );

						//ColocarJugadorCerca(players,oponents, Player_Script.TypePlayer.ATTACKER);
										
					}
					//El  tiro de esquina se realiza en el area de los amarillos
					else {	
						//El candidato a realizar el saque es rojo y es el mas cercano al balón
						candidateToThrowIn = SearchPlayerNearBall( players );
						if(candidateToThrowIn.GetComponent<Player_Script>().type== Player_Script.TypePlayer.ATTACKER){
							candidateToThrowIn.GetComponent<Player_Script>().type= Player_Script.TypePlayer.MIDDLER;
						}
						PutPlayersInCornerArea( oponents,players, Player_Script.TypePlayer.ATTACKER );
						//ColocarJugadorCerca(oponents,players, Player_Script.TypePlayer.ATTACKER);					
						
					}
					
				
					//Jugador se situa en el corner que le corresponde para el saque de esquina
					candidateToThrowIn.transform.LookAt(sphere.transform.position);//Gira hacia la esquina del CORNER

					cornerpos=new Vector3 ( cornerSource.position.x, candidateToThrowIn.transform.position.y, cornerSource.position.z);
					//Debug.Log("cornerpos "+cornerpos+"ID candidateToThrowIn"+candidateToThrowIn.GetComponent<Player_Script>().id);
					candidateToThrowIn.GetComponent<Player_Script>().posicionFinal=cornerpos;
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
					candidateToThrowIn.GetComponent<Player_Script>().valor_state=0;

					////Debug.Log("JUGADOR QUE TIRA TIRO DE ESQUINA "+candidateToThrowIn);
					//-----------L-------------------------L---------------------------

				
					state = InGameState.PRUEBA;
				
				
				break;
				case InGameState.PRUEBA2://ES UN ESTADO DE CORNER. CORNER_1
					//Debug.Log("POSICION DEL JUGADOR DE TIRO ESQUINA "+candidateToThrowIn.transform.position);
					//Debug.Log("Ingresó a InGameState.PRUEBA2");

					candidateToThrowIn.transform.position=cornerpos;
					candidateToThrowIn.SendMessage("GuardarPosicionesList");
					//Se coloca el balon en posicion, en la esquina correspondiente
					sphere.gameObject.transform.position = cornerSource.position;

					if ( whoLastTouched.tag == "PlayerTeam1" ) { // SI EL ULTIMO EN TOCAR EL BALON FUE ROJO
					
						candidateToThrowIn.GetComponent<Player_Script>().temporallyUnselectable = true;
						candidateToThrowIn.GetComponent<Player_Script>().timeToBeSelectable = 1.5f;

						//Busca compañero mas cerca
						//Debug.Log("Jugador que realiza tiro Esquina: "+candidateToThrowIn);
						GameObject re=SearchPlayerNearBall( oponents );

						//Debug.Log("Jugador mas cerca a la bola: "+re);
						candidateToThrowIn.transform.LookAt( re.transform.position);

					}
					else{
						//Busca compañero mas cerca
							//Busca compañero mas cerca
						//Debug.Log("Jugador que realiza tiro Esquina: "+candidateToThrowIn);
						GameObject re=SearchPlayerNearBall( players);

						//Debug.Log("Jugador mas cerca a la bola: "+re);
						candidateToThrowIn.transform.LookAt( re.transform.position);

					}
					//POR EL MOMENTO NO HACE NADA PORQUE ESE ESTADO NO TIENE NADA
					candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.CORNER_KICK;
					sphere.GetComponent<Rigidbody>().isKinematic = true;

					//Posicion del candidato a realizar el tiro de esquina 
					target_throw_in = candidateToThrowIn.transform.position + candidateToThrowIn.transform.forward;
					candidateToThrowIn.GetComponent<Animation>().Play("rest");

					candidateToThrowIn.GetComponent<Player_Script>().valor_state=0;
					masLargo_atacante.GetComponent<Player_Script>().valor_state=0;
					masLargo_defensor.GetComponent<Player_Script>().valor_state=0;

					//////Debug.Log("LEGÓ AL FINAL DE InGameState.PRUEBA2");
					state = InGameState.CORNER_CHASING;


				break;


					
			case InGameState.CORNER_CHASING:
				//Debug.Log("Entró a CORNER_CHASING en InGameState");


				////Debug.Log("EL TIRADOR DE ESQUINA OBSERVA HACIA "+target_throw_in);
				//candidateToThrowIn.transform.LookAt( target_throw_in ); EN EL CASO ANTERIOR YA SE PUSO EL JUGADOR A MIRAR HACIA UN LADO

				//////Debug.Log("SE IMPRIME target_throw_in EN InGameState.CORNER_CHASING: ");
				//////Debug.Log(target_throw_in);

				//Este estado no hace nada porque no está definido
				candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.CORNER_KICK;
				
				sphere.GetComponent<Rigidbody>().isKinematic = true;

				if ( whoLastTouched.tag != "PlayerTeam1" ) {//Si el jugador no es ROJO
					//Debug.Log("Ingresó a:  whoLastTouched.tag != PlayerTeam1");

					timeToSaqueOponent -= Time.deltaTime;
					
					if ( timeToSaqueOponent < 0.0f ) {					
						timeToSaqueOponent = 3.0f;
						sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
						candidateToThrowIn.GetComponent<Animation>().Play("backwards");
						state = InGameState.CORNER_DOING;
					}

					/*
					target_throw_in += Camera.main.transform.right*(sphere.fHorizontal/10.0f);
					
					//if (sphere.bPassButton) {//-------------MODIFIQUE---------------------
						candidateToThrowIn.GetComponent<Animation>().Play("backwards");
						state = InGameState.CORNER_DOING;
		
					//}
					*/
						
				} else {//Si es amarillo
					//Debug.Log("Ingresó a:  whoLastTouched.tag = PlayerTeam1");
					
					timeToSaqueOponent -= Time.deltaTime;
					
					if ( timeToSaqueOponent < 0.0f ) {					
						timeToSaqueOponent = 3.0f;
						sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
						candidateToThrowIn.GetComponent<Animation>().Play("backwards");
						state = InGameState.CORNER_DOING;
					}
					
				}
				//}
				
			break;
			
			case InGameState.CORNER_DOING:
				//Debug.Log("Ingresó a: InGameState.CORNER_DOING");
			
				candidateToThrowIn.transform.position -= candidateToThrowIn.transform.forward * Time.deltaTime;
				candidateToThrowIn.SendMessage("GuardarPosicionesList");
				if ( candidateToThrowIn.GetComponent<Animation>().IsPlaying("backwards") == false ) {//Caminando hacia atras
					
					candidateToThrowIn.GetComponent<Animation>().Play("saque_esquina");
					state = InGameState.CORNER_DOING_2;
				}
				
			break;				
							
			case InGameState.CORNER_DOING_2:
				//Debug.Log("Ingresó a: InGameState.CORNER_DOING_2");
				
				foreach(GameObject go in oponents){
					//Para no cambiar el estado de candidateToThrowIn
					if(go.gameObject==candidateToThrowIn.gameObject){}

					else{
						go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
					}
						//go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
					//Debug.Log("Estado "+go.GetComponent<Player_Script>().state);

				}
				foreach(GameObject go in players){
					//Para no cambiar el estado de candidateToThrowIn
					if(go.gameObject==candidateToThrowIn.gameObject){}
					else{
						go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
					}
					//Debug.Log("Estado "+go.GetComponent<Player_Script>().state);	

				}
				
				if ( candidateToThrowIn.GetComponent<Animation>()["saque_esquina"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true ) {
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( candidateToThrowIn.transform.forward*7000.0f + new Vector3(0.0f, 3300.0f, 0.0f) );
					//Debug.Log("Ingresó a ADDFORCE AL BALON");					
				} 
				
				if ( candidateToThrowIn.GetComponent<Animation>().IsPlaying("saque_esquina") == false ) {
					state = InGameState.CORNER_DONE;
					
					//Debug.Log("Ingreso a: candidateToThrowIn.GetComponent<Animation>().IsPlaying(saque_esquina) == false");
				}
												
			break;
								
			case InGameState.CORNER_DONE:
				//Debug.Log("Ingresó a: +InGameState.CORNER_DONE");
				
				candidateToThrowIn.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;				
			
				state = InGameState.PLAYING;
				
			break;
							
			case InGameState.GOAL_KICK:  //Saque de Puerta
				 //Debug.Log("Entró a GOAL KICK en InGameState");
				
				sphere.transform.position = goal_kick.position; //Coloca Balon en posicion para el saque de puerta 
				sphere.gameObject.GetComponent<Rigidbody>().isKinematic = true;
				goalKeeper.transform.rotation = goal_kick.transform.rotation;

				goalKeeper.transform.position = new Vector3( goal_kick.transform.position.x, goalKeeper.transform.position.y ,goal_kick.transform.position.z)- (goalKeeper.transform.forward*1.0f);
				goalKeeper.SendMessage("GuardarPosicionesList");
				//Debug.Log("POSICION DEL PORTERO A SACAR BOLA "+goalKeeper.transform.position+" id "+goalKeeper.GetComponent<GoalKeeper_Script>().id);

				goalKeeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GOAL_KICK;//NO HACE NADA
							
				sphere.owner = null;

				foreach ( GameObject go in players ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
				}
				foreach ( GameObject go in oponents ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
				}
				
				sphere.owner = null;

			
				goalKeeper.GetComponent<Animation>().Play("backwards");	
				state = InGameState.GOAL_KICK_RUNNING;
				
				
			break;
			case InGameState.GOAL_KICK_RUNNING:
				//Debug.Log("Entró a InGameState.GOAL_KICK_RUNNING");
				goalKeeper.transform.position -= goalKeeper.transform.forward * Time.deltaTime;
				goalKeeper.SendMessage("GuardarPosicionesList");
				
				if ( goalKeeper.GetComponent<Animation>().IsPlaying("backwards") == false ) {
					goalKeeper.GetComponent<Animation>().Play("saque_esquina");	
					state = InGameState.GOAL_KICK_KICKING;
				}
			
				
			break;	
				
			case InGameState.GOAL_KICK_KICKING:
				//Debug.Log("Entró a InGameState.GOAL_KICK_KICKING");
				goalKeeper.transform.position += goalKeeper.transform.forward * Time.deltaTime;
				goalKeeper.SendMessage("GuardarPosicionesList");

				if ( goalKeeper.GetComponent<Animation>()["saque_esquina"].normalizedTime >= 0.5f && sphere.gameObject.GetComponent<Rigidbody>().isKinematic == true) {
					sphere.gameObject.GetComponent<Rigidbody>().isKinematic = false;
					float force = Random.Range(5000.0f, 12000.0f);
					sphere.gameObject.GetComponent<Rigidbody>().AddForce( (goalKeeper.transform.forward*force) + new Vector3(0,3000.0f,0) );
				}
	
				if ( goalKeeper.GetComponent<Animation>().IsPlaying("saque_esquina") == false ) {

					goalKeeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.GO_ORIGIN;	
					state = InGameState.PLAYING;
					
				}
				
			break;

			case InGameState.GOAL:

				//Asigno las posicones finales
				//Debug.Log("Ingresó a: "+InGameState.GOAL);
					sphere.owner = null;
					sphere.codigo=0.0f;
					//Vector3 playerA;
					//Vector3 playerB;
					j=0;
					
					// Se requiere cambiar posiciones finales de llegada
					
					
					while(j<10){
						//PONER JUGADORES EN ESTADO DE DESCANSO 
						players[j].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						oponents[j].GetComponent<Player_Script>().state = Player_Script.Player_State.RESTING;
						
						j++;

					}
					//Debug.Log("SALIO 1 WHILE InGameState.GOAL");
					j=0;
					
					while(j<10){
						//Recupera las coordenadas iniciales de los jugadores "resetPosition"
						playerA=players[j].GetComponent<Player_Script>().MostrarInicio();
						playerB=oponents[j].GetComponent<Player_Script>().MostrarInicio();
						

						//AMARILLOS ----> OPONENTES
						//ROJOS ----> PLAYERS

						//AMARILLOS MUEVEN EL BALON
						if(scoredbylocal){
							
							if(oponents[j].GetComponent<Player_Script>().mediaCancha1==true){
								oponents[j].GetComponent<Player_Script>().posicionFinal= medioTiempoO;
													
							}else if(oponents[j].GetComponent<Player_Script>().mediaCancha2==true){
								oponents[j].GetComponent<Player_Script>().posicionFinal= new Vector3(0.0f,0.25f,medioTiempoO.z*(-4.0f));
							
							}
							else{
								//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
								oponents[j].GetComponent<Player_Script>().posicionFinal=playerB;
							}
							players[j].GetComponent<Player_Script>().posicionFinal=playerA;

						}
						//ROJOS
						if(scoredbyvisiting){
							
							if(players[j].GetComponent<Player_Script>().mediaCancha1==true){
								players[j].GetComponent<Player_Script>().posicionFinal=medioTiempoP;
												
							}else if(players[j].GetComponent<Player_Script>().mediaCancha2==true){
								Vector3 prue=new Vector3(0.0f,0.25f,medioTiempoP.z*(-4.0f));
								players[j].GetComponent<Player_Script>().posicionFinal= prue;// new Vector3(0.0f,0.0f,medioTiempoP.z*(-4.0f));
								
							}
							else{
								//Asigna la posicion final a la que debe de moverse cada jugador y la accion que debe de realizar
								players[j].GetComponent<Player_Script>().posicionFinal=playerA;
								
							}
							oponents[j].GetComponent<Player_Script>().posicionFinal=playerB;

						}
											
						j++;					

					}
					//Debug.Log("SALIO 2 WHILE InGameState.GOAL");
			
				//*************************************************************************
				//ENCUENTRO CUAL ES LA POSICION MAS LARGA
				//Ver cual es el jugador mas largo hasta la posicion inicial.
				int i=1;
				float dis1;
				float dis2;
				
				//Calcula la distancia entre la posicion final y la inicial y se guarda el jugador con dicha posicion mas larga
				
				while(i<10){
					
					dis1=(players[i-1].GetComponent<Player_Script>().posicionFinal -players[i-1].transform.position).magnitude;
					dis2=(players[i].GetComponent<Player_Script>().posicionFinal -players[i].transform.position).magnitude;
					if(dis1>dis2){
						masLargo_atacante=players[i-1].gameObject;
					}else{masLargo_atacante=players[i].gameObject;}

					dis1=(oponents[i-1].GetComponent<Player_Script>().posicionFinal - oponents[i-1].transform.position).magnitude;
					dis2=(oponents[i].GetComponent<Player_Script>().posicionFinal - oponents[i].transform.position).magnitude;
					if(dis1>dis2){
						masLargo_defensor=oponents[i-1].gameObject;
					}else{masLargo_defensor=oponents[i].gameObject;}
					i++;
				}
				keeper.GetComponent<GoalKeeper_Script>().posicionFinal=keeper.GetComponent<GoalKeeper_Script>().MostrarInicio();
				keeper_oponent.GetComponent<GoalKeeper_Script>().posicionFinal=keeper_oponent.GetComponent<GoalKeeper_Script>().MostrarInicio();
				//Debug.Log("SALIO 3 WHILE InGameState.GOAL");

				//Se calcula la distancia mas larga de los jugadores con la posicion mas larga
				// y se asigna el que tiene la posición mas larga.
				dis1=(masLargo_atacante.GetComponent<Player_Script>().posicionFinal -masLargo_atacante.transform.position).magnitude;
				dis2=(masLargo_defensor.GetComponent<Player_Script>().posicionFinal -masLargo_defensor.transform.position).magnitude;
				if(dis1>dis2){
						jugadorMasLargo=masLargo_atacante.gameObject;
				}else{jugadorMasLargo=masLargo_defensor.gameObject;}

				jugadorMasLargo.GetComponent<Player_Script>().valor_state=2;//state=InGameState=PREPARE_TO_KICK_OFF
				state = InGameState_Script.InGameState.DESPLAZAR;
				
			break;
				
			case InGameState.KICK_OFF://Puntapie Inicial
				//Debug.Log("Ingreso a InGameState.KICK_OFF");
				
				foreach ( GameObject go in players ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;
					go.transform.position = go.GetComponent<Player_Script>().initialPosition;
				}
				foreach ( GameObject go in oponents ) {
					go.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;
					go.transform.position = go.GetComponent<Player_Script>().initialPosition;
					go.SendMessage("GuardarPosicionesList");
				}
				
				keeper.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
				keeper_oponent.GetComponent<GoalKeeper_Script>().state = GoalKeeper_Script.GoalKeeper_State.RESTING;
				
				sphere.owner = null;
				sphere.gameObject.transform.position = center.position;
				sphere.gameObject.GetComponent<Rigidbody>().drag = 0.5f;
				state = InGameState_Script.InGameState.PLAYING;
				
			break;


			case InGameState.PREPARE_TO_KICK_OFF://Prepare Puntapie Inicial
				if(jugadorMasLargo!=null){jugadorMasLargo.GetComponent<Player_Script>().valor_state=0;}
				//////Debug.Log("Ingreso a: "+ InGameState.PREPARE_TO_KICK_OFF);
				PatadaInicial();
				state=InGameState.PRUEBA;
				sphere.owner=null;	
				//Debug.Log("FINAL DE InGameState.PREPARE_TO_KICK_OFF");			
			break;							
				
			}
		
		}
		
	}
	// Esta fucion define tamañanos para los jugadores
	void EstaturaJugadores(GameObject[] arrayPlayers, GameObject[] arrayPlayers2,GameObject portero1, GameObject portero2,float x1,float y1,float z1){

		float x;
		float y;
		float z;
		foreach(GameObject go in arrayPlayers){
			x=Random.Range( 0.0f, x1 );
			y=Random.Range( 0.0f, y1 );
			z=Random.Range( 0.0f, z1 );
			go.transform.localScale=new Vector3(go.transform.localScale.x+x,go.transform.localScale.y+y,go.transform.localScale.z+z);
		}
		foreach(GameObject go in arrayPlayers2){
			x=Random.Range( 0.0f, x1 );
			y=Random.Range( 0.0f, y1 );
			z=Random.Range( 0.0f, z1 );
			go.transform.localScale=new Vector3(go.transform.localScale.x+x,go.transform.localScale.y+y,go.transform.localScale.z+z);
		}

		x=Random.Range( 0.0f, x1 );
		y=Random.Range( 0.0f, y1 );
		z=Random.Range( 0.0f, z1 );
		portero1.transform.localScale=new Vector3(portero1.transform.localScale.x+x,portero1.transform.localScale.y+y,portero1.transform.localScale.z+z);
		
		x=Random.Range( 0.0f, x1 );
		y=Random.Range( 0.0f, y1 );
		z=Random.Range( 0.0f, z1 );
		portero2.transform.localScale=new Vector3(portero2.transform.localScale.x+x,portero2.transform.localScale.y+y,portero2.transform.localScale.z+z);
		
	}

	
	// Search player more close to the ball
	//Busca jugador mas cerca de la pelota

	GameObject SearchPlayerNearBall( GameObject[] arrayPlayers) {
		
	    GameObject candidatePlayer = null;
		float distance = 1000.0f;
		foreach ( GameObject player in arrayPlayers ) {			
			
			//Entra aqui si el jugador  temporalmente seleccionable es false por el negador
			if ( !player.GetComponent<Player_Script>().temporallyUnselectable ) {
				
				Vector3 relativePos = sphere.transform.InverseTransformPoint( player.transform.position );		
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;					
					candidatePlayer = player;					

				}
			}
			
		}
						
		return candidatePlayer;	
	}
		
	// Set players inside area to corner kick
	
	void PutPlayersInCornerArea( GameObject[] arrayDefensores, GameObject[] arrayPlayers, Player_Script.TypePlayer type) {

		int i=0;
		int[] juga=new int[5];
		int j=0;
		//Se define la posicion final al que debe de llegar el jugador dentro del area
		foreach ( GameObject player in arrayPlayers ) {			
			
			if ( player.GetComponent<Player_Script>().type == type ) {
				juga[j]=i;
				//Se obtienen los límites del componente BoxCollider que se ubica en el area grande del portero
				float xmin = areaCorner.GetComponent<BoxCollider>().bounds.min.x;
				float xmax = areaCorner.GetComponent<BoxCollider>().bounds.max.x;
				float zmin = areaCorner.GetComponent<BoxCollider>().bounds.min.z;
				float zmax = areaCorner.GetComponent<BoxCollider>().bounds.max.z;
				
				float x = Random.Range( xmin, xmax );
				float z = Random.Range( zmin, zmax );

				player.GetComponent<Player_Script>().posicionFinal=new Vector3( x, player.transform.position.y ,z);								
				//player.transform.position = new Vector3( x, player.transform.position.y ,z);
				j++;
								
			}
			i++;
						
		}
		i=1;
		//Se busca cual es el jugador que tiene la poscion mas larga entre la actual y la final
		// Y se GUARDA ESE JUGADOR Y SU DISTANCIA
		while(i<j){
			//Debug.Log("Jugador con type ATTACKER: "+(int)(i-1)+" id: "+arrayPlayers[juga[i-1]].GetComponent<Player_Script>().id);			
			
					//Se optine la magnitud de las posiciones finales de cada jugador
			float magnitud1=arrayPlayers[juga[i-1]].GetComponent<Player_Script>().PosMasLarga();
			float magnitud2=arrayPlayers[juga[i]].GetComponent<Player_Script>().PosMasLarga();
					//Guarda jugador y distancia mas larga

			if(magnitud1 > magnitud2){ //Magnitud de distancia para jugador anterior y el actual
				masLargo_atacante=arrayPlayers[juga[i-1]].gameObject;
							
			}else{
				masLargo_atacante=arrayPlayers[juga[i]].gameObject;
							
			}			
				
				//Se pone el jugador a desplazar hasta una posicion dentro del area
			arrayPlayers[juga[i]].GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
			arrayPlayers[juga[i]].GetComponent<Player_Script>().valor_state=0;
								
			
			i++;
		}
		//Se pone le primer jugador del arreglo de atacantes a desplazar.
		arrayPlayers[juga[0]].GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
		arrayPlayers[juga[0]].GetComponent<Player_Script>().valor_state=0;

		//*********************************************************************************** */
		int jaux=j;//para limitar un numero de defensores
		j=0;
		int[] def=new int[5];//Para guardar los defensores
		//Recupera las posicion final del atacante y asigna una posicion para los defensores cerca de esa posicion
		//Hay cuatro defensores y 4 atacantes
		i=0;
		while(i<10){
			//////Debug.Log("Array defensores");	
			if ( arrayDefensores[i].GetComponent<Player_Script>().type == Player_Script.TypePlayer.DEFENDER && j<jaux) {
				
				//Cuando se ingresá aquí se modifica el j
				def[j]=i;
				//Debug.Log("DEFENSOR: "+j +" id:"+arrayDefensores[i].GetComponent<Player_Script>().id);
				//Escoger una posicion alrededor del atacante
				
				//DEFINE LOS LIMITES
				float zmin_tem=arrayPlayers[juga[j]].GetComponent<Player_Script>().posicionFinal.z-2.5f;
				float zmax_tem=arrayPlayers[juga[j]].GetComponent<Player_Script>().posicionFinal.z+2.5f;
				//Optener un valor
				float z = Random.Range( zmin_tem, zmax_tem);
				float zpos = Mathf.Clamp ((z), -54.5f,54.4f );// Mathf.Clamp ((valor),MIN,MAX )()

				float xmin_tem=arrayPlayers[juga[j]].GetComponent<Player_Script>().posicionFinal.x-2.0f;
				float xmax_tem=arrayPlayers[juga[j]].GetComponent<Player_Script>().posicionFinal.x+3.0f;
				//Optener un valor
				float x = Random.Range( xmin_tem, xmax_tem);
				//Asigna la posicion final a la cual debe de llegar el jugador defensor
				arrayDefensores[i].GetComponent<Player_Script>().posicionFinal=new Vector3(x,arrayDefensores[i].transform.position.y,zpos);
				arrayDefensores[i].GetComponent<Player_Script>().state = Player_Script.Player_State.DESPLAZAR;
				arrayDefensores[i].GetComponent<Player_Script>().valor_state=0;

				j++;

			}
			i++;
			
		}

		i=1;
		while(i<j){
			//Se guarda el jugador con la posicion mas larga
			if(arrayDefensores[def[i-1]].GetComponent<Player_Script>().PosMasLarga() > arrayDefensores[i].GetComponent<Player_Script>().PosMasLarga()){
				masLargo_defensor =arrayDefensores[def[i-1]].gameObject;
			}
			else{
				masLargo_defensor =arrayDefensores[i].gameObject;
			}
			i++;
		}
		//****************************************************************************************** */
		//verificar cual jugador mas largo es el mas largo
		if(masLargo_atacante.GetComponent<Player_Script>().PosMasLarga() > masLargo_defensor.GetComponent<Player_Script>().PosMasLarga()){
			jugadorMasLargo=masLargo_atacante.gameObject;
			jugadorMasLargo.GetComponent<Player_Script>().valor_state=3;
		}else{
			jugadorMasLargo=masLargo_defensor.gameObject;
			jugadorMasLargo.GetComponent<Player_Script>().valor_state=3;
						//masLargo_atacante.GetComponent<Player_Script>().valor_state=3;
		}
		//Debug.Log("JUGADOR MAS LARGO: "+jugadorMasLargo.GetComponent<Player_Script>().id);
		
	}
	//Esta funcion coloca jugadores cerca de otro

	
	public void GuardarPosicionesListTXT(GameObject go){
		Debug.Log("GUARDAR LISTA INGRESÓ");
		List<string> lista=go.GetComponent<Player_Script>().ExportarLista();
		string dato;
		int cont=0; //Es para saber cuando se llega a un nuevo id
		foreach(string elemento in lista){
			dato=elemento+"\n";
			archivo.WriteLine(dato);
	

		}
		//go.SendMessage("ReseterListPosiciones");
	
	}
	/*
	public void CerrarArchivoTXT(){
		archivo.Close();
	}
	*/
	//.........................................................
	void PatadaInicial(){
		
		sphere.codigo=1.0f;

		sphere.transform.position = center.position;	
		//Apuntan direccion forward de los jugadores hacia delante
		foreach ( GameObject go in players ) {
			go.transform.LookAt( sphere.transform );
			//GuardarPosicionesListTXT(go);
		
		}
		//archivo.Close();
		
		foreach ( GameObject go in oponents ) {
			go.transform.LookAt( sphere.transform );
			////GuardarPosicionesListTXT(go);
		}
		
				
		 // COLOCAR JUGADORES PARA SAQUE INICIAL
		if ( scoredbylocal==true  ) { //AMARILLOS oponentteam
			////////Debug.Log("Ingreso a InGameState  scorebylocal");

			
			int candidato=0;
			float pos2;
			while(candidato<10){
				
				if(oponents[candidato].GetComponent<Player_Script>().mediaCancha1){
					
					//Vector3 sco=new Vector3(sphere.transform.position.x,0.21f,sphere.transform.position.z) + medioTiempoO;
					Vector3 sco=new Vector3(-0.2f,0.21f,-0.5f);
					oponents[candidato].transform.position=sco;
					//oponents[candidato].SendMessage("GuardarPosicionesList");
					////Debug.Log("POS1: "+oponents[candidato].transform.position+"sco "+sco+" y "+oponents[candidato].transform.position.y);
					oponents[candidato].transform.LookAt( sphere.transform.position );
					////Debug.Log("POS2: "+oponents[candidato].transform.position);
					
					oponents[candidato].GetComponent<Player_Script>().state = Player_Script.Player_State.KICK_OFFER;
					sphere.owner=oponents[candidato].gameObject;
					//////Debug.Log("forward 1:" +sphere.owner.transform.forward);

					////GuardarPosicionesListTXT(oponents[candidato]);
					
				}
				
				if(oponents[candidato].GetComponent<Player_Script>().mediaCancha2){
					oponents[candidato].transform.position=new Vector3(sphere.transform.position.x,0.25f,sphere.transform.position.z) +new Vector3(0.3f,0.0f,medioTiempoO.z*-7.0f);//players[buscar].transform.position + (players[buscar].transform.forward * 5.0f);
					//oponents[candidato].SendMessage("GuardarPosicionesList");

					oponents[candidato].transform.LookAt( sphere.transform.position );
					//////Debug.Log("forward 2:" +sphere.owner.transform.forward);

					////GuardarPosicionesListTXT(oponents[candidato]);
					}

				candidato++;
			}

			
		}

			
		if ( scoredbyvisiting ==true ) { //ROJOS  playerteam

			////////Debug.Log("Ingreso a InGameState  scorebyvisiting");

			
			int candidato=0;
			float pos2;
			while(candidato<10){

				if(players[candidato].GetComponent<Player_Script>().mediaCancha1){
					////////Debug.Log(sphere.transform.position);

					Vector3 sco=new Vector3(-0.2f,0.21f,-0.5f);
					players[candidato].transform.position=new Vector3(-0.25f,0.21f,0.0f) + medioTiempoP;
					players[candidato].SendMessage("GuardarPosicionesList");
					players[candidato].transform.LookAt( sphere.transform.position );
					
					players[candidato].GetComponent<Player_Script>().state = Player_Script.Player_State.KICK_OFFER;
					sphere.owner=players[candidato].gameObject;

					////GuardarPosicionesListTXT(players[candidato]);
					
				}
				
				if(players[candidato].GetComponent<Player_Script>().mediaCancha2){
					players[candidato].transform.position=new Vector3(sphere.transform.position.x,0.25f,sphere.transform.position.z) +new Vector3(0.3f,0.3f,medioTiempoP.z*-4.0f);//players[buscar].transform.position + (players[buscar].transform.forward * 5.0f);
					players[candidato].SendMessage("GuardarPosicionesList");
					players[candidato].transform.LookAt( sphere.transform.position );
					
					////GuardarPosicionesListTXT(players[candidato]);//Guarda posicion
				}

				candidato++;
			}
								
		}
			
		scoredbylocal = false;
		scoredbyvisiting = false;
		//----------------------------------------------------------------------------------------------		

	}
	
}