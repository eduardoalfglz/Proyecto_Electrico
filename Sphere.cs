using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Sphere : MonoBehaviour {
	
	public SPlayer owner;	// the player it owns the ball
	public GameObject inputPlayer;	// player selected
	public GameObject lastInputPlayer;	// last player selected
	
    public List<SPlayer> Locals = new List<SPlayer>();
    public List<SPlayer> Visitors = new List<SPlayer>();
    public Transform shadowBall;
	//public Transform blobPlayerSelected;
	public float timeToSelectAgain = 0.0f;
	public GameObject lastCandidatePlayer;



    
    //[HideInInspector]	
    //public float fHorizontal;
    //[HideInInspector]	
    //public float fVertical;
    //[HideInInspector]	
    //public bool bPassButton;
    //[HideInInspector]	
    //public bool bShootButton;
    //[HideInInspector]
    //public bool bShootButtonFinished;
    //[HideInInspector]		
    //public bool pressiPhoneShootButton = false;
    //[HideInInspector]	
    //public bool pressiPhonePassButton = false;
    //[HideInInspector]	
    //public bool pressiPhoneShootButtonEnded = false;

    //public Joystick_Script joystick;	
    //public InGameState_Script inGame;

    public InGameState_Script inGame;

	public float timeShootButtonPressed = 0.0f;

	public ScorerTimeHUD scorerTime; //AGREGO
	public float codigo =1.0f;

	public int prueba=0;
    [HideInInspector]	
    public bool OutofBounds;
    [HideInInspector]
    public Vector3 OutPosition;
    [HideInInspector]
    public SPlayer LastTouch;

    // Use this for initialization
    void Start () {
        OutPosition = new Vector3(38, 100, 55); //Valor inicial, resetear siempre que se completen los eventos
        // get players, joystick, InGame and Blob
        Locals = GameObject.Find("Local").GetComponent<STeam>().Locals;
        Visitors= GameObject.Find("Visit").GetComponent<STeam>().Visitors;
        
		//joystick = GameObject.FindGameObjectWithTag("joystick").GetComponent<Joystick_Script>();
		inGame = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<InGameState_Script>();
		//blobPlayerSelected = GameObject.FindGameObjectWithTag("PlayerSelected").transform;		
	}


	void LateUpdate() {
		//Sombra de la bola
		shadowBall.position = new Vector3( transform.position.x, 0.35f ,transform.position.z );
		shadowBall.rotation = Quaternion.identity;

	}
	
	// Update is called once per frame
	void Update () {

        if (Mathf.Abs(transform.position.x) > 37.5 || Mathf.Abs(transform.position.z)> 55)
            OutofBounds = true;
        else
            OutofBounds = false;
        if (!OutofBounds)
        {
            if (owner != null)
            {
                LastTouch = this.owner;
                //Debug.Log(owner.name);
                transform.position = owner.transform.position + owner.transform.forward / 1.5f + owner.transform.up / 5.0f;
                float velocity = owner.transform.forward.magnitude * 5f * Time.deltaTime;
                
                
                
                if (velocity > 0)
                {
                    transform.RotateAround(owner.transform.right, velocity * 10.0f);
                }





            }
            else //FIXME: esto no permite que le quiten el balon
            {

                float ballRadius = 0.8f;
                foreach (SPlayer item in Locals)
                {
                    if (Vector3.SqrMagnitude(item.transform.position - transform.position) < ballRadius)
                    {

                        this.owner = item;
                    }
                }

                foreach (SPlayer item in Visitors)
                {
                    if (Vector3.SqrMagnitude(item.transform.position - transform.position) < ballRadius)
                    {
                        this.owner = item;
                    }
                }
            }

        }
        else
        {
            if (OutPosition==new Vector3 (38, 100, 55))
            {
                OutPosition = transform.position;   //Guarda la posición de salida del balón
                //Debug.Log(OutPosition);
            }
        }




        //AGREGUE




    }
    /**
    *@funtion ResetCorner
    *@brief Devuelve las variables a las posiciones iniciales
    **/
    public void ResetCorner()
    {
        
    }

    // activate nearest oponent to ball;
    //void ActivateNearestOponent() {

    //	float distance = 100000.0f;
    //	GameObject candidatePlayer = null;
    //	foreach ( GameObject oponent in opponent ) {			

    //		if ( !oponent.GetComponent<Player_Script>().temporallyUnselectable ) {

    //			oponent.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;

    //			Vector3 relativePos = transform.InverseTransformPoint( oponent.transform.position );

    //			float newdistance = relativePos.magnitude;

    //			if ( newdistance < distance ) {

    //				distance = newdistance;
    //				candidatePlayer = oponent;

    //			}
    //		}

    //	}

    //	// set in STOLE_BALL if player found
    //	if ( candidatePlayer )
    //		 candidatePlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.STOLE_BALL;


    //}

    //// activate nearest player to ball
    //void ActivateNearestPlayer() {

    //	//------------------------PUSE----------------------------------------------------

    //	float distance = 100000.0f;
    //	GameObject candidatePlayer = null;
    //	foreach ( GameObject player in players ) {			

    //		if ( !player.GetComponent<Player_Script>().temporallyUnselectable ) {

    //			player.GetComponent<Player_Script>().state = Player_Script.Player_State.MOVE_AUTOMATIC;

    //			Vector3 relativePos = transform.InverseTransformPoint( player.transform.position );

    //			float newdistance = relativePos.magnitude;

    //			if ( newdistance < distance ) {

    //				distance = newdistance;
    //				candidatePlayer = player;

    //			}
    //		}

    //	}

    //	// set in STOLE_BALL if player found
    //	if ( candidatePlayer )
    //		candidatePlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.STOLE_BALL;
    //	//----------------------------------------------------------------------

    /*	
		lastInputPlayer = inputPlayer;
		
		float distance = 1000000.0f;
		GameObject candidatePlayer = null;
		foreach ( GameObject player in players ) {			
			
			if ( !player.GetComponent<Player_Script>().temporallyUnselectable ) {
				
				Vector3 relativePos = transform.InverseTransformPoint( player.transform.position );
				
				float newdistance = relativePos.magnitude;
				
				if ( newdistance < distance ) {
				
					distance = newdistance;
					candidatePlayer = player;
					
				}
			}
			
		}
		
		timeToSelectAgain += Time.deltaTime;
		if ( timeToSelectAgain > 0.5f ) {
			inputPlayer = candidatePlayer;
			timeToSelectAgain = 0.0f;
		} else {
			candidatePlayer = lastCandidatePlayer;
		}
		
		lastCandidatePlayer = candidatePlayer;
		
		
		if ( inputPlayer != null && candidatePlayer ) {
			blobPlayerSelected.transform.position = new Vector3( candidatePlayer.transform.position.x, candidatePlayer.transform.position.y+0.1f, candidatePlayer.transform.position.z);
			blobPlayerSelected.transform.LookAt( new Vector3( blobPlayerSelected.position.x + fHorizontal, blobPlayerSelected.position.y, blobPlayerSelected.position.z + fVertical  ) );
	
		
			// if player is not in any of this states then just CONTROLLING
			if ( inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PASSING &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.SHOOTING &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.PICK_BALL &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.CHANGE_DIRECTION &&
			     inputPlayer.GetComponent<Player_Script>().state != Player_Script.Player_State.TACKLE

			    )
			{
				inputPlayer.GetComponent<Player_Script>().state = Player_Script.Player_State.CONTROLLING;
			}
		} */

    //}




}