using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text.RegularExpressions;

public class CSVread : MonoBehaviour
{
   
    //Variables to instatiate players
    //public SPlayer[] locals;
    private GameObject Local;
    
    private GameObject GK_LocaL;
    //public SPlayer[] visitors;
    private GameObject Visit;
    
    public SPlayer playerPrefab;
    public SPlayer OpponentPrefab;
    private GameObject GK_Visit;
    //
    [HideInInspector]
    public Sphere sphere;





  
    //GPSEnable Variables
    //###############################################################################
    private bool GPSEnable;
    public List<SPlayer> Locals = new List<SPlayer>();
    public List<SPlayer> Visitors = new List<SPlayer>();
    private List<bool> initializedplayer = new List<bool>();
    private List<string> player_name = new List<string>();
    private List<int> player_lat = new List<int>();
    private List<int> player_lon = new List<int>();
    


    private int player_count=0;


    // Start is called before the first frame update

    private bool firstline = true;
    StreamReader csv_stm;
    void Start()
    {
        Debug.Log(PlayerPrefs.GetString("CSVFile"));
        csv_stm = new StreamReader(PlayerPrefs.GetString("CSVFile"));
        Local = GameObject.Find("Local");
        Visit = GameObject.Find("Visit");


    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!csv_stm.EndOfStream)
        {            
            string inp_ln = csv_stm.ReadLine();
            if (firstline)
            {
                firstline = false;
                string[] temp = inp_ln.Split(',');
                for (int j = 0; j < temp.Length; j++)
                {
                    temp[j] = temp[j].Trim();  //removed the blank spaces  
                    //Debug.Log(temp[j]);
                    if (Regex.IsMatch(temp[j], "GPS LAT", RegexOptions.IgnoreCase))
                    {
                        //do something
                        string temp_name = Regex.Match(temp[j], "(.+)GPS LAT",RegexOptions.IgnoreCase).Value;
                        temp_name = temp_name.Trim();
                        player_name.Add(temp_name);
                        player_count++;
                        initializedplayer.Add(false);
                        
                        Vector3 temp_Location = new Vector3(800, 800, 800);

                        SPlayer tempPlayer = Instantiate(playerPrefab, temp_Location, Quaternion.identity, Local.transform);
                        Locals.Add(tempPlayer);

                        player_lat.Add(j);
                        j++;
                        if (Regex.IsMatch(temp[j], "GPS LON", RegexOptions.IgnoreCase))
                        {
                            player_lon.Add(j);
                        } else
                        {
                            Debug.LogError("No existe latitud :(");
                            Application.Quit();
                        }
                    }
                }
                Debug.Log(player_count);
                for (int j = 0; j < player_count; j++)
                {
                    Debug.Log(player_name[j]);
                    Debug.Log(player_lat[j]);
                    Debug.Log(player_lon[j]);
                    Debug.Log(initializedplayer[j]);
                }
                    
            } else
            {
                string[] temp = inp_ln.Split(',');

                for (int j = 0; j < player_count; j++)
                {

                    if (initializedplayer[j])
                    {
                        if (temp[player_lat[j]] != "" && temp[player_lon[j]] != "")
                        {
                            float temp_lat = float.Parse(temp[player_lat[j]]);
                            temp_lat *= 10;
                            float temp_lon = float.Parse(temp[player_lon[j]]);
                            temp_lon *= 10;
                            Locals[j].transform.position = new Vector3(temp_lat, 0.2552834f, temp_lon);
                        }
                        
                    } else
                    {
                        if (temp[player_lat[j]]!="" && temp[player_lon[j]]!="")
                        {
                            initializedplayer[j] = true;
                            
                            float temp_lat = float.Parse(temp[player_lat[j]]);
                            temp_lat *= 10;
                            float temp_lon = float.Parse(temp[player_lon[j]]);
                            temp_lon *= 10;
                            Vector3 temp_Location = new Vector3(temp_lat, 0.2552834f, temp_lon);

                            SPlayer tempPlayer = Instantiate(playerPrefab, temp_Location, Quaternion.identity, Local.transform);
                            tempPlayer.name = player_name[j];
                            tempPlayer.PlayerId = j;
                            
                            Locals[j] = tempPlayer;

                            //GameObject tempPlayer_delete = Local.transform.Find("Player_Calvo(Clone)").gameObject;
                            //Debug.Log(tempPlayer_delete);

                            //Destroy(tempPlayer);


                        }

                    }
                    
                }
                //Debug.Log(inp_ln);
            }
            

            
            //stringList.Add(inp_ln);

        }
    }

    
    public void OnApplicationQuit()
    {
        csv_stm.Close();
    }
    

}

