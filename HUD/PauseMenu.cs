using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
//using ScreenRecorderUnity;

public class PauseMenu : MonoBehaviour
{
    public static bool GamePaused=false;
    private GameObject PauseMenuGUI;
    private GameObject RecordMenuGUI;
    private GameObject CameraMenuGUI;
    private GameObject GUI;
    private int GUIState;
    private ScreenRecording SR;
    //private Record_Screen rs;
    private bool recording = false ; 
    // Update is called once per frame


    private void Start()
    {
        
        GUIState = PlayerPrefs.GetInt("GUIState",1);
        GUI = GameObject.Find("UI");
        if (GUIState==1)
        {
            GUI.SetActive(true);
        }
        else
        {
            GUI.SetActive(false);
        }
        

        PauseMenuGUI = transform.Find("pause").gameObject;
        CameraMenuGUI = transform.Find("CameraMenu").gameObject;
        RecordMenuGUI = transform.Find("recordmenu").gameObject;
        Time.timeScale = PlayerPrefs.GetFloat("TimeScale", 1);
        SR = GameObject.FindObjectOfType(typeof(ScreenRecording)) as ScreenRecording;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)||Input.GetKeyDown(KeyCode.Joystick1Button7))
        {
            if (GamePaused == true)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }


        
    }
    public void Resume()
    {
        PauseMenuGUI.SetActive(false);
        Debug.Log(PlayerPrefs.GetFloat("TimeScale"));
        Time.timeScale = PlayerPrefs.GetFloat("TimeScale",1);
        //Modificar para agregar varias velocidades
        GamePaused = false;

    }

    public void RecordMenu()
    {
        PauseMenuGUI.SetActive(false);
        //EventS1.SetActive(false);
        RecordMenuGUI.SetActive(true);
        //EventS2.SetActive(true);
    }
    public void CameraMenu()
    {
        PauseMenuGUI.SetActive(false);
        //EventS1.SetActive(false);
        CameraMenuGUI.SetActive(true);
        //EventS2.SetActive(true);
    }
    public void GoBack()
    {
        RecordMenuGUI.SetActive(false);
        PauseMenuGUI.SetActive(true);
        //EventS1.SetActive(false);
        
        //EventS2.SetActive(true);
    }
    public void RecordStop()
    {
        if (!recording)
        {
            SR.StartRecording();
            var textUIComp = GameObject.Find("RecordText").GetComponent<TextMeshProUGUI>();
            textUIComp.text = "Stop";
            recording = !recording;
        }
        else
        {
            SR.CanRecord = false;
            SR.StopRecording();
            var textUIComp = GameObject.Find("RecordText").GetComponent<TextMeshProUGUI>();
            textUIComp.text = "Done";
        }
       
        //EventS2.SetActive(true);
    }
    void Pause()
    {
        PauseMenuGUI.SetActive(true);
        Time.timeScale = 0f;
        GamePaused = true;
    }
    public void GameMenu()
    {
        SceneManager.LoadScene("Menu");
        Time.timeScale = 1f;
    }
    public void ExitGame()
    {
        Debug.Log("Exit application");
        Application.Quit();
    }
    public void ChangeGUI()
    {
        if (GUIState==1)
        {
            GUI.SetActive(false);
            var textUIComp = GameObject.Find("ChangeGUIText").GetComponent<TextMeshProUGUI>();
            textUIComp.text = "Enable GUI";
            GUIState = 0;
            PlayerPrefs.SetInt("GUIState", 0);
        }
        else
        {
            GUI.SetActive(true);
            var textUIComp = GameObject.Find("ChangeGUIText").GetComponent<TextMeshProUGUI>();
            textUIComp.text = "Disable GUI";
            GUIState = 1;
            PlayerPrefs.SetInt("GUIState", 1);
        }
        
    }
    public void Quit()
    {
        Debug.Log("Prueba_quit game");
        Application.Quit();
    }


    public void TimeScale(int Selection)
    {
        if (Selection == 0)
        {
            Debug.Log("Seleccion 1");
            PlayerPrefs.SetFloat("TimeScale", 1);
            
        }
        else if (Selection == 1)
        {
            Debug.Log("Seleccion 2");
            PlayerPrefs.SetFloat("TimeScale", 2f); 
        }
        else if (Selection == 2)
        {
            PlayerPrefs.SetFloat("TimeScale", 5);
        }
        else if (Selection == 3)
        {
            PlayerPrefs.SetFloat("TimeScale", 10);
        }
    }
    /*public void Restart()
    {
        
        
        Resume();
    }
    public void OpenNextScene()
    {
        SceneManager.LoadScene("Level1");
       
    }*/
}
