using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    
    private GameObject PauseMenuGUI;
    private GameObject CameraMenuGUI;
    
    // Start is called before the first frame update
    void Start()
    {
        PauseMenuGUI = transform.Find("pause").gameObject;
        CameraMenuGUI = transform.Find("CameraMenu").gameObject;
        
        //Camera.main = GameObject.Find("Main_Camera");
        if (PlayerPrefs.GetFloat("PositionX")!=0)
        {
            
            Camera.main.transform.position = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), -1f);
            Camera.main.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("RotationX"), -90f, 0.6f);
        }
        
        /*PlayerPrefs.SetFloat("PositionX", Camera.main.transform.position.x);
        PlayerPrefs.SetFloat("PositionY", Camera.main.transform.position.y);
        PlayerPrefs.SetFloat("RotationX", Camera.main.transform.rotation.x);*/




        //Camera Standard values 
        //position X=105.5, y=73.3, Z=0
        //Angle X=38, y=-90, z=0.6

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void CambiarAltura(bool Aumentar)
    {
        PlayerPrefs.SetFloat("PositionX", Camera.main.transform.position.x);
        if (Aumentar)
        {
            
            PlayerPrefs.SetFloat("PositionY", PlayerPrefs.GetFloat("PositionY") + 1);

        }
        else
        {
            PlayerPrefs.SetFloat("PositionY", PlayerPrefs.GetFloat("PositionY") - 1);
        }

        //Vector3 inicio = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 fin = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), transform.position.z);
        //Camera.main.transform.position = Vector3.Lerp(inicio, fin, Time.deltaTime);
        Camera.main.transform.position = fin;
    }
    public void CambiarProfundidad(bool Aumentar)
    {
        PlayerPrefs.SetFloat("PositionY", Camera.main.transform.position.y);
        if (Aumentar)
        {
            
            PlayerPrefs.SetFloat("PositionX", PlayerPrefs.GetFloat("PositionX") + 1);

        }
        else
        {
            
            PlayerPrefs.SetFloat("PositionX", PlayerPrefs.GetFloat("PositionX") - 1);
        }

        //Vector3 inicio = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 fin = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), transform.position.z);
        Camera.main.transform.position = fin;
        //Camera.main.transform.position = Vector3.Lerp(inicio, fin, Time.deltaTime);
    }
    public void CambiarDistancia(bool Aumentar)
    {
        if (Aumentar)
        {
            
            PlayerPrefs.SetFloat("PositionX", PlayerPrefs.GetFloat("PositionX") + 1);
            PlayerPrefs.SetFloat("PositionY", PlayerPrefs.GetFloat("PositionY") + 1);

        }
        else
        {
            PlayerPrefs.SetFloat("PositionX", PlayerPrefs.GetFloat("PositionX") - 1);
            PlayerPrefs.SetFloat("PositionY", PlayerPrefs.GetFloat("PositionY") - 1);
        }

        //Vector3 inicio = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        Vector3 fin = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), transform.position.z);
        
        Camera.main.transform.position = fin;
    }
    public void CambiarAngulo(bool Aumentar)
    {
        //PlayerPrefs.SetFloat("RotationX", Camera.main.transform.rotation.x);

        if (Aumentar)
        {
            PlayerPrefs.SetFloat("RotationX", PlayerPrefs.GetFloat("RotationX") + 0.5f);

        }
        else
        {
            PlayerPrefs.SetFloat("RotationX", PlayerPrefs.GetFloat("RotationX") - 0.5f);
        }

        
        Quaternion fin = Quaternion.Euler(PlayerPrefs.GetFloat("RotationX"), -90f, 0.6f);
        Camera.main.transform.rotation = fin;
    }
    public void GoBack()
    {
        CameraMenuGUI.SetActive(false);
        PauseMenuGUI.SetActive(true);
        //EventS1.SetActive(false);

        //EventS2.SetActive(true);
    }
    public void ResetPosition()
    {
        PlayerPrefs.SetFloat("PositionX",105.5f);
        PlayerPrefs.SetFloat("PositionY", 73.3f);
        PlayerPrefs.SetFloat("PositionZ", 0f);
        Camera.main.transform.position = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), PlayerPrefs.GetFloat("PositionZ"));
        PlayerPrefs.SetFloat("RotationX", 38f);
        PlayerPrefs.SetFloat("RotationY", -90f);
        PlayerPrefs.SetFloat("RotationZ", 0.6f);
        Camera.main.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("RotationX"), PlayerPrefs.GetFloat("RotationY"), PlayerPrefs.GetFloat("RotationZ"));
    }
    private void OnApplicationQuit()
    {
        //save playerpreffs
        PlayerPrefs.Save();
    }
    
}
