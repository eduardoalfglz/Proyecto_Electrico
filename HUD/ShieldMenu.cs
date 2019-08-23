using UnityEngine;
using System.Collections;

public class ShieldMenu : MonoBehaviour {

    //public Material selectMaterial;
    public string nameTeam;
    public string localOrVisit;
    private Material originalMat;
    // Use this for initialization
    void Start () {
        originalMat = GetComponent<Renderer>().material;
        PlayerPrefs.SetString("Local", "blue");
        PlayerPrefs.SetString("Visit", "green");
        if (nameTeam == PlayerPrefs.GetString(localOrVisit))
        {
            gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/selected") as Material;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {

            Vector3 inputPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            inputPos.z = 0;

            RaycastHit2D hit;
            hit = Physics2D.Raycast(inputPos, new Vector2(0, 0));

            if (hit.collider != null)
            {
                if (hit.collider == gameObject.GetComponent<BoxCollider2D>())
                {

                    
                    gameObject.GetComponent<Renderer>().material = Resources.Load("Materials/selected") as Material;
                    PlayerPrefs.SetString(localOrVisit, nameTeam);

                }

            }

        }
        if (nameTeam!=PlayerPrefs.GetString(localOrVisit))
        {
            gameObject.GetComponent<Renderer>().material =originalMat;
        }
    }
    
}
