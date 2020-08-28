using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    
    public float walkSpeed = 5;
    public float runSpeed = 50;
    private float speed;
    public bool MovementEnabled;

    void Start()
    {

        //Camera.main = GameObject.Find("Main_Camera");
        if (PlayerPrefs.GetFloat("PositionX") != 0)
        {

            Camera.main.transform.position = new Vector3(PlayerPrefs.GetFloat("PositionX"), PlayerPrefs.GetFloat("PositionY"), PlayerPrefs.GetFloat("PositionZ"));
            Camera.main.transform.rotation = Quaternion.Euler(PlayerPrefs.GetFloat("RotationX"), -90f, 0.6f);
        }
    }

    void Update()
    {
        if (MovementEnabled)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                speed = runSpeed;
            }
            else
            {
                speed = walkSpeed;
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.Translate(Vector3.left * Time.deltaTime * speed, Space.Self); //LEFT
            }
            if (Input.GetKey(KeyCode.D))
            {
                transform.Translate(Vector3.right * Time.deltaTime * speed, Space.Self); //RIGHT
            }
            if (Input.GetKey(KeyCode.W))
            {
                transform.Translate(Vector3.forward * Time.deltaTime * speed, Space.Self); //FORWARD
            }
            if (Input.GetKey(KeyCode.S))
            {
                transform.Translate(Vector3.back * Time.deltaTime * speed, Space.Self); //BACKWARD
            }
            if (Input.GetKey(KeyCode.E))
            {
                transform.Translate(Vector3.down * Time.deltaTime * speed, Space.Self); //DOWN
            }
            if (Input.GetKey(KeyCode.Q))
            {
                transform.Translate(Vector3.up * Time.deltaTime * speed, Space.Self); //UP
            }
            if (Input.GetKey(KeyCode.L))
            {
                PlayerPrefs.SetFloat("PositionX", Camera.main.transform.position.x);
                PlayerPrefs.SetFloat("PositionY", Camera.main.transform.position.y);
                PlayerPrefs.SetFloat("PositionZ", Camera.main.transform.position.z);
            }
            
        }

        


    }
    private void OnApplicationQuit()
    {
        //save playerpreffs
        PlayerPrefs.Save();
    }
}
