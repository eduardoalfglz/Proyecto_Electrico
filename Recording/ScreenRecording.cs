using UnityEngine;
using ScreenRecorderNET;
//using ScreenRecorderUnity;


//using Microsoft.Expression.
//Comentario para git

public class ScreenRecording : MonoBehaviour
{
    //private bool Recording = false;
    ScreenRecorder screenRec = new ScreenRecorder();
    public bool CanRecord= false;


    private void Start()
    {
        
    }
    public void StartRecording(int frameRate=10, string videoName="VideoFile.mp4")
    {
        screenRec.ScreenRecorderINIT(frameRate, videoName);
        CanRecord = true;
    }

    private void FixedUpdate()
    {
        if (screenRec.CanRecord && Time.timeScale!=0 && CanRecord)
        {
            screenRec.RecordVideo();
        }
    }
    public void StopRecording()
    {
        screenRec.Stop();
    }
    public void OnApplicationQuit()
    {
        if (CanRecord)
        {
            screenRec.Stop();
        }
        
    }
}