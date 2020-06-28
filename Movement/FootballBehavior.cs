using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class FootballBehavior : Sbehavior
{
    public Sphere ball;

    private void Awake()
    {
        ball = GameObject.Find("soccer_ball").GetComponent<Sphere>();
    }
    
}
