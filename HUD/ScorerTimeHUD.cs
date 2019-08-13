using UnityEngine;
using System.Collections;

public class ScorerTimeHUD : MonoBehaviour {
	
	public float timeMatch = 0.0f;
	public int minutes = 0;
	public int seconds = 0;
	public float TRANSFORM_TIME = 1.0f;
	private InGameState_Script inGame;

	//Para el tiempo de todo el juego.
	public float timeMatch1 = 0.0f;
	public int horas=0;
	public int minutes1 = 0;
	public int seconds1 = 0;
	
	// Use this for initialization
	void Start () {
	
		inGame = GameObject.FindObjectOfType( typeof( InGameState_Script ) ) as InGameState_Script;		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
		//Lo hace en segundos
		if (inGame.state == InGameState_Script.InGameState.PLAYING) {	
			timeMatch += Time.deltaTime * TRANSFORM_TIME;
		}		

		int d = (int)(timeMatch * 100.0f);
		minutes = d / (60 * 100);
		seconds = (d % (60 * 100)) / 100;
		//Debug.Log(minutes);
				
		string time = string.Format ("{0:00}:{1:00}", minutes, seconds);
		//GetComponentInChildren<GUIText> ().text = time;

		//Codigo para tiempo corrido, en minutos
		if (inGame.state != InGameState_Script.InGameState.FIN) {	
			timeMatch1 += Time.deltaTime * TRANSFORM_TIME;
		}		

		int d1= (int)(timeMatch1 * 10.0f);
		minutes1 = d1 / (60 * 100);
		seconds1 = (d1 % (60 * 100)) / 100;
		//Debug.Log(minutes);
				
		//string time = string.Format ("{0:00}:{1:00}", minutes, seconds);
		//GetComponentInChildren<GUIText> ().text = time;

	
	}
	
}
