using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject MMenu;
    public GameObject Select_team;
    public GameObject OptionsMenu;
    
    
    //public GameObject EventS1;
    //public GameObject EventS2;
    //public GameObject BGmusic;
    //private AudioSource Music;
    //private SoundManager SManager;
    //private GameObject MusicSlider;
    //private GameObject AudioSlider;
    // Start is called before the first frame update
    void Start()
    {
        
        /*PlayerPrefs.SetInt("hp", 5);
        Music = BGmusic.GetComponent<AudioSource>();


        MusicSlider = GameObject.Find("MusicSlider");
        Music.volume= PlayerPrefs.GetFloat("MusicVolume", 0);
        if (MusicSlider != null)
        {
            MusicSlider.GetComponent<Slider>().value = Music.volume;
        }
        


        AudioSlider = GameObject.Find("AudioSlider");
        AudioListener.volume= PlayerPrefs.GetFloat("AudioVolume", 0);
        if (AudioSlider!=null)
        {
            AudioSlider.GetComponent<Slider>().value = AudioListener.volume;
        }*/
        
    }

    /*public void MusicVolume(float Value)
    {
        Music.volume = Value;
        PlayerPrefs.SetFloat("MusicVolume", Value);
    }
    public void AudioVolume(float Value)
    {
        AudioListener.volume = Value;
        PlayerPrefs.SetFloat("AudioVolume", Value);
    }*/

    public void Play()
    {

        SceneManager.LoadScene("Football_match");

    }




    public void Quit()
    {
        Debug.Log("Prueba_quit game");
        Application.Quit();
    }
    /*public void Tutorial()
    {
        SceneManager.LoadScene("Level0");
    }*/
    public void Options()
    {
        Debug.Log("Options");

        MMenu.SetActive(false);
        //EventS1.SetActive(false);
        OptionsMenu.SetActive(true);
        //EventS2.SetActive(true);
    }
    public void Team_Selection()
    {
        Debug.Log("Team_Selection");

        MMenu.SetActive(false);
        //EventS1.SetActive(false);
        Select_team.SetActive(true);
        //EventS2.SetActive(true);
    }
    public void Back()
    {
        OptionsMenu.SetActive(false);
        Select_team.SetActive(false);
        //EventS2.SetActive(false);
        MMenu.SetActive(true);
        //EventS1.SetActive(true);

    }
    /*public void ShieldVisit()
    {
        gameObject.GetComponent<Renderer>().material = selectMaterial;
        Selected = shield.nameTeam;
    }*/
    public void Graphics(int Selection)
    {
        if (Selection==0)
        {
            Debug.Log("Seleccion Ultra");
            QualitySettings.SetQualityLevel(5, true);
        }
        else if (Selection == 1)    
        {
            Debug.Log("Sellecion High");
            QualitySettings.SetQualityLevel(3, true);
        }
        else if (Selection ==2)
        {
            Debug.Log("Seleccion Normal");
            QualitySettings.SetQualityLevel(2, true);
        }
        else if (Selection==3)
        {
            Debug.Log("Seleccion Low");
            QualitySettings.SetQualityLevel(0, true);
        } 
    }
}
