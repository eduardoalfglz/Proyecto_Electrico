using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScorerTimeHUD : MonoBehaviour {
	
    public enum GameState
    {
        KickOff,
        HalfTime,
        Playing,
        Stopped
    }

    public GameState GState;    //Variable para medir tiempo
	private float timeMatch = 0.0f; //Tiempo de inicio
	public int minutes = 0;
	public int seconds = 0;
	public float TRANSFORM_TIME = 1.0f;
	private InGameState_Script inGame;

	//Para el tiempo de todo el juego.
	public float timeMatch1 = 0.0f;
	public int horas=0;
	public int minutes1 = 0;
	public int seconds1 = 0;
    private string time;
    private string score;

    // Use this for initialization
    void Start () {
	
		inGame = GameObject.FindObjectOfType( typeof( InGameState_Script ) ) as InGameState_Script;		
		
	}
	
	// Update is called once per frame
	void Update ()
	{
        //Lo hace en segundos
        if (GState == GameState.Playing || GState==GameState.Stopped)
        {
            timeMatch += Time.deltaTime * TRANSFORM_TIME;
            
        }
        if(GState == GameState.Stopped) { timeMatch1 += Time.deltaTime * TRANSFORM_TIME; } //Tiempo de reposicion

        int d = (int)(timeMatch * 100.0f);
        minutes = d / (60 * 100);
        seconds = (d % (60 * 100)) / 100;
        //Debug.Log(seconds);
        
        time = string.Format("{0:00}:{1:00}", minutes, seconds);
        
        
        score = inGame.score_local + "-" + inGame.score_visiting;
        //GetComponentInChildren<GUIText> ().text = time;

        

        int d1 = (int)(timeMatch1 * 10.0f);
        minutes1 = d1 / (60 * 100);
        seconds1 = (d1 % (60 * 100)) / 100;
        //Debug.Log(minutes);

        //string time = string.Format ("{0:00}:{1:00}", minutes, seconds);
        //GetComponentInChildren<GUIText> ().text = time;
        update_time();

    }
    void update_time()
    {
        //Debug.Log(time);
        //Debug.Log(score);
        if (GameObject.Find("Time") != null)
        {
            var textUIComp = GameObject.Find("Time").GetComponent<Text>();
            textUIComp.text = time;
        }
        else
        {
            Debug.LogError("No se encuentra el tiempo");
        }
        if (GameObject.Find("Score") != null)
        {

            var textUIComp = GameObject.Find("Score").GetComponent<Text>();
            textUIComp.text = score;
        }
        else
        {
            Debug.LogError("No se encuentra el marcador");
        }

    }
	
}
