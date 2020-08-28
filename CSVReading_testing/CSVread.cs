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
    private List<string> player_name = new List<string>();
    private List<int> player_lat = new List<int>();
    private List<int> player_lon = new List<int>();
    private List<int> player_alt = new List<int>();



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
                        
                        
                        Vector3 temp_Location = new Vector3(800, 800, 800);

                        SPlayer tempPlayer = Instantiate(playerPrefab, temp_Location, Quaternion.identity, Local.transform);
                        tempPlayer.name = player_name[player_count];
                        tempPlayer.PlayerId = player_count;

                        
                        Locals.Add(tempPlayer);
                        player_count++;
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
                        j++;
                        j++;

                        if (Regex.IsMatch(temp[j], "GPS ALT", RegexOptions.IgnoreCase))
                        {
                            player_alt.Add(j);
                        }
                        else
                        {
                            Debug.LogError("No existe Altura :(");
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
                    Debug.Log(player_alt[j]);
                }
                    
            } else
            {
                string[] temp = inp_ln.Split(',');

                for (int j = 0; j < player_count; j++)
                {
                    if (temp[player_lat[j]] != "" && temp[player_lon[j]] != "" && temp[player_alt[j]] != "")
                    {
                        float temp_lat = float.Parse(temp[player_lat[j]]);
                        temp_lat *= Mathf.PI / 180;
                        
                        float temp_lon = float.Parse(temp[player_lon[j]]);
                        temp_lon *= Mathf.PI / 180;

                        float temp_alt = float.Parse(temp[player_alt[j]]);
                        float radius = temp_alt + 6371000;
                        float xt = radius * Mathf.Cos(temp_lat) * Mathf.Cos(temp_lon);
                        xt %= 10000;
                        float yt = radius * Mathf.Cos(temp_lat) * Mathf.Sin(temp_lon);
                        yt %= 10000;
                        Locals[j].transform.position = new Vector3(xt, 0.2552834f, yt);
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

