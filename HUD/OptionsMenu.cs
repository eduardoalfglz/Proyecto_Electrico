using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{

    public enum SimulationType
    {
        Corner = 0,
        ThrowIn = 1,
        Oclusion = 2,
        Regular = 3,
    };

    private GameObject LenghtSlider;
    private GameObject PlayersSlider;
    // Start is called before the first frame update
    void Start()
    {



        LenghtSlider = GameObject.Find("MatchLength");
        PlayersSlider = GameObject.Find("Players");


        LenghtSlider.GetComponentInChildren<Slider>().value = PlayerPrefs.GetInt("PeriodTime",45);

        PlayersSlider.GetComponentInChildren<Slider>().value = 10;
        PlayerPrefs.SetInt("NPlayers", 10);


        //PlayerPrefs.SetInt("SimType", (int)SimulationType.Regular);
    }

    public void LengthChange(int Value)
    {        
        PlayerPrefs.SetInt("PeriodTime", Value);
    }
    public void PlayerNChange(int Value)
    {
        PlayerPrefs.SetInt("NPlayers", Value);
    }
    



    
    public void Mode(int Selection)
    {
        if (Selection == 0)
        {
            Debug.Log("Seleccion Regular");
            PlayerPrefs.SetInt("SimType", (int)SimulationType.Regular);
        }
        else if (Selection == 1)
        {
            Debug.Log("Seleccion Corner");
            PlayerPrefs.SetInt("SimType", (int)SimulationType.Corner);
        }
        else if (Selection == 2)
        {
            Debug.Log("Seleccion ThrowIn");
            PlayerPrefs.SetInt("SimType", (int)SimulationType.ThrowIn);
        }
        else if (Selection == 3)
        {
            Debug.Log("Seleccion Occlusion");
            PlayerPrefs.SetInt("SimType", (int)SimulationType.Oclusion);
        }
    }
}
