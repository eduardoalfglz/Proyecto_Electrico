using UnityEngine;
using System.Collections;

public class Banda_Script : MonoBehaviour {
	
	public Sphere sphere;
	public Vector3 direction_throwin;
	private GameObject[] players;
	private GameObject[] opponent;
	
	// Use this for initialization
	void Start () {
		players = GameObject.FindGameObjectsWithTag("PlayerTeam1");   //rojos
		opponent = GameObject.FindGameObjectsWithTag("OpponentTeam");  //Amarillos
		sphere = (Sphere)GameObject.FindObjectOfType( typeof(Sphere) );		
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	

	void OnTriggerEnter( Collider other) {


		// Detect if Players are outside of field
		//if ( (other.gameObject.tag == "PlayerTeam1" || other.gameObject.tag == "OpponentTeam") && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		
		//	if ( other.gameObject != sphere.owner ) {
		//		//other.gameObject.transform.LookAt(other.gameObject.GetComponent<Player_Script>().MostrarInicio());
		//		other.gameObject.GetComponent<Player_Script>().temporallyUnselectable = true;
		//		other.gameObject.GetComponent<Player_Script>().timeToBeSelectable = 0.5f;
		//		other.gameObject.GetComponent<Player_Script>().state = Player_Script.Player_State.GO_ORIGIN;
		//	}
			
		//}

		// Detect if Ball is outside
		//if ( other.gameObject.tag == "Ball" && Camera.main.GetComponent<InGameState_Script>().state == InGameState_Script.InGameState.PLAYING ) {
		//	foreach(GameObject go in players){
		//		go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
		//	}
		//	foreach(GameObject go in opponent){
		//		go.GetComponent<Player_Script>().state=Player_Script.Player_State.RESTING;
		//	}

		//	sphere.owner = null;
		//	//Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;
		//	Camera.main.GetComponent<InGameState_Script>().timeToChangeState = 2.0f;

		//	Camera.main.GetComponent<InGameState_Script>().state = InGameState_Script.InGameState.THROW_IN;
		//	Camera.main.GetComponent<InGameState_Script>().positionSide = sphere.gameObject.transform.position;	
				
			
		//}
		
		
		
	}
	
	
}
