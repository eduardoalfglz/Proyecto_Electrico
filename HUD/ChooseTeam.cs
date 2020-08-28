using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class ChooseTeam : MonoBehaviour {

	
	public Material normalMaterial;
	public Material selectMaterial;

	public ShieldMenu[] shields;

	public string Selected;
	public string localOrVisit;
	// Use this for initialization
	void Start () {


		if ( localOrVisit == "Local") {
			Selected = "blue";
		} else {
			Selected = "green";
		}
	
	}
	//Nota se puede cambiar el menú completamente para hacerlo más sencillo, con los player prefabs se puede guardar cuál es el uniforme que se quiere
	// Update is called once per frame
	void Update () {
	
		// control buttons in menu
		if ( Input.GetMouseButton(0) ) {

			Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
			inputPos.z = 0;

			RaycastHit2D hit;
			hit = Physics2D.Raycast( inputPos, new Vector2( 0,0 ) );

			if ( hit.collider != null ) {

				foreach ( ShieldMenu shield in shields ) {

					if ( hit.collider == shield.GetComponent<BoxCollider2D>() ) {

						foreach ( ShieldMenu _shield in shields ) {
							_shield.GetComponent<Renderer>().material = normalMaterial; 
						}

						shield.GetComponent<Renderer>().material = selectMaterial;
						Selected = shield.nameTeam;
                        
                    }
				}
			}
            //This is the most useless code ever :)
			if ( hit.collider && hit.collider.tag == "playbutton" ) {

				
                //SceneManager.LoadScene("Football_match");
                PlayerPrefs.SetString(localOrVisit, Selected);
                //Application.LoadLevel("Football_match");

            }


		}




	}
}

